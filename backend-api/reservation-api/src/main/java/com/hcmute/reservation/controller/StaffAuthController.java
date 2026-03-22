package com.hcmute.reservation.controller;

import com.hcmute.reservation.dto.auth.StaffLoginRequest;
import com.hcmute.reservation.dto.auth.StaffLoginResponse;
import com.hcmute.reservation.service.StaffAuthService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/staff/auth")
@RequiredArgsConstructor
public class StaffAuthController {

    private final StaffAuthService staffAuthService;

    /** POST /api/staff/auth/login */
    @PostMapping("/login")
    public ResponseEntity<StaffLoginResponse> login(@Valid @RequestBody StaffLoginRequest req) {
        return ResponseEntity.ok(staffAuthService.login(req));
    }
}
