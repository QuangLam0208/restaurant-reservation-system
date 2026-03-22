package com.hcmute.reservation.controller;

import com.hcmute.reservation.dto.report.DailyReservationReport;
import com.hcmute.reservation.dto.report.NoShowRateResponse;
import com.hcmute.reservation.service.ReportService;
import lombok.RequiredArgsConstructor;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

@RestController
@RequestMapping("/api/reports")
@RequiredArgsConstructor
public class ReportController {

    private final ReportService reportService;

    /** GET /api/reports/reservations-by-date?from=&to= */
    @GetMapping("/reservations-by-date")
    public ResponseEntity<List<DailyReservationReport>> getReservationsByDate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate from,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate to) {
        return ResponseEntity.ok(reportService.getReservationsByDate(from, to));
    }

    /** GET /api/reports/no-show-rate?from=&to= */
    @GetMapping("/no-show-rate")
    public ResponseEntity<NoShowRateResponse> getNoShowRate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate from,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate to) {
        return ResponseEntity.ok(reportService.getNoShowRate(from, to));
    }
}
