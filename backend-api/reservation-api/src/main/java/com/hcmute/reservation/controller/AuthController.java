package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.auth.*;
import com.hcmute.reservation.service.AuthService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.net.URI;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.Map;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
public class AuthController {
    private final AuthService authService;

    /** POST /api/auth/register */
    @PostMapping("/register")
    public ResponseEntity<Map<String, String>> register(@Valid @RequestBody RegisterRequest req) {
        String token = authService.register(req);
        return ResponseEntity.ok(Map.of(
            "message", "Đăng ký thành công. Vui lòng kiểm tra email để xác minh tài khoản.",
            "token", token
        ));
    }

    /** POST /api/auth/resend-verification */
    @PostMapping("/resend-verification")
    public ResponseEntity<Map<String, String>> resendVerification(@RequestBody Map<String, String> req) {
        String email = req.get("email");
        authService.resendVerification(email);
        // Tìm lại token mới nhất để trả về cho frontend restart polling (optional security choice)
        String newToken = authService.getLatestVerificationToken(email); 
        return ResponseEntity.ok(Map.of(
            "message", "Mã xác minh mới đã được gửi tới email của bạn.",
            "token", newToken
        ));
    }

    /** GET /api/auth/verify-email?token= — Xác minh xong Redirect về Trang Thành công/Thất bại */
    @GetMapping("/verify-email")
    public ResponseEntity<Void> verifyEmail(@RequestParam String token) {
        String baseUrl = authService.getBaseUrl();
        try {
            authService.verifyEmail(token);
            // Xác minh thành công -> Đánh dấu Approved để Polling ở trang 1 bắt được
            authService.approveVerification(token);
            
            String redirectUrl = baseUrl + "/auth-success.html";
            return ResponseEntity.status(HttpStatus.FOUND)
                    .location(URI.create(redirectUrl))
                    .build();
        } catch (Exception e) {
            // XÁC MINH THẤT BẠI -> Redirect về trang lỗi kèm message
            String errorMsg = URLEncoder.encode(e.getMessage(), StandardCharsets.UTF_8);
            String redirectUrl = baseUrl + "/auth-error.html?msg=" + errorMsg;
            return ResponseEntity.status(HttpStatus.FOUND)
                    .location(URI.create(redirectUrl))
                    .build();
        }
    }

    /** POST /api/auth/login */
    @PostMapping("/login")
    public ResponseEntity<LoginResponse> login(@Valid @RequestBody LoginRequest req) {
        return ResponseEntity.ok(authService.login(req));
    }

    /** POST /api/auth/forgot-password */
    @PostMapping("/forgot-password")
    public ResponseEntity<Map<String, String>> forgotPassword(@Valid @RequestBody ForgotPasswordRequest req) {
        String token = authService.forgotPassword(req);
        return ResponseEntity.ok(Map.of(
            "message", "Mã đặt lại mật khẩu đã được gửi.",
            "token", token
        ));
    }

    /** POST /api/auth/reset-password */
    @PostMapping("/reset-password")
    public ResponseEntity<Map<String, String>> resetPassword(@Valid @RequestBody ResetPasswordRequest req) {
        authService.resetPassword(req);
        authService.removeResetApproval(req.getToken());
        return ResponseEntity.ok(Map.of("message", "Đặt lại mật khẩu thành công."));
    }

    /** GET /api/auth/reset-password-page?token= — Xác thực xong báo thành công cho Polling */
    @GetMapping("/reset-password-page")
    public ResponseEntity<Void> resetPasswordPage(@RequestParam String token) {
        String baseUrl = authService.getBaseUrl();
        boolean isValid = authService.validateResetToken(token);

        if (isValid) {
            // Token đúng -> Đánh dấu là đã duyệt (Approved) để Polling ở trang 1 bắt được
            authService.approveReset(token);
            
            // Redirect về trang thành công
            String redirectUrl = baseUrl + "/auth-success.html";
            return ResponseEntity.status(HttpStatus.FOUND)
                    .location(URI.create(redirectUrl))
                    .build();
        }

        // Token SAI hoặc HẾT HẠN -> Redirect về trang lỗi
        String redirectUrl = baseUrl + "/auth-error.html?msg=" + 
                URLEncoder.encode("This password reset link is invalid or has expired.", StandardCharsets.UTF_8);
        return ResponseEntity.status(HttpStatus.FOUND)
                .location(URI.create(redirectUrl))
                .build();
    }

    /** GET /api/auth/check-reset-status?token= — Frontend polling gọi để check link đã bấm chưa */
    @GetMapping("/check-reset-status")
    public ResponseEntity<Map<String, Boolean>> checkResetStatus(@RequestParam String token) {
        boolean approved = authService.isResetApproved(token);
        return ResponseEntity.ok(Map.of("approved", approved));
    }

    /** GET /api/auth/check-verify-status?token= — Frontend polling gọi để check link verify đã bấm chưa */
    @GetMapping("/check-verify-status")
    public ResponseEntity<Map<String, Boolean>> checkVerifyStatus(@RequestParam String token) {
        boolean approved = authService.isVerificationApproved(token);
        if (approved) {
            authService.removeVerificationApproval(token);
        }
        return ResponseEntity.ok(Map.of("approved", approved));
    }
}
