package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.waitlist.WaitlistRequest;
import com.hcmute.reservation.dto.waitlist.WaitlistResponse;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.Customer;
import com.hcmute.reservation.model.Waitlist;
import com.hcmute.reservation.model.enums.WaitlistStatus;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.repository.WaitlistRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class WaitlistService {

    private final WaitlistRepository waitlistRepository;
    private final CustomerRepository customerRepository;

    private WaitlistResponse toResponse(Waitlist w) {
        return WaitlistResponse.builder()
                .waitlistId(w.getWaitlistId())
                .customerName(w.getCustomer().getName())
                .customerPhone(w.getCustomer().getPhone())
                .guestCount(w.getGuestCount())
                .joinedAt(w.getJoinedAt())
                .status(w.getStatus())
                .build();
    }

    @Transactional
    public WaitlistResponse addToWaitlist(WaitlistRequest req) {
        // Tìm hoặc tạo walk-in customer (dùng query có index thay vì findAll)
        Customer customer = customerRepository.findByPhoneAndPasswordHashIsNull(req.getPhone())
                .orElseGet(() -> customerRepository.save(Customer.builder()
                        .name(req.getName())
                        .phone(req.getPhone())
                        .isVerified(true)
                        .build()));

        Waitlist entry = Waitlist.builder()
                .customer(customer)
                .guestCount(req.getGuestCount())
                .status(WaitlistStatus.WAITING)
                .build();
        return toResponse(waitlistRepository.save(entry));
    }

    public List<WaitlistResponse> getWaitlist() {
        return waitlistRepository.findByStatusOrderByJoinedAtAsc(WaitlistStatus.WAITING)
                .stream().map(this::toResponse).collect(Collectors.toList());
    }

    @Transactional
    public WaitlistResponse seatWaitlistEntry(Long id) {
        Waitlist entry = waitlistRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Hàng đợi #" + id + " không tồn tại."));
        entry.seat();
        return toResponse(waitlistRepository.save(entry));
    }

    @Transactional
    public WaitlistResponse skipWaitlistEntry(Long id) {
        Waitlist entry = waitlistRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Hàng đợi #" + id + " không tồn tại."));
        entry.skip();
        return toResponse(waitlistRepository.save(entry));
    }
}
