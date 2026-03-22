package com.hcmute.reservation.dto.reservation;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.time.LocalDateTime;

@Data
public class WalkInRequest {
    @NotNull @Min(1)
    private Integer guestCount;

    @NotBlank
    private String customerName;

    @NotBlank
    private String customerPhone;

    // null = auto-detect now
    private LocalDateTime startTime;

    // Cho phép chỉ định bàn cụ thể (optional, null = auto-assign)
    private Long tableId;

    // Ghép bàn: nếu 1 bàn không đủ
    private boolean mergeTables;

    private String note;
}
