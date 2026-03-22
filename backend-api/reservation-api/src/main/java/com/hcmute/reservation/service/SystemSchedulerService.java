package com.hcmute.reservation.service;

import com.hcmute.reservation.model.Reservation;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class SystemSchedulerService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;

    @Value("${reservation.soft-lock-minutes:5}")
    private int softLockMinutes;

    @Value("${reservation.grace-period-minutes:15}")
    private int gracePeriodMinutes;

    /**
     * POST /api/system/expire-reservations
     * Hết 5 phút PENDING_PAYMENT → EXPIRED, giải phóng soft lock
     */
    @Transactional
    public Map<String, Object> expireReservations() {
        LocalDateTime expiredBefore = LocalDateTime.now().minusMinutes(softLockMinutes);
        List<Reservation> toExpire = reservationRepository.findExpiredPendingPayments(expiredBefore);
        int count = 0;
        for (Reservation r : toExpire) {
            r.markExpired();
            reservationRepository.save(r);
            // Giải phóng soft lock
            releaseLockedTables(r.getReservationId());
            count++;
        }
        Map<String, Object> result = new HashMap<>();
        result.put("expired", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    /**
     * POST /api/system/release-no-show
     * Quá grace period 15 phút → NO_SHOW, giải phóng bàn
     */
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
        Map<String, Object> result = new HashMap<>();
        result.put("noShow", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    /**
     * POST /api/system/check-overstay
     * Scan SEATED quá end_time → Cập nhật Table_Info OVERSTAY + alert
     */
    @Transactional
    public Map<String, Object> checkOverstay() {
        List<Reservation> overstayed = reservationRepository.findOverstayed(LocalDateTime.now());
        int count = 0;
        for (Reservation r : overstayed) {
            // Cập nhật bàn thành OVERSTAY
            if (r.getTableMappings() != null) {
                r.getTableMappings().forEach(m -> {
                    TableInfo t = m.getTableInfo();
                    if (t.getStatus() == TableStatus.OCCUPIED) {
                        t.setStatus(TableStatus.OVERSTAY);
                        tableInfoRepository.save(t);
                    }
                });
            }
            count++;
        }
        Map<String, Object> result = new HashMap<>();
        result.put("overstayDetected", count);
        result.put("executedAt", LocalDateTime.now().toString());
        return result;
    }

    private void releaseLockedTables(Long reservationId) {
        tableInfoRepository.findAll().stream()
                .filter(t -> reservationId.equals(t.getLockedByReservationId()))
                .forEach(t -> { t.releaseSoftLock(); tableInfoRepository.save(t); });
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
