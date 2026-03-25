package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.reservation.WalkInRequest;
import com.hcmute.reservation.dto.table.AvailableWindowResponse;
import com.hcmute.reservation.dto.waitlist.WaitlistRequest;
import com.hcmute.reservation.dto.waitlist.WaitlistResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.Customer;
import com.hcmute.reservation.model.Waitlist;
import com.hcmute.reservation.model.enums.WaitlistStatus;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.repository.WaitlistRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class WaitlistService {

    private final WaitlistRepository waitlistRepository;
    private final CustomerRepository customerRepository;
    private final ReservationService reservationService;
    private final AvailabilityService availabilityService;

    private WaitlistResponse toResponse(Waitlist waitlist, AvailableWindowResponse suggestion) {
        WaitlistResponse.WaitlistResponseBuilder builder = WaitlistResponse.builder()
                .waitlistId(waitlist.getWaitlistId())
                .customerName(waitlist.getCustomer().getName())
                .customerPhone(waitlist.getCustomer().getPhone())
                .guestCount(waitlist.getGuestCount())
                .allowShortSeating(waitlist.isAllowShortSeating())
                .joinedAt(waitlist.getJoinedAt())
                .status(waitlist.getStatus());

        if (suggestion != null) {
            builder.readyToSeat(true)
                    .seatingType(suggestion.getAvailability().name())
                    .suggestedAvailableUntil(suggestion.getAvailableUntil())
                    .suggestedTableIds(extractSuggestedTableIds(suggestion));
        }

        return builder.build();
    }

    @Transactional
    public WaitlistResponse addToWaitlist(WaitlistRequest req) {
        boolean alreadyWaiting = waitlistRepository
                .existsByCustomerPhoneAndStatus(req.getPhone(), WaitlistStatus.WAITING);
        if (alreadyWaiting) {
            throw new ConflictException("So dien thoai " + req.getPhone() + " da co trong danh sach cho.");
        }

        ensureWaitlistIsNeeded(req.getGuestCount(), req.isAllowShortSeating());

        Customer customer = resolveWalkInCustomer(req);

        Waitlist entry = Waitlist.builder()
                .customer(customer)
                .guestCount(req.getGuestCount())
                .allowShortSeating(req.isAllowShortSeating())
                .status(WaitlistStatus.WAITING)
                .build();
        return toResponse(waitlistRepository.save(entry), null);
    }

    public List<WaitlistResponse> getWaitlist() {
        LocalDateTime assessmentTime = LocalDateTime.now();
        Set<Long> consumedTableIds = new HashSet<>();
        return waitlistRepository.findByStatusOrderByJoinedAtAsc(WaitlistStatus.WAITING)
                .stream()
                .map(entry -> {
                    AvailableWindowResponse suggestion = findBestWindow(entry, assessmentTime, consumedTableIds);
                    if (suggestion != null) {
                        consumedTableIds.addAll(extractSuggestedTableIds(suggestion));
                    }
                    return toResponse(entry, suggestion);
                })
                .collect(Collectors.toList());
    }

    @Transactional
    public WaitlistResponse seatWaitlistEntry(Long id) {
        Waitlist entry = waitlistRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Hang doi #" + id + " khong ton tai."));
        if (entry.getStatus() != WaitlistStatus.WAITING) {
            throw new BadRequestException("Chi co the xep ban cho khach dang o trang thai WAITING.");
        }

        AvailableWindowResponse suggestion = findBestWindow(entry, LocalDateTime.now(), Set.of());
        if (suggestion == null) {
            throw new BadRequestException("Hien chua co phuong an xep ban phu hop cho khach trong hang doi #" + id + ".");
        }

        WalkInRequest walkInReq = new WalkInRequest();
        walkInReq.setGuestCount(entry.getGuestCount());
        walkInReq.setCustomerName(entry.getCustomer().getName());
        walkInReq.setCustomerPhone(entry.getCustomer().getPhone());
        applySuggestionToWalkInRequest(walkInReq, suggestion);

        try {
            reservationService.createWalkIn(walkInReq);
        } catch (BadRequestException | ConflictException e) {
            throw new BadRequestException(
                    "Khong the xep ban cho khach trong hang doi #" + id + ": " + e.getMessage());
        }

        entry.seat();
        return toResponse(waitlistRepository.save(entry), null);
    }

    @Transactional
    public WaitlistResponse skipWaitlistEntry(Long id) {
        Waitlist entry = waitlistRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Hang doi #" + id + " khong ton tai."));
        if (entry.getStatus() != WaitlistStatus.WAITING) {
            throw new BadRequestException("Chi co the bo qua khach dang o trang thai WAITING.");
        }
        entry.skip();
        return toResponse(waitlistRepository.save(entry), null);
    }

    private void ensureWaitlistIsNeeded(int guestCount, boolean allowShortSeating) {
        if (hasImmediateSeatingOption(guestCount, allowShortSeating, LocalDateTime.now())) {
            throw new BadRequestException("Hien van con phuong an xep ban ngay cho nhom khach nay, khong nen dua vao waitlist.");
        }
    }

    private Customer resolveWalkInCustomer(WaitlistRequest req) {
        return customerRepository.findByPhoneAndPasswordHashIsNull(req.getPhone())
                .map(existing -> {
                    if (req.getName() != null && !req.getName().isBlank() && !req.getName().equals(existing.getName())) {
                        existing.setName(req.getName());
                        return customerRepository.save(existing);
                    }
                    return existing;
                })
                .orElseGet(() -> customerRepository.save(Customer.builder()
                        .name(req.getName())
                        .phone(req.getPhone())
                        .isVerified(true)
                        .build()));
    }

    private boolean hasImmediateSeatingOption(int guestCount, boolean allowShortSeating, LocalDateTime assessmentTime) {
        return availabilityService.getAvailableWindows(guestCount, assessmentTime).stream()
                .anyMatch(window -> isWindowAllowed(window, allowShortSeating));
    }

    private AvailableWindowResponse findBestWindow(Waitlist waitlist,
                                                   LocalDateTime assessmentTime,
                                                   Set<Long> consumedTableIds) {
        return availabilityService.getAvailableWindows(waitlist.getGuestCount(), assessmentTime).stream()
                .filter(window -> isWindowAllowed(window, waitlist.isAllowShortSeating()))
                .filter(window -> consumedTableIds.stream().noneMatch(extractSuggestedTableIds(window)::contains))
                .findFirst()
                .orElse(null);
    }

    private boolean isWindowAllowed(AvailableWindowResponse window, boolean allowShortSeating) {
        return window.getAvailability() == AvailableWindowResponse.Availability.FULL_AVAILABLE || allowShortSeating;
    }

    private void applySuggestionToWalkInRequest(WalkInRequest walkInReq, AvailableWindowResponse suggestion) {
        if (suggestion.getTableId() != null) {
            walkInReq.setTableId(List.of(suggestion.getTableId()));
        } else if (suggestion.getMergeCandidateIds() != null && !suggestion.getMergeCandidateIds().isEmpty()) {
            walkInReq.setMergeTables(true);
        }

        if (suggestion.getAvailability() == AvailableWindowResponse.Availability.PARTIAL_AVAILABLE
                && suggestion.getAvailableUntil() != null) {
            walkInReq.setEndTime(suggestion.getAvailableUntil());
        }
    }

    private List<Long> extractSuggestedTableIds(AvailableWindowResponse window) {
        if (window.getMergeCandidateIds() != null && !window.getMergeCandidateIds().isEmpty()) {
            return window.getMergeCandidateIds();
        }
        if (window.getTableId() != null) {
            return List.of(window.getTableId());
        }
        return List.of();
    }
}
