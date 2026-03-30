package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.dto.report.DailyReservationReport;
import com.hcmute.reservation.model.dto.report.NoShowRateResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.service.ReportService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.sql.Date;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@Service
@RequiredArgsConstructor
public class ReportServiceImpl implements ReportService {

    private final ReservationRepository reservationRepository;

    @Override
    public List<DailyReservationReport> getReservationsByDate(LocalDate from, LocalDate to) {
        validateDateRange(from, to);

        LocalDateTime fromDt = from.atStartOfDay();
        LocalDateTime toDt = to.atTime(LocalTime.MAX);

        Map<LocalDate, Long> countsByDate = reservationRepository.countScheduledByDate(fromDt, toDt).stream()
                .collect(Collectors.toMap(
                        row -> toLocalDate(row[0]),
                        row -> ((Number) row[1]).longValue(),
                        Long::sum,
                        LinkedHashMap::new
                ));

        return Stream.iterate(from, date -> !date.isAfter(to), date -> date.plusDays(1))
                .map(date -> new DailyReservationReport(date.toString(), countsByDate.getOrDefault(date, 0L)))
                .collect(Collectors.toList());
    }

    @Override
    public NoShowRateResponse getNoShowRate(LocalDate from, LocalDate to) {
        validateDateRange(from, to);

        LocalDateTime fromDt = from.atStartOfDay();
        LocalDateTime toDt = to.atTime(LocalTime.MAX);

        long total = reservationRepository.countTotalForServiceDate(fromDt, toDt);
        long noShows = reservationRepository.countNoShowsForServiceDate(fromDt, toDt);
        double rate = total == 0 ? 0.0 : Math.round((double) noShows / total * 10000.0) / 100.0;

        return new NoShowRateResponse(total, noShows, rate);
    }

    private void validateDateRange(LocalDate from, LocalDate to) {
        if (from.isAfter(to)) {
            throw new BadRequestException("Ngày bắt đầu không được nằm sau ngày kết thúc.");
        }
    }

    private LocalDate toLocalDate(Object value) {
        if (value instanceof LocalDate localDate) {
            return localDate;
        }
        if (value instanceof LocalDateTime localDateTime) {
            return localDateTime.toLocalDate();
        }
        if (value instanceof Date sqlDate) {
            return sqlDate.toLocalDate();
        }
        return LocalDate.parse(value.toString());
    }
}
