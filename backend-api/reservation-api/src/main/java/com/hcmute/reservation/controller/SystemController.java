package com.hcmute.reservation.controller;

import com.hcmute.reservation.service.SystemSchedulerService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("/api/system")
@RequiredArgsConstructor
public class SystemController {

    private final SystemSchedulerService schedulerService;

    /** POST /api/system/expire-reservations */
    @PostMapping("/expire-reservations")
    public ResponseEntity<Map<String, Object>> expireReservations() {
        return ResponseEntity.ok(schedulerService.expireReservations());
    }

    /** POST /api/system/release-no-show */
    @PostMapping("/release-no-show")
    public ResponseEntity<Map<String, Object>> releaseNoShow() {
        return ResponseEntity.ok(schedulerService.releaseNoShow());
    }

    /** POST /api/system/check-overstay */
    @PostMapping("/check-overstay")
    public ResponseEntity<Map<String, Object>> checkOverstay() {
        return ResponseEntity.ok(schedulerService.checkOverstay());
    }
}
