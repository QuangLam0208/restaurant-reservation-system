package com.hcmute.reservation.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class NoShowRateResponse {
    private long totalReservations;
    private long noShowCount;
    private double noShowRate; // percentage 0-100
}
