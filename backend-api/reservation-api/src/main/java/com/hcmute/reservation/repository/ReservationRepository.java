package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.Reservation;
import com.hcmute.reservation.model.enums.ReservationStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface ReservationRepository extends JpaRepository<Reservation, Long> {

    List<Reservation> findByStatus(ReservationStatus status);

    // Danh sách đơn đang SEATED
    List<Reservation> findByStatusOrderByStartTimeAsc(ReservationStatus status);

    // Truy vấn booking kế tiếp của một bàn cụ thể
    @Query("SELECT r FROM Reservation r JOIN r.tableMappings m WHERE m.tableInfo.tableId = :tableId " +
            "AND r.status IN ('RESERVED','SEATED') AND r.startTime > :now ORDER BY r.startTime ASC")
    List<Reservation> findNextBookingForTable(@Param("tableId") Long tableId, @Param("now") LocalDateTime now);

    // Đơn RESERVED sắp đến trong N phút
    @Query("SELECT r FROM Reservation r WHERE r.status = 'RESERVED' AND r.startTime BETWEEN :now AND :until")
    List<Reservation> findUpcoming(@Param("now") LocalDateTime now, @Param("until") LocalDateTime until);

    // PENDING_PAYMENT quá hạn → EXPIRED
    @Query("SELECT r FROM Reservation r WHERE r.status = 'PENDING_PAYMENT' AND r.createdAt < :expiredBefore")
    List<Reservation> findExpiredPendingPayments(@Param("expiredBefore") LocalDateTime expiredBefore);

    // RESERVED quá grace period → NO_SHOW
    @Query("SELECT r FROM Reservation r WHERE r.type = 'ONLINE' " +
            "AND r.status = 'RESERVED' AND r.startTime < :graceCutoff")
    List<Reservation> findNoShows(@Param("graceCutoff") LocalDateTime graceCutoff);

    // SEATED quá end_time → OVERSTAY
    @Query("SELECT r FROM Reservation r WHERE r.status = 'SEATED' AND r.endTime < :now")
    List<Reservation> findOverstayed(@Param("now") LocalDateTime now);

    // Báo cáo: đơn đặt bàn theo ngày
    @Query("SELECT DATE(r.createdAt), COUNT(r) FROM Reservation r " +
           "WHERE r.createdAt BETWEEN :from AND :to GROUP BY DATE(r.createdAt) ORDER BY DATE(r.createdAt)")
    List<Object[]> countByDate(@Param("from") LocalDateTime from, @Param("to") LocalDateTime to);

    // Báo cáo: no-show rate
    @Query(value = "SELECT COUNT(*) FROM reservation " +
            "WHERE type = 'ONLINE' " +
            "AND status IN ('RESERVED', 'SEATED', 'COMPLETED', 'NO_SHOW') " +
            "AND created_at BETWEEN :from AND :to",
            nativeQuery = true)
    long countTotal(@Param("from") LocalDateTime from,
                    @Param("to") LocalDateTime to);

    @Query(value = "SELECT COUNT(*) FROM reservation " +
            "WHERE type = 'ONLINE' " +
            "AND status = 'NO_SHOW' " +
            "AND created_at BETWEEN :from AND :to",
            nativeQuery = true)
    long countNoShows(@Param("from") LocalDateTime from,
                      @Param("to") LocalDateTime to);

    // Tìm các tableId đã có reservation RESERVED/SEATED giao với khoảng [startTime, endTime]
    // Hoặc các tableId đang bị Soft Lock reservation CREATED/PENDING_PAYMENT (có người dùng online đang đặt)
    @Query("SELECT DISTINCT m.tableInfo.tableId FROM ReservationTableMapping m " +
           "WHERE m.reservation.status IN ('CREATED','PENDING_PAYMENT','RESERVED','SEATED') " +
           "AND m.reservation.startTime < :endTime AND m.reservation.endTime > :startTime")
    List<Long> findOccupiedTableIds(@Param("startTime") LocalDateTime startTime,
                                    @Param("endTime") LocalDateTime endTime);

    // Tìm reservation COMPLETED có endTime < now (bàn cần được giải phóng)
    @Query("SELECT r FROM Reservation r WHERE r.status = 'COMPLETED' AND r.endTime < :now")
    List<Reservation> findCompletedWithReleasableTables(@Param("now") LocalDateTime now);
}
