package com.hcmute.reservation.dto.waitlist;

import com.hcmute.reservation.model.enums.WaitlistStatus;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;

@Data
@Builder
public class WaitlistResponse {
    private Long waitlistId;
    private String customerName;
    private String customerPhone;
    private int guestCount;
    private LocalDateTime joinedAt;
    private WaitlistStatus status;
}
