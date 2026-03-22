package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.auth.StaffLoginRequest;
import com.hcmute.reservation.dto.auth.StaffLoginResponse;
import com.hcmute.reservation.exception.UnauthorizedException;
import com.hcmute.reservation.model.Account;
import com.hcmute.reservation.repository.AccountRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class StaffAuthService {

    private final AccountRepository accountRepository;
    private final PasswordEncoder passwordEncoder;

    @Transactional
    public StaffLoginResponse login(StaffLoginRequest req) {
        Account account = accountRepository.findByUsername(req.getUsername())
                .orElseThrow(() -> new UnauthorizedException("Tên đăng nhập hoặc mật khẩu không đúng."));
        if (!passwordEncoder.matches(req.getPassword(), account.getPasswordHash())) {
            throw new UnauthorizedException("Tên đăng nhập hoặc mật khẩu không đúng.");
        }
        // Tạo session token (có hiệu lực 8 giờ — 1 ca làm việc)
        String sessionToken = UUID.randomUUID().toString();
        account.setSessionToken(sessionToken);
        account.setSessionExpiresAt(LocalDateTime.now().plusHours(8));
        accountRepository.save(account);
        return new StaffLoginResponse(sessionToken, account.getAccountId(),
                                     account.getUsername(), account.getRole().name());
    }

    @Transactional
    public void logout(String token) {
        // Tìm tài khoản dựa trên session token đang hoạt động
        accountRepository.findBySessionToken(token).ifPresent(account -> {
            account.setSessionToken(null);
            account.setSessionExpiresAt(null);
            accountRepository.save(account);
        });
    }
}
