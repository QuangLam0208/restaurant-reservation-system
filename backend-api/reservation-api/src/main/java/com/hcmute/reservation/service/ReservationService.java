package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.reservation.OnlineReservationRequest;
import com.hcmute.reservation.dto.reservation.ReservationResponse;
import com.hcmute.reservation.dto.reservation.WalkInRequest;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.*;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.ReservationType;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.*;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class ReservationService {

    private final ReservationRepository reservationRepository;
    private final CustomerRepository customerRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;

    @Value("${reservation.duration-minutes:120}")
    private int durationMinutes;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Value("${reservation.soft-lock-minutes:5}")
    private int softLockMinutes;

    // ────── Helpers ──────────────────────────────────────────────────

    private ReservationResponse toResponse(Reservation r) {
        List<Long> tableIds = r.getTableMappings() == null ? List.of() :
                r.getTableMappings().stream()
                        .map(m -> m.getTableInfo().getTableId())
                        .collect(Collectors.toList());
        return ReservationResponse.builder()
                .reservationId(r.getReservationId())
                .status(r.getStatus())
                .type(r.getType())
                .guestCount(r.getGuestCount())
                .startTime(r.getStartTime())
                .endTime(r.getEndTime())
                .note(r.getNote())
                .createdAt(r.getCreatedAt())
                .customerId(r.getCustomer() != null ? r.getCustomer().getCustomerId() : null)
                .customerName(r.getCustomer() != null ? r.getCustomer().getName() : null)
                .customerPhone(r.getCustomer() != null ? r.getCustomer().getPhone() : null)
                .tableIds(tableIds)
                .build();
    }

    // ────── 3.2.2 Online booking ──────────────────────────────────────

    @Transactional
    public ReservationResponse createOnlineReservation(OnlineReservationRequest req, Long customerId) {
        Customer customer = customerRepository.findById(customerId)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng."));

        // Auto-assign bàn + soft lock
        List<TableInfo> available = tableInfoRepository.findAvailableTablesForGuests(req.getGuestCount());
        if (available.isEmpty()) {
            throw new BadRequestException("Hiện không có bàn trống phù hợp. Vui lòng chọn giờ khác.");
        }
        TableInfo table = available.get(0);

        LocalDateTime start = req.getStartTime();
        LocalDateTime end = start.plusMinutes(durationMinutes + bufferMinutes);

        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.ONLINE)
                .guestCount(req.getGuestCount())
                .startTime(start)
                .endTime(end)
                .status(ReservationStatus.PENDING_PAYMENT)
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        // Soft lock bàn 5 phút
        table.applySoftLock(reservation.getReservationId(), softLockMinutes);
        tableInfoRepository.save(table);

        return toResponse(reservation);
    }

    // ────── 3.2.6 Payment Webhook ─────────────────────────────────────

    @Transactional
    public ReservationResponse confirmPayment(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() != ReservationStatus.PENDING_PAYMENT) {
            throw new BadRequestException("Đơn không ở trạng thái PENDING_PAYMENT.");
        }

        // Xác nhận → RESERVED
        reservation.setStatus(ReservationStatus.RESERVED);
        reservation = reservationRepository.save(reservation);

        // Lưu Reservation_Table_Mapping — tìm bàn đang bị soft-lock bởi reservation này
        final Long reservationId = reservation.getReservationId();
        List<TableInfo> lockedTables = tableInfoRepository.findAll().stream()
                .filter(t -> reservationId.equals(t.getLockedByReservationId()))
                .collect(Collectors.toList());

        final Reservation savedRes = reservation;
        for (TableInfo t : lockedTables) {
            t.setSoftLockUntil(null);
            t.setLockedByReservationId(null);
            t.setStatus(TableStatus.OCCUPIED);
            tableInfoRepository.save(t);
            mappingRepository.save(ReservationTableMapping.builder()
                    .reservation(savedRes)
                    .tableInfo(t)
                    .build());
        }
        return toResponse(reservation);
    }

    @Transactional
    public ReservationResponse cancelPayment(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() == ReservationStatus.PENDING_PAYMENT) {
            reservation.setStatus(ReservationStatus.CANCELLED);
        } else if (reservation.getStatus() == ReservationStatus.CREATED) {
            reservation.setStatus(ReservationStatus.EXPIRED);
        }
        reservationRepository.save(reservation);

        // Giải phóng soft lock
        releaseLockedTable(id);
        return toResponse(reservation);
    }

    // ────── Get / Cancel ──────────────────────────────────────────────

    public ReservationResponse getById(Long id) {
        Reservation r = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        return toResponse(r);
    }

    @Transactional
    public void cancelReservation(Long id, Long customerId) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (customerId != null && (reservation.getCustomer() == null ||
            !reservation.getCustomer().getCustomerId().equals(customerId))) {
            throw new BadRequestException("Bạn không có quyền hủy đơn này.");
        }
        if (reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException("Chỉ có thể hủy đơn đang ở trạng thái RESERVED.");
        }
        reservation.cancel();
        reservationRepository.save(reservation);
        // Giải phóng bàn
        releaseTablesByReservation(reservation);
    }

    // ────── Walk-in ────────────────────────────────────────────────────

    @Transactional
    public ReservationResponse createWalkIn(WalkInRequest req) {
        // Tạo walk-in customer tạm nếu chưa có
        Customer customer = customerRepository.findAll().stream()
                .filter(c -> c.getPhone().equals(req.getCustomerPhone()) && c.getPasswordHash() == null)
                .findFirst()
                .orElseGet(() -> customerRepository.save(Customer.builder()
                        .name(req.getCustomerName())
                        .phone(req.getCustomerPhone())
                        .isVerified(true) // walk-in không cần xác minh
                        .build()));

        // Chọn bàn
        List<TableInfo> selectedTables = new ArrayList<>();
        if (req.getTableId() != null) {
            TableInfo t = tableInfoRepository.findById(req.getTableId())
                    .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + req.getTableId() + " không tồn tại."));
            if (t.getStatus() != TableStatus.AVAILABLE || t.isSoftLocked()) {
                throw new ConflictException("Bàn #" + req.getTableId() + " hiện không khả dụng.");
            }
            selectedTables.add(t);
        } else if (req.isMergeTables()) {
            // Ghép bàn tự động
            List<TableInfo> available = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE);
            int total = 0;
            for (TableInfo t : available) {
                if (t.isSoftLocked()) continue;
                selectedTables.add(t);
                total += t.getCapacity();
                if (total >= req.getGuestCount()) break;
            }
            if (total < req.getGuestCount()) {
                throw new BadRequestException("Không đủ bàn để ghép. Cần " + req.getGuestCount() + " chỗ.");
            }
        } else {
            List<TableInfo> available = tableInfoRepository.findAvailableTablesForGuests(req.getGuestCount());
            if (available.isEmpty()) throw new BadRequestException("Không có bàn trống phù hợp.");
            selectedTables.add(available.get(0));
        }

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime end = now.plusMinutes(durationMinutes + bufferMinutes);

        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.WALK_IN)
                .guestCount(req.getGuestCount())
                .startTime(now)
                .endTime(end)
                .status(ReservationStatus.SEATED) // walk-in ngồi ngay
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        final Reservation savedReservation = reservation;
        for (TableInfo t : selectedTables) {
            t.setStatus(TableStatus.OCCUPIED);
            tableInfoRepository.save(t);
            mappingRepository.save(ReservationTableMapping.builder()
                    .reservation(savedReservation).tableInfo(t).build());
        }

        return toResponse(reservation);
    }

    // ────── Check-in / Check-out ───────────────────────────────────────

    @Transactional
    public ReservationResponse checkIn(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException("Đơn không ở trạng thái RESERVED.");
        }
        reservation.checkIn();
        return toResponse(reservationRepository.save(reservation));
    }

    @Transactional
    public ReservationResponse checkOut(Long id, int bufferMinutesAfterCheckout) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() != ReservationStatus.SEATED) {
            throw new BadRequestException("Đơn không ở trạng thái SEATED.");
        }
        reservation.checkOut();
        reservation.setEndTime(LocalDateTime.now().plusMinutes(bufferMinutesAfterCheckout));
        reservationRepository.save(reservation);
        return toResponse(reservation);
    }

    // ────── Active / Upcoming ─────────────────────────────────────────

    public List<ReservationResponse> getActiveReservations() {
        return reservationRepository.findByStatusOrderByStartTimeAsc(ReservationStatus.SEATED)
                .stream().map(this::toResponse).collect(Collectors.toList());
    }

    public List<ReservationResponse> getUpcomingReservations(int minutes) {
        LocalDateTime now = LocalDateTime.now();
        return reservationRepository.findUpcoming(now, now.plusMinutes(minutes))
                .stream().map(this::toResponse).collect(Collectors.toList());
    }

    // ────── Private helpers ────────────────────────────────────────────

    private void releaseLockedTable(Long reservationId) {
        tableInfoRepository.findAll().stream()
                .filter(t -> reservationId.equals(t.getLockedByReservationId()))
                .forEach(t -> {
                    t.releaseSoftLock();
                    tableInfoRepository.save(t);
                });
    }

    private void releaseTablesByReservation(Reservation reservation) {
        if (reservation.getTableMappings() != null) {
            reservation.getTableMappings().forEach(m -> {
                TableInfo t = m.getTableInfo();
                t.setStatus(TableStatus.AVAILABLE);
                tableInfoRepository.save(t);
            });
        }
    }
}

// Helper field to track locked table on PENDING_PAYMENT — add to Reservation entity via transient
// (we use a workaround via soft lock on TableInfo tracking lockedByReservationId)
