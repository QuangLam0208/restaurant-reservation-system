package com.hcmute.reservation.controller;

import com.hcmute.reservation.dto.override.OverrideLogResponse;
import com.hcmute.reservation.dto.override.OverrideRequest;
import com.hcmute.reservation.model.Account;
import com.hcmute.reservation.service.OverrideService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;

@RestController
@RequiredArgsConstructor
public class OverrideController {

    private final OverrideService overrideService;

    /** POST /api/reservations/{id}/override */
    @PostMapping("/api/reservations/{id}/override")
    public ResponseEntity<OverrideLogResponse> overrideReservation(
            @PathVariable Long id,
            @Valid @RequestBody OverrideRequest req,
            Authentication auth) {
        Account account = (Account) auth.getPrincipal();
        return ResponseEntity.ok(overrideService.override(id, req, account.getAccountId()));
    }

    /** GET /api/override-logs */
    @GetMapping("/api/override-logs")
    public ResponseEntity<List<OverrideLogResponse>> getLogs(
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime from,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime to,
            @RequestParam(required = false) Long accountId) {
        return ResponseEntity.ok(overrideService.getLogs(from, to, accountId));
    }
}
