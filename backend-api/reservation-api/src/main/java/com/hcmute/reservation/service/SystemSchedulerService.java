package com.hcmute.reservation.service;

import com.hcmute.reservation.model.Reservation;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.transaction.support.TransactionTemplate;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class SystemSchedulerService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final TransactionTemplate transactionTemplate;

    @Value("${reservation.soft-lock-minutes:5}")
    private int softLockMinutes;

    @Value("${reservation.grace-period-minutes:15}")
    private int gracePeriodMinutes;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    /**
     * POST /api/system/expire-reservations
     * Hết 5 phút PENDING_PAYMENT → EXPIRED, giải phóng soft lock
     */
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> expireReservations() {
        List<Reservation> toExpire = reservationRepository
                .findExpiredPendingPayments(LocalDateTime.now().minusMinutes(softLockMinutes));
        int count = 0;

        for (Reservation r : toExpire) {
            try {
                transactionTemplate.executeWithoutResult(status -> {
                    // Re-fetch để tránh stale state
                    Reservation fresh = reservationRepository.findById(r.getReservationId())
                            .orElse(null);
                    if (fresh == null || fresh.getStatus() != ReservationStatus.PENDING_PAYMENT) {
                        return; // Đã được xử lý bởi luồng khác, bỏ qua
                    }
                    fresh.markExpired();
                    reservationRepository.save(fresh);
                    releaseLockedTables(fresh.getReservationId());
                });
                count++;
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi khi hủy đơn {}: {}", r.getReservationId(), e.getMessage());
            }
        }

        if (count > 0) log.info("[Scheduler] expireReservations: {} đơn hết hạn.", count);
        Map<String, Object> result = new HashMap<>();
        result.put("expired", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    /**
     * POST /api/system/release-no-show
     * Quá grace period 15 phút → NO_SHOW, giải phóng bàn
     */
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> releaseNoShow() {
        List<Reservation> toNoShow = reservationRepository
                .findNoShows(LocalDateTime.now().minusMinutes(gracePeriodMinutes));
        int count = 0;

        for (Reservation r : toNoShow) {
            try {
                transactionTemplate.executeWithoutResult(status -> {
                    Reservation fresh = reservationRepository.findById(r.getReservationId())
                            .orElse(null);
                    if (fresh == null || fresh.getStatus() != ReservationStatus.RESERVED) {
                        return;
                    }
                    fresh.markNoShow();
                    reservationRepository.save(fresh);
                    releaseTablesByReservation(fresh);
                });
                count++;
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi khi đánh no-show đơn {}: {}", r.getReservationId(), e.getMessage());
            }
        }

        if (count > 0) log.info("[Scheduler] releaseNoShow: {} đơn no-show.", count);
        Map<String, Object> result = new HashMap<>();
        result.put("noShow", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    /**
     * POST /api/system/check-overstay
     * Scan SEATED quá end_time → Cập nhật Table_Info OVERSTAY + alert
     */
    @Scheduled(fixedDelay = 60_000)
    // LƯU Ý: Không dùng @Transactional ở đây nữa, vì đã dùng TransactionTemplate bên trong
    public Map<String, Object> checkOverstay() {
        LocalDateTime now = LocalDateTime.now();

        // 1. Tìm tất cả các đơn đang ngồi (SEATED) mà giờ kết thúc đã qua (end_time < now)
        // Lưu ý: Dùng findByStatusAndEndTimeBefore rất rõ nghĩa và tốt.
        List<Reservation> overstayReservations = reservationRepository
                .findByStatusAndEndTimeBefore(ReservationStatus.SEATED, now);

        int count = 0;

        for (Reservation res : overstayReservations) {
            try {
                // 2. Bọc mỗi đơn vào 1 Transaction độc lập để chống lỗi "Chết chùm" (All-or-nothing)
                transactionTemplate.executeWithoutResult(status -> {
                    if (res.getTableMappings() != null) {
                        res.getTableMappings().forEach(mapping -> {
                            TableInfo table = mapping.getTableInfo();

                            // Chuyển trạng thái bàn vật lý sang OVERSTAY
                            if (table.getStatus() != TableStatus.OVERSTAY) {
                                table.setStatus(TableStatus.OVERSTAY);
                                tableInfoRepository.save(table);
                            }
                        });
                    }
                });
                count++;
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi khi đánh dấu OVERSTAY cho đơn {}: {}", res.getReservationId(), e.getMessage());
            }
        }

        if (count > 0) log.info("[Scheduler] checkOverstay: {} đơn đã bị đánh dấu OVERSTAY.", count);

        Map<String, Object> result = new HashMap<>();
        result.put("overstayDetected", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    /**
     * POST /api/system/release-completed
     * Giải phóng bàn sau buffer_time kể từ khi checkout.
     * Tìm reservation COMPLETED có endTime < now → bàn vẫn OCCUPIED/OVERSTAY → set AVAILABLE.
     */
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> releaseCompletedTables() {
        // BUG FIX: Chỉ lấy COMPLETED mà bàn vẫn còn OCCUPIED/OVERSTAY (tránh memory leak)
        List<Reservation> completed = reservationRepository
                .findCompletedWithReleasableTables(LocalDateTime.now().minusMinutes(bufferMinutes));
        int count = 0;

        for (Reservation r : completed) {
            try {
                // BUG FIX: Mỗi đơn 1 transaction riêng (chống chết chùm)
                final int[] released = {0};
                transactionTemplate.executeWithoutResult(status -> {
                    Reservation fresh = reservationRepository.findById(r.getReservationId())
                            .orElse(null);
                    if (fresh == null || fresh.getStatus() != ReservationStatus.COMPLETED) {
                        return;
                    }
                    if (fresh.getTableMappings() == null) return;

                    for (var m : fresh.getTableMappings()) {
                        TableInfo t = m.getTableInfo();

                        boolean hasActiveSession = t.getMappings() != null &&
                                t.getMappings().stream().anyMatch(other ->
                                        !other.getReservation().getReservationId()
                                                .equals(fresh.getReservationId()) &&
                                                (other.getReservation().getStatus() == ReservationStatus.SEATED ||
                                                        other.getReservation().getStatus() == ReservationStatus.RESERVED));

                        if (!hasActiveSession &&
                                (t.getStatus() == TableStatus.OCCUPIED ||
                                        t.getStatus() == TableStatus.OVERSTAY)) {
                            t.setStatus(TableStatus.AVAILABLE);
                            tableInfoRepository.save(t);
                            released[0]++;
                        }
                    }
                });
                count += released[0];
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi khi giải phóng bàn cho đơn {}: {}",
                        r.getReservationId(), e.getMessage());
            }
        }

        if (count > 0) log.info("[Scheduler] releaseCompletedTables: {} bàn được giải phóng.", count);
        Map<String, Object> result = new HashMap<>();
        result.put("tablesReleased", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private void releaseLockedTables(Long reservationId) {
        tableInfoRepository.findByLockedByReservationId(reservationId).forEach(t -> {
            t.releaseSoftLock();
            tableInfoRepository.save(t);
        });
    }

    private void releaseTablesByReservation(Reservation r) {
        if (r.getTableMappings() != null) {
            for (var m : r.getTableMappings()) {
                TableInfo t = m.getTableInfo();

                // Guard: Kiểm tra xem bàn này có đang bị một ca SEATED nào khác chiếm dụng vật lý không
                // (Đề phòng trường hợp khách Walk-in đang "Ngồi tạm" - Short Seating)
                boolean isPhysicallyOccupiedByOthers = t.getMappings() != null &&
                        t.getMappings().stream().anyMatch(other ->
                                !other.getReservation().getReservationId().equals(r.getReservationId()) &&
                                        other.getReservation().getStatus() == ReservationStatus.SEATED);

                // Chỉ trả về AVAILABLE nếu không có ai đang ngồi thực tế
                if (!isPhysicallyOccupiedByOthers) {
                    t.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.save(t);
                }
            }
        }
    }
}
