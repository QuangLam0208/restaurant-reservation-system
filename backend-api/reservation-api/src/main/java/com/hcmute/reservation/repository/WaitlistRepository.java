package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.Waitlist;
import com.hcmute.reservation.model.enums.WaitlistStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface WaitlistRepository extends JpaRepository<Waitlist, Long> {
    List<Waitlist> findByStatusOrderByJoinedAtAsc(WaitlistStatus status);
}
