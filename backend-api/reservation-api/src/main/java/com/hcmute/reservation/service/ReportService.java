package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.report.DailyReservationReport;
import com.hcmute.reservation.model.dto.report.NoShowRateResponse;

import java.time.LocalDate;
import java.util.List;

public interface ReportService {

    /**
     * Lấy báo cáo số lượng đơn đặt bàn theo từng ngày trong một khoảng thời gian.
     */
    List<DailyReservationReport> getReservationsByDate(LocalDate from, LocalDate to);

    /**
     * Lấy tỷ lệ khách hàng không đến (No-Show Rate) trong một khoảng thời gian.
     */
    NoShowRateResponse getNoShowRate(LocalDate from, LocalDate to);
}