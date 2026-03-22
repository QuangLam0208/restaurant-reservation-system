package com.hcmute.reservation.dto.table;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
public class TableRequest {
    @NotNull @Min(1)
    private Integer capacity;

    private Boolean isActive = true;
}
