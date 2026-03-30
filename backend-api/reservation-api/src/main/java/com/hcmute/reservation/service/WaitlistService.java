package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.waitlist.WaitlistRequest;
import com.hcmute.reservation.model.dto.waitlist.WaitlistResponse;

import java.util.List;

public interface WaitlistService {

    WaitlistResponse addToWaitlist(WaitlistRequest req);

    List<WaitlistResponse> getWaitlist();

    WaitlistResponse skipWaitlistEntry(Long id);
}