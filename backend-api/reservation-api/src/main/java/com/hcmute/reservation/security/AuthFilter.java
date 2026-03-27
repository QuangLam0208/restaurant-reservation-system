package com.hcmute.reservation.security;

import com.hcmute.reservation.model.entity.Account;
import com.hcmute.reservation.repository.AccountRepository;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

/**
 * Filter xác thực session token hoặc JWT tuỳ theo header.
 * - X-Staff-Token: session token WinForm POS
 * - Authorization: Bearer <jwt>: token khách hàng
 */
@Component
@RequiredArgsConstructor
public class AuthFilter extends OncePerRequestFilter {

    private final JwtUtil jwtUtil;
    private final AccountRepository accountRepository;

    @Override
    protected void doFilterInternal(HttpServletRequest request,
                                    HttpServletResponse response,
                                    FilterChain filterChain) throws ServletException, IOException {
        // 1) Staff session token
        String staffToken = request.getHeader("X-Staff-Token");
        if (staffToken != null && !staffToken.isBlank()) {
            Optional<Account> accountOpt = accountRepository.findBySessionToken(staffToken);
            if (accountOpt.isPresent()) {
                Account account = accountOpt.get();
                if (account.getSessionExpiresAt() != null &&
                    account.getSessionExpiresAt().isAfter(LocalDateTime.now())) {
                    var auth = new UsernamePasswordAuthenticationToken(
                            account, null,
                            List.of(new SimpleGrantedAuthority("ROLE_RECEPTIONIST"),
                                    new SimpleGrantedAuthority("ROLE_" + account.getRole().name())));
                    SecurityContextHolder.getContext().setAuthentication(auth);
                }
            }
        }

        // 2) Customer JWT
        String authHeader = request.getHeader("Authorization");
        if (authHeader != null && authHeader.startsWith("Bearer ") &&
            SecurityContextHolder.getContext().getAuthentication() == null) {
            String token = authHeader.substring(7);
            if (jwtUtil.isValid(token)) {
                String email = jwtUtil.getEmail(token);
                Long customerId = jwtUtil.getCustomerId(token);
                var auth = new UsernamePasswordAuthenticationToken(
                        email, null,
                        List.of(new SimpleGrantedAuthority("ROLE_CUSTOMER")));
                auth.setDetails(customerId);
                SecurityContextHolder.getContext().setAuthentication(auth);
            }
        }

        filterChain.doFilter(request, response);
    }
}
