package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.report.DailyReservationReport;
import com.hcmute.reservation.dto.report.NoShowRateResponse;
import com.hcmute.reservation.repository.ReservationRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class ReportService {

    private final ReservationRepository reservationRepository;

    public List<DailyReservationReport> getReservationsByDate(LocalDate from, LocalDate to) {
        LocalDateTime fromDt = from.atStartOfDay();
        LocalDateTime toDt = to.atTime(LocalTime.MAX);
        List<Object[]> rows = reservationRepository.countByDate(fromDt, toDt);
        return rows.stream()
                .map(row -> new DailyReservationReport(row[0].toString(), ((Number) row[1]).longValue()))
                .collect(Collectors.toList());
    }

    public NoShowRateResponse getNoShowRate(LocalDate from, LocalDate to) {
        LocalDateTime fromDt = from.atStartOfDay();
        LocalDateTime toDt = to.atTime(LocalTime.MAX);
        long total = reservationRepository.countTotal(fromDt, toDt);
        long noShows = reservationRepository.countNoShows(fromDt, toDt);
        double rate = total == 0 ? 0.0 : Math.round((double) noShows / total * 10000.0) / 100.0;
        return new NoShowRateResponse(total, noShows, rate);
    }
}
