package com.hcmute.reservation.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class DailyReservationReport {
    private String date;
    private long count;
}
