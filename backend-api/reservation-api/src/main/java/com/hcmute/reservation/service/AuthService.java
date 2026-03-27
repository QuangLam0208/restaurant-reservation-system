package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.auth.*;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.UnauthorizedException;
import com.hcmute.reservation.model.Customer;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.security.JwtUtil;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final CustomerRepository customerRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtUtil jwtUtil;
    private final EmailService emailService;
    private final Map<String, Boolean> tokenApprovalMap = new ConcurrentHashMap<>();
    private final Map<String, Boolean> verifyApprovalMap = new ConcurrentHashMap<>();

    @Value("${app.base-url}")
    private String baseUrl;

    @Transactional
    public String register(RegisterRequest req) {
        String verificationToken = UUID.randomUUID().toString();

        var existing = customerRepository.findByEmail(req.getEmail());
        if (existing.isPresent()) {
            Customer c = existing.get();
            if (c.getPasswordHash() == null && !c.getIsVerified()) {
                c.setName(req.getName());
                c.setPhone(req.getPhone());
                c.setPasswordHash(passwordEncoder.encode(req.getPassword()));
                c.setIsVerified(false);
                c.setVerificationToken(verificationToken);
                customerRepository.save(c);
                emailService.sendVerificationEmail(req.getEmail(), verificationToken);
                return verificationToken;
            }

            if (c.getPasswordHash() != null && !c.getIsVerified()) {
                c.setVerificationToken(verificationToken);
                customerRepository.save(c);
                emailService.sendVerificationEmail(req.getEmail(), verificationToken);
                return verificationToken;
            }

            throw new ConflictException("Email đã được đăng ký: " + req.getEmail());
        }

        Customer customer = Customer.builder()
                .name(req.getName())
                .phone(req.getPhone())
                .email(req.getEmail())
                .passwordHash(passwordEncoder.encode(req.getPassword()))
                .isVerified(false)
                .verificationToken(verificationToken)
                .build();
        customerRepository.save(customer);
        emailService.sendVerificationEmail(req.getEmail(), verificationToken);
        return verificationToken;
    }

    @Transactional
    public String verifyEmail(String token) {
        Customer customer = customerRepository.findByVerificationToken(token)
                .orElseThrow(() -> new BadRequestException("Token xác minh không hợp lệ hoặc đã hết hạn."));
        customer.setIsVerified(true);
        customer.setVerificationToken(null);
        customerRepository.save(customer);
        return token;
    }

    public void approveVerification(String token) {
        verifyApprovalMap.put(token, true);
    }

    public boolean isVerificationApproved(String token) {
        return verifyApprovalMap.getOrDefault(token, false);
    }

    public void removeVerificationApproval(String token) {
        verifyApprovalMap.remove(token);
    }

    public LoginResponse login(LoginRequest req) {
        Customer customer = customerRepository.findByEmail(req.getEmail())
                .orElseThrow(() -> new UnauthorizedException("Email hoặc mật khẩu không đúng."));
        if (!customer.getIsVerified()) {
            throw new UnauthorizedException("Tài khoản chưa được xác minh. Vui lòng kiểm tra email.");
        }
        if (!passwordEncoder.matches(req.getPassword(), customer.getPasswordHash())) {
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");
        }
        String token = jwtUtil.generateToken(customer.getEmail(), customer.getCustomerId());
        return new LoginResponse(token, customer.getCustomerId(), customer.getName(), customer.getEmail());
    }

    @Transactional
    public String forgotPassword(ForgotPasswordRequest req) {
        String token = UUID.randomUUID().toString();
        customerRepository.findByEmail(req.getEmail()).ifPresent(customer -> {
            customer.setResetToken(token);
            customer.setResetTokenExpiresAt(LocalDateTime.now().plusMinutes(15));
            customerRepository.save(customer);
            emailService.sendResetPasswordEmail(req.getEmail(), token);
        });
        return token;
    }

    @Transactional
    public void resetPassword(ResetPasswordRequest req) {
        Customer customer = customerRepository.findByResetToken(req.getToken())
                .orElseThrow(() -> new BadRequestException("Token đặt lại mật khẩu không hợp lệ."));
        if (customer.getResetTokenExpiresAt() == null ||
                customer.getResetTokenExpiresAt().isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Token đã hết hạn. Vui lòng yêu cầu lại.");
        }
        customer.setPasswordHash(passwordEncoder.encode(req.getNewPassword()));
        customer.setResetToken(null);
        customer.setResetTokenExpiresAt(null);
        customerRepository.save(customer);
    }

    public boolean validateResetToken(String token) {
        return customerRepository.findByResetToken(token)
                .map(c -> c.getResetTokenExpiresAt() != null && c.getResetTokenExpiresAt().isAfter(LocalDateTime.now()))
                .orElse(false);
    }

    public void approveReset(String token) {
        tokenApprovalMap.put(token, true);
    }

    public boolean isResetApproved(String token) {
        return tokenApprovalMap.getOrDefault(token, false);
    }

    public void removeResetApproval(String token) {
        tokenApprovalMap.remove(token);
    }

    public String getBaseUrl() {
        return baseUrl;
    }
}
