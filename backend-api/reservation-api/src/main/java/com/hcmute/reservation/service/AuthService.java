package com.hcmute.reservation.service;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.UnauthorizedException;
import com.hcmute.reservation.model.dto.auth.*;
import com.hcmute.reservation.model.entity.Customer;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.security.JwtUtil;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final CustomerRepository customerRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtUtil jwtUtil;
    private final EmailService emailService;

    @Transactional
    public void register(RegisterRequest req) {
        String verificationToken = UUID.randomUUID().toString();

        var existing = customerRepository.findByEmail(req.getEmail());
        if (existing.isPresent()) {
            Customer c = existing.get();
            if (c.getPasswordHash() == null && !c.getIsVerified()) {
                // Walk-in customer chưa có tài khoản thật → nâng cấp thay vì từ chối
                c.setName(req.getName());
                c.setPhone(req.getPhone());
                c.setPasswordHash(passwordEncoder.encode(req.getPassword()));
                c.setIsVerified(false);
                c.setVerificationToken(verificationToken);
                customerRepository.save(c);
                emailService.sendVerificationEmail(req.getEmail(), verificationToken);
                return; // dừng tại đây, không tạo thêm record
            }

            // ĐÃ ĐĂNG KÝ nhưng chưa verify email (quên check mail)
            if (c.getPasswordHash() != null && !c.getIsVerified()) {
                c.setVerificationToken(verificationToken);
                customerRepository.save(c);
                emailService.sendVerificationEmail(req.getEmail(), verificationToken);
                return;
            }

            throw new ConflictException("Email đã được đăng ký: " + req.getEmail());
        }

        // Email chưa tồn tại → tạo tài khoản mới
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
    }

    @Transactional
    public void verifyEmail(String token) {
        Customer customer = customerRepository.findByVerificationToken(token)
                .orElseThrow(() -> new BadRequestException("Token xác minh không hợp lệ hoặc đã hết hạn."));
        customer.setIsVerified(true);
        customer.setVerificationToken(null);
        customerRepository.save(customer);
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
    public void forgotPassword(ForgotPasswordRequest req) {
        customerRepository.findByEmail(req.getEmail()).ifPresent(customer -> {
            String resetToken = UUID.randomUUID().toString();
            customer.setResetToken(resetToken);
            customer.setResetTokenExpiresAt(LocalDateTime.now().plusMinutes(15));
            customerRepository.save(customer);
            emailService.sendResetPasswordEmail(req.getEmail(), resetToken);
        });
        // Không tiết lộ email có tồn tại không — luôn trả về thành công
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
}
