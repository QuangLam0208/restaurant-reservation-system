package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.event.TableAlertEvent;
import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.ConfigProviderService;
import com.hcmute.reservation.service.SystemSchedulerService;
import com.hcmute.reservation.service.TableReleaseService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.transaction.support.TransactionTemplate;

import java.time.LocalDateTime;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.function.Consumer;

@Slf4j
@Service
@RequiredArgsConstructor
public class SystemSchedulerServiceImpl implements SystemSchedulerService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final TransactionTemplate transactionTemplate;
    private final ConfigProviderService configProvider;
    private final TableReleaseService tableReleaseService;

    private final ApplicationEventPublisher eventPublisher;
    private final Set<Long> currentlyBlinkingTables = ConcurrentHashMap.newKeySet();

    @Scheduled(fixedDelay = 60_000)
    public void scanAndAlertUpcomingConflicts() {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime windowEnd = now.plusMinutes(15); // Quét trước 15 phút

        List<Reservation> upcoming = reservationRepository.findUpcomingReservationsForAlert(now, windowEnd);

        Set<Long> conflictedTableIdsInThisScan = new HashSet<>();

        // 1. Quét tìm các bàn có nguy cơ
        for (Reservation r : upcoming) {
            if (r.getTableMappings() == null) continue;

            for (var mapping : r.getTableMappings()) {
                TableInfo table = mapping.getTableInfo();

                // Nếu bàn CHƯA TRỐNG (đang bị Occupied/Overstay/SoftLocked)
                if (table.getStatus() != TableStatus.AVAILABLE) {
                    conflictedTableIdsInThisScan.add(table.getTableId());

                    // Nếu bàn này chưa nằm trong danh sách nhấp nháy -> Phát sự kiện báo BẬT nhấp nháy
                    if (!currentlyBlinkingTables.contains(table.getTableId())) {
                        currentlyBlinkingTables.add(table.getTableId());

                        eventPublisher.publishEvent(
                                new TableAlertEvent(this, table.getTableId(), "START_BLINK"));
                        log.info("Bật cảnh báo nhấp nháy cho bàn {}", table.getTableId());
                    }
                }
            }
        }

        // 2. Tắt cảnh báo cho những bàn đã "an toàn"
        // (Trước đó nó nhấp nháy, nhưng lần quét này không còn bị conflict nữa)
        Iterator<Long> iterator = currentlyBlinkingTables.iterator();
        while (iterator.hasNext()) {
            Long tableId = iterator.next();
            if (!conflictedTableIdsInThisScan.contains(tableId)) {
                // Bàn đã trống hoặc đã quá giờ đơn đặt -> Phát sự kiện báo TẮT nhấp nháy
                eventPublisher.publishEvent(
                        new TableAlertEvent(this, tableId, "STOP_BLINK"));
                log.info("Tắt cảnh báo nhấp nháy cho bàn {}", tableId);

                iterator.remove(); // Xóa khỏi danh sách nhớ
            }
        }
    }

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> expireReservations() {
        int softLockMinutes = configProvider.getSoftLockMinutes();

        List<Reservation> toExpire = reservationRepository
                .findExpiredPendingPayments(LocalDateTime.now().minusMinutes(softLockMinutes));

        int count = processBatch(toExpire, ReservationStatus.PENDING_PAYMENT, fresh -> {
            fresh.markExpired();
            reservationRepository.save(fresh);
            tableReleaseService.releaseLockedTable(fresh.getReservationId());
        });

        if (count > 0) log.info("[Scheduler] expireReservations: {} đơn hết hạn.", count);
        return buildResult("expired", count);
    }

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> releaseNoShow() {
        int gracePeriodMinutes = configProvider.getGracePeriodMinutes();
        List<Reservation> toNoShow = reservationRepository
                .findNoShows(LocalDateTime.now().minusMinutes(gracePeriodMinutes));

        int count = processBatch(toNoShow, ReservationStatus.RESERVED, fresh -> {
            fresh.markNoShow();
            reservationRepository.save(fresh);
        });

        if (count > 0) log.info("[Scheduler] releaseNoShow: {} đơn no-show.", count);
        return buildResult("noShow", count);
    }

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> checkOverstay() {
        // 1. Tìm tất cả các đơn đang ngồi (SEATED) mà giờ kết thúc đã qua (end_time < now)
        List<Reservation> overstayReservations = reservationRepository
                .findByStatusAndEndTimeBefore(ReservationStatus.SEATED, LocalDateTime.now());

        int count = processBatch(overstayReservations, ReservationStatus.SEATED, fresh -> {
            if (fresh.getTableMappings() == null) return;

            fresh.getTableMappings().forEach(mapping -> {
                TableInfo table = mapping.getTableInfo();
                if (table.getStatus() != TableStatus.OVERSTAY) {
                    table.setStatus(TableStatus.OVERSTAY);
                    tableInfoRepository.save(table);

                    eventPublisher.publishEvent(
                            new TableStatusChangedEvent(this, table.getTableId(), "OVERSTAY"));
                }
            });
        });

        if (count > 0) log.info("[Scheduler] checkOverstay: {} đơn đã bị đánh dấu OVERSTAY.", count);
        return buildResult("overstayDetected", count);
    }

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> releaseCompletedTables() {
        int bufferMinutes = configProvider.getBufferMinutes();

        // Chỉ lấy COMPLETED mà bàn vẫn còn OCCUPIED/OVERSTAY (tránh memory leak)
        List<Reservation> completed = reservationRepository
                .findCompletedWithReleasableTables(LocalDateTime.now().minusMinutes(bufferMinutes));

        AtomicInteger tablesReleasedCount = new AtomicInteger(0);

        processBatch(completed, ReservationStatus.COMPLETED, fresh -> {
            if (fresh.getTableMappings() == null) return;

            for (var m : fresh.getTableMappings()) {
                TableInfo t = m.getTableInfo();
                int seatedCount = tableInfoRepository.countOtherSeated(t.getTableId(), fresh.getReservationId());
                if (seatedCount == 0 && (t.getStatus() == TableStatus.OCCUPIED || t.getStatus() == TableStatus.OVERSTAY)) {
                    t.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.save(t);
                    tablesReleasedCount.incrementAndGet();

                    eventPublisher.publishEvent(
                            new TableStatusChangedEvent(this, t.getTableId(), "AVAILABLE"));
                }
            }
        });

        int count = tablesReleasedCount.get();
        if (count > 0) log.info("[Scheduler] releaseCompletedTables: {} bàn được giải phóng.", count);
        return buildResult("tablesReleased", count);
    }

    /**
     * Hàm bậc cao (Higher-Order Function) xử lý boilerplate cho Transaction, Try-Catch và Re-fetch.
     */
    private int processBatch(List<Reservation> reservations, ReservationStatus expectedStatus, Consumer<Reservation> action) {
        int successCount = 0;
        for (Reservation r : reservations) {
            try {
                boolean processed = Boolean.TRUE.equals(transactionTemplate.execute(status -> {
                    Reservation fresh = reservationRepository.findById(r.getReservationId()).orElse(null);
                    // Double check để tránh thao tác đè lên dữ liệu cũ (Stale State)
                    if (fresh == null || fresh.getStatus() != expectedStatus) {
                        return false;
                    }
                    action.accept(fresh);
                    return true;
                }));
                if (processed) successCount++;
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi khi xử lý đơn {}: {}", r.getReservationId(), e.getMessage());
            }
        }
        return successCount;
    }

    private Map<String, Object> buildResult(String key, int count) {
        Map<String, Object> result = new HashMap<>();
        result.put(key, count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }
}
