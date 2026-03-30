package com.hcmute.reservation.service;

import java.util.Map;

public interface SystemSchedulerService {

    /**
     * Hủy các đơn PENDING_PAYMENT quá hạn (Giải phóng soft-lock).
     */
    Map<String, Object> expireReservations();

    /**
     * Đánh dấu NO_SHOW cho các đơn quá giờ giữ chỗ (Giải phóng bàn).
     */
    Map<String, Object> releaseNoShow();

    /**
     * Quét và cập nhật trạng thái OVERSTAY cho các bàn có khách ngồi quá giờ.
     */
    Map<String, Object> checkOverstay();

    /**
     * Tự động dọn dẹp và giải phóng bàn (về AVAILABLE) sau thời gian buffer_time kể từ lúc thanh toán.
     */
    Map<String, Object> releaseCompletedTables();
}