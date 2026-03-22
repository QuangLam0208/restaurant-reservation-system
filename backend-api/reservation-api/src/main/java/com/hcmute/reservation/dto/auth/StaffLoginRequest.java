package com.hcmute.reservation.dto.auth;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
public class StaffLoginRequest {
    @NotBlank
    private String username;

    @NotBlank
    private String password;
}
