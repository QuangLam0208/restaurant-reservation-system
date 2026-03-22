package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface TableInfoRepository extends JpaRepository<TableInfo, Long> {

    List<TableInfo> findByIsActiveTrue();

    List<TableInfo> findByStatusAndIsActiveTrue(TableStatus status);

    // Tìm bàn AVAILABLE có đủ sức chứa, không bị soft-lock
    @Query("SELECT t FROM TableInfo t WHERE t.status = 'AVAILABLE' AND t.isActive = true " +
           "AND t.capacity >= :guests AND (t.softLockUntil IS NULL OR t.softLockUntil < CURRENT_TIMESTAMP)")
    List<TableInfo> findAvailableTablesForGuests(@Param("guests") int guests);
}
