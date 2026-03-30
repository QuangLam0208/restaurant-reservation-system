package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.SystemSchedulerService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.transaction.support.TransactionTemplate;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.function.Consumer;

@Slf4j
@Service
@RequiredArgsConstructor
public class SystemSchedulerServiceImpl implements SystemSchedulerService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final TransactionTemplate transactionTemplate;

    @Value("${reservation.soft-lock-minutes:5}")
    private int softLockMinutes;

    @Value("${reservation.grace-period-minutes:15}")
    private int gracePeriodMinutes;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> expireReservations() {
        List<Reservation> toExpire = reservationRepository
                .findExpiredPendingPayments(LocalDateTime.now().minusMinutes(softLockMinutes));

        int count = processBatch(toExpire, ReservationStatus.PENDING_PAYMENT, fresh -> {
            fresh.markExpired();
            reservationRepository.save(fresh);
            releaseLockedTables(fresh.getReservationId());
        });

        if (count > 0) log.info("[Scheduler] expireReservations: {} đơn hết hạn.", count);
        return buildResult("expired", count);
    }

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> releaseNoShow() {
        List<Reservation> toNoShow = reservationRepository
                .findNoShows(LocalDateTime.now().minusMinutes(gracePeriodMinutes));

        int count = processBatch(toNoShow, ReservationStatus.RESERVED, fresh -> {
            fresh.markNoShow();
            reservationRepository.save(fresh);
            releaseTablesByReservation(fresh);
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
                }
            });
        });

        if (count > 0) log.info("[Scheduler] checkOverstay: {} đơn đã bị đánh dấu OVERSTAY.", count);
        return buildResult("overstayDetected", count);
    }

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> releaseCompletedTables() {
        // Chỉ lấy COMPLETED mà bàn vẫn còn OCCUPIED/OVERSTAY (tránh memory leak)
        List<Reservation> completed = reservationRepository
                .findCompletedWithReleasableTables(LocalDateTime.now().minusMinutes(bufferMinutes));

        AtomicInteger tablesReleasedCount = new AtomicInteger(0);

        processBatch(completed, ReservationStatus.COMPLETED, fresh -> {
            if (fresh.getTableMappings() == null) return;

            for (var m : fresh.getTableMappings()) {
                TableInfo t = m.getTableInfo();
                if (!isPhysicallyOccupiedByOthers(t, fresh.getReservationId()) &&
                        (t.getStatus() == TableStatus.OCCUPIED || t.getStatus() == TableStatus.OVERSTAY)) {
                    t.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.save(t);
                    tablesReleasedCount.incrementAndGet();
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

    private void releaseLockedTables(Long reservationId) {
        tableInfoRepository.findByLockedByReservationId(reservationId).forEach(t -> {
            t.releaseSoftLock();
            tableInfoRepository.save(t);
        });
    }

    private void releaseTablesByReservation(Reservation r) {
        if (r.getTableMappings() == null) return;

        for (var m : r.getTableMappings()) {
            TableInfo t = m.getTableInfo();
            if (!isPhysicallyOccupiedByOthers(t, r.getReservationId())) {
                t.setStatus(TableStatus.AVAILABLE);
                tableInfoRepository.save(t);
            }
        }
    }

    /**
     * Logic kiểm tra xem bàn có đang bị khách khác ngồi không
     */
    private boolean isPhysicallyOccupiedByOthers(TableInfo table, Long currentReservationId) {
        if (table.getMappings() == null) return false;
        return table.getMappings().stream().anyMatch(other ->
                !other.getReservation().getReservationId().equals(currentReservationId) &&
                        (other.getReservation().getStatus() == ReservationStatus.SEATED ||
                                other.getReservation().getStatus() == ReservationStatus.RESERVED));
    }
}
