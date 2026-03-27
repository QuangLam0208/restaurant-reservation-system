package com.hcmute.reservation.security;

import lombok.RequiredArgsConstructor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.CorsConfigurationSource;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;

import java.util.List;

@Configuration
@EnableWebSecurity
@RequiredArgsConstructor
public class SecurityConfig {

    private final AuthFilter authFilter;

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http
            .csrf(csrf -> csrf.disable())
            .cors(cors -> cors.configurationSource(corsConfigurationSource()))
            .sessionManagement(sm -> sm.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
            .authorizeHttpRequests(auth -> auth
                // ── Public: customer auth ─────────────────────────────────────────
                .requestMatchers(
                        "/api/auth/register",
                        "/api/auth/verify-email",
                        "/api/auth/login",
                        "/api/auth/forgot-password",
                        "/api/auth/reset-password",
                        "/api/auth/reset-password-page"
                ).permitAll()
                // ── Public: staff auth (POS login) ────────────────────────────────
                .requestMatchers("/api/staff/auth/login").permitAll()
                // ── Public: availability check ────────────────────────────────────
                .requestMatchers(HttpMethod.GET, "/api/reservations/availability").permitAll()

                // ── CUSTOMER only ─────────────────────────────────────────────────
                .requestMatchers(HttpMethod.POST, "/api/reservations/online").authenticated()
                .requestMatchers(HttpMethod.DELETE, "/api/reservations/**").authenticated()

                // ── MANAGER only ──────────────────────────────────────────────────
                .requestMatchers(HttpMethod.POST, "/api/staff/auth/register").hasRole("MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/tables").hasRole("MANAGER")
                .requestMatchers(HttpMethod.PUT, "/api/tables/*").hasRole("MANAGER")
                .requestMatchers(HttpMethod.DELETE, "/api/tables/*").hasRole("MANAGER")
                .requestMatchers("/api/reports/**").hasRole("MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/override-logs").hasRole("MANAGER")
                // System scheduler endpoints — MANAGER hoặc internal cron
                .requestMatchers("/api/system/**").hasRole("MANAGER")

                // ── STAFF or MANAGER ──────────────────────────────────────────────
                .requestMatchers(HttpMethod.POST, "/api/reservations/walk-in").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/change-table").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/check-in").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/check-out").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/override").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers("/api/waitlist/**").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/tables/floor-map").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/tables/available-windows").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/reservations/active").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/reservations/upcoming").hasAnyRole("RECEPTIONIST", "MANAGER")

                // ── Any authenticated user ────────────────────────────────────────
                .anyRequest().authenticated()
            )
            .addFilterBefore(authFilter, UsernamePasswordAuthenticationFilter.class);

        return http.build();
    }

    @Bean
    public CorsConfigurationSource corsConfigurationSource() {
        CorsConfiguration config = new CorsConfiguration();
        config.setAllowedOriginPatterns(List.of("*"));
        config.setAllowedMethods(List.of("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS"));
        config.setAllowedHeaders(List.of("*"));
        config.setAllowCredentials(true);
        UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
        source.registerCorsConfiguration("/**", config);
        return source;
    }
}
