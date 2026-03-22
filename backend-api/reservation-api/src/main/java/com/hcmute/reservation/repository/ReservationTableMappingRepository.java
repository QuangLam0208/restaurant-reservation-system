package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.ReservationTableMapping;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ReservationTableMappingRepository extends JpaRepository<ReservationTableMapping, Long> {
}
