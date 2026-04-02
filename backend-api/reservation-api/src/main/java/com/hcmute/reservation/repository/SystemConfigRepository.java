package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.SystemConfig;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface SystemConfigRepository extends JpaRepository<SystemConfig, String> {
    // Kế thừa sẵn findById (chính là find by configKey)
}