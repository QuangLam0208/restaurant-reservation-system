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

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class SystemSchedulerService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;

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
    @Transactional
    public Map<String, Object> expireReservations() {
        LocalDateTime expiredBefore = LocalDateTime.now().minusMinutes(softLockMinutes);
        List<Reservation> toExpire = reservationRepository.findExpiredPendingPayments(expiredBefore);
        int count = 0;
        for (Reservation r : toExpire) {
            r.markExpired();
            reservationRepository.save(r);
            releaseLockedTables(r.getReservationId());
            count++;
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
    @Transactional
    public Map<String, Object> releaseNoShow() {
        LocalDateTime graceCutoff = LocalDateTime.now().minusMinutes(gracePeriodMinutes);
        List<Reservation> toNoShow = reservationRepository.findNoShows(graceCutoff);
        int count = 0;
        for (Reservation r : toNoShow) {
            r.markNoShow();
            reservationRepository.save(r);
            releaseTablesByReservation(r);
            count++;
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
    @Transactional
    public Map<String, Object> checkOverstay() {
        List<Reservation> overstayed = reservationRepository.findOverstayed(LocalDateTime.now());
        List<Long> overstayTableIds = new ArrayList<>(); // Danh sách ID bàn

        for (Reservation r : overstayed) {
            if (r.getTableMappings() != null) {
                r.getTableMappings().forEach(m -> {
                    TableInfo t = m.getTableInfo();
                    if (t.getStatus() == TableStatus.OCCUPIED) {
                        t.setStatus(TableStatus.OVERSTAY);
                        tableInfoRepository.save(t);
                        overstayTableIds.add(t.getTableId()); // Lưu lại ID bàn
                    }
                });
            }
        }

        if (!overstayTableIds.isEmpty()) {
            log.info("[Scheduler] checkOverstay: Các bàn bị overstay {}", overstayTableIds);
        }

        Map<String, Object> result = new HashMap<>();
        result.put("overstayTableIds", overstayTableIds); // Trả về list ID cụ thể
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    @Scheduled(fixedRate = 60000)
    @Transactional
    public void scanAndMarkOverstay() {
        LocalDateTime now = LocalDateTime.now();

        // 1. Tìm tất cả các đơn đang ngồi (SEATED) mà giờ kết thúc đã qua (end_time < now)
        List<Reservation> overstayReservations = reservationRepository
                .findByStatusAndEndTimeBefore(ReservationStatus.SEATED, now);

        for (Reservation res : overstayReservations) {
            if (res.getTableMappings() != null) {
                res.getTableMappings().forEach(mapping -> {
                    TableInfo table = mapping.getTableInfo();
                    // 2. Chuyển trạng thái bàn vật lý sang OVERSTAY
                    if (table.getStatus() != TableStatus.OVERSTAY) {
                        table.setStatus(TableStatus.OVERSTAY);
                        tableInfoRepository.save(table);
                    }
                });
            }
        }
    }

    /**
     * POST /api/system/release-completed
     * Giải phóng bàn sau buffer_time kể từ khi checkout.
     * Tìm reservation COMPLETED có endTime < now → bàn vẫn OCCUPIED/OVERSTAY → set AVAILABLE.
     */
    @Scheduled(fixedDelay = 60_000)
    @Transactional
    public Map<String, Object> releaseCompletedTables() {
        // Lấy các đơn cần nhả bàn theo query tối ưu mới
        List<Reservation> toRelease = reservationRepository.findReservationsNeedingTableRelease(LocalDateTime.now().minusMinutes(bufferMinutes));
        int count = 0;

        for (Reservation r : toRelease) {
            if (r.getTableMappings() != null) {
                for (var m : r.getTableMappings()) {
                    TableInfo t = m.getTableInfo();

                    // Guard: kiểm tra bàn không có ca SEATED/RESERVED khác đang chạy
                    boolean hasActiveSession = t.getMappings() != null &&
                            t.getMappings().stream().anyMatch(other ->
                                    !other.getReservation().getReservationId().equals(r.getReservationId()) &&
                                            (other.getReservation().getStatus() == ReservationStatus.SEATED ||
                                                    other.getReservation().getStatus() == ReservationStatus.RESERVED));

                    if (!hasActiveSession &&
                            (t.getStatus() == TableStatus.OCCUPIED || t.getStatus() == TableStatus.OVERSTAY)) {
                        t.setStatus(TableStatus.AVAILABLE);
                        tableInfoRepository.save(t);
                        count++;
                    }
                }
            }
        }

        if (count > 0) log.info("[Scheduler] releaseCompletedTables: {} bàn được giải phóng từ các đơn COMPLETED/CANCELLED.", count);

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
            r.getTableMappings().forEach(m -> {
                m.getTableInfo().setStatus(TableStatus.AVAILABLE);
                tableInfoRepository.save(m.getTableInfo());
            });
        }
    }
}
