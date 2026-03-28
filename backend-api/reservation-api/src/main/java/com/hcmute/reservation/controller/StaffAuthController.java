package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.auth.StaffLoginRequest;
import com.hcmute.reservation.model.dto.auth.StaffLoginResponse;
import com.hcmute.reservation.model.dto.auth.StaffRegisterRequest;
import com.hcmute.reservation.service.StaffAuthService;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

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

    @PostMapping("/logout")
    public ResponseEntity<Void> logout(HttpServletRequest request) {
        String token = extractToken(request);
        if (token != null && !token.isBlank()) {
            staffAuthService.logout(token);
        }
        return ResponseEntity.noContent().build();
    }

    @PostMapping("/register")
    public ResponseEntity<Map<String, String>> registerStaff(@Valid @RequestBody StaffRegisterRequest req) {
        staffAuthService.register(req);
        return ResponseEntity.ok(Map.of("message",
                "Tạo tài khoản " + req.getRole().name() + " thành công!"));
    }

    // Hàm bổ trợ để trích xuất token từ Header tùy chỉnh
    private String extractToken(HttpServletRequest request) {
        return request.getHeader("X-Staff-Token");
    }

}
