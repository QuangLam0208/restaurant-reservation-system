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
import java.util.*;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class ReservationService {

    private final ReservationRepository reservationRepository;
    private final CustomerRepository customerRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;
    private final AssignmentService assignmentService;

    @Value("${reservation.grace-period-minutes:15}")
    private int gracePeriodMinutes;

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

    // ────── Thuật toán ghép bàn thông minh ────────────────────────────
    private List<TableInfo> findBestTableCombination(List<TableInfo> availableTables, int targetGuests) {
        // Ép sắp xếp giảm dần (Ví dụ: 8, 4, 4) để chộp bàn to trước
        List<TableInfo> sortedTables = availableTables.stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).reversed())
                .collect(Collectors.toList());

        List<TableInfo> bestCombination = new ArrayList<>();
        int[] bestDiff = {Integer.MAX_VALUE};
        int[] minTables = {Integer.MAX_VALUE};

        backtrack(sortedTables, targetGuests, 0, new ArrayList<>(), 0, bestCombination, bestDiff, minTables);

        return bestCombination;
    }

    private void backtrack(List<TableInfo> tables, int target, int start,
                           List<TableInfo> currentCombo, int currentSum,
                           List<TableInfo> bestCombo, int[] bestDiff, int[] minTables) {
        if (currentSum >= target) {
            int diff = currentSum - target;
            if (diff < bestDiff[0] || (diff == bestDiff[0] && currentCombo.size() < minTables[0])) {
                bestDiff[0] = diff;
                minTables[0] = currentCombo.size();
                bestCombo.clear();
                bestCombo.addAll(currentCombo);
            }
            return;
        }

        // Tối đa cho phép ghép 4 bàn
        if (currentCombo.size() > 4) return;

        for (int i = start; i < tables.size(); i++) {
            currentCombo.add(tables.get(i));
            backtrack(tables, target, i + 1, currentCombo, currentSum + tables.get(i).getCapacity(), bestCombo, bestDiff, minTables);
            currentCombo.remove(currentCombo.size() - 1);
        }
    }

    // ────── 3.2.2 Online booking ──────────────────────────────────────

    @Transactional
    public ReservationResponse createOnlineReservation(OnlineReservationRequest req, Long customerId) {
        Customer customer = customerRepository.findById(customerId)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng."));

        LocalDateTime start = req.getStartTime();
        LocalDateTime end = start.plusMinutes(durationMinutes);

        // Lọc bàn theo overlap
        LocalDateTime blockUntil = end.plusMinutes(bufferMinutes);
        Set<Long> occupiedTableIds = new HashSet<>(reservationRepository.findOccupiedTableIds(start, blockUntil));
        // Tìm bàn đơn
        List<TableInfo> available = tableInfoRepository.findAvailableTablesForGuests(req.getGuestCount())
                .stream()
                .filter(t -> !occupiedTableIds.contains(t.getTableId()))
                .collect(Collectors.toList());

        List<TableInfo> selectedTables = new ArrayList<>();
        if (!available.isEmpty()) {
            selectedTables.add(available.get(0));
        } else {
            // Không đủ bàn tĩnh -> Tìm ghép bàn
            List<TableInfo> allAvailable = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                    .stream()
                    .filter(t -> !t.isSoftLocked() && !occupiedTableIds.contains(t.getTableId()))
                    .collect(Collectors.toList());

            for (TableInfo t : allAvailable) {
                System.out.print(t.getCapacity() + " ");
            }

            // Gọi "Bộ não" ghép bàn
            selectedTables = findBestTableCombination(allAvailable, req.getGuestCount());

            if (selectedTables.isEmpty()) {
                throw new BadRequestException("Hiện không có tổ hợp bàn trống phù hợp để ghép cho " + req.getGuestCount() + " người. Vui lòng chọn giờ khác.");
            }
        }

        // Tạo Reservation với status = CREATED
        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.ONLINE)
                .guestCount(req.getGuestCount())
                .startTime(start)
                .endTime(end)
                .status(ReservationStatus.CREATED)
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        try {
            for (TableInfo t : selectedTables) {
                t.applySoftLock(reservation.getReservationId(), softLockMinutes);
                tableInfoRepository.saveAndFlush(t);
            }
        } catch (org.springframework.orm.ObjectOptimisticLockingFailureException e) {
            throw new ConflictException("Rất tiếc, bàn bạn chọn vừa được khách hàng khác đặt thành công. Vui lòng chọn lại khung giờ hoặc bàn khác!");
        }

        // Chuyển sang status = PENDING_PAYMENT
        reservation.setStatus(ReservationStatus.PENDING_PAYMENT);
        reservation = reservationRepository.save(reservation);

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

        // Lấy danh sách bàn đang được soft-lock
        final Long reservationId = reservation.getReservationId();
        List<TableInfo> lockedTables = tableInfoRepository.findByLockedByReservationId(reservationId);

        // Xử lý Race Condition với Scheduler
        if (lockedTables.isEmpty()) {
            // Nếu Scheduler đã chạy và giải phóng bàn trước khi payment webhook trả về,
            // ta tuyệt đối không được chuyển status sang RESERVED (vì sẽ tạo ra đơn rác không có bàn).
            // Ném lỗi 409 Conflict để FE/Gateway biết đường xử lý (ví dụ: kích hoạt API refund).
            throw new ConflictException("Giao dịch thanh toán mất quá nhiều thời gian. Thời gian giữ bàn (5 phút) đã hết và bàn đã bị giải phóng. Vui lòng liên hệ nhà hàng để được hỗ trợ hoàn tiền hoặc xếp bàn mới.");
        }

        // Xác nhận → RESERVED
        reservation.setStatus(ReservationStatus.RESERVED);
        reservation = reservationRepository.save(reservation);

        for (TableInfo t : lockedTables) {
            t.setSoftLockUntil(null);
            t.setLockedByReservationId(null);
            t.setStatus(TableStatus.AVAILABLE);
            tableInfoRepository.save(t);

            mappingRepository.save(ReservationTableMapping.builder()
                    .reservation(reservation)
                    .tableInfo(t)
                    .build());
        }

        // Logic gửi email sẽ ở đây
        // ...
        return toResponse(reservation);
    }

    @Transactional
    public ReservationResponse cancelPayment(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));

        // EXPIRED chỉ dành riêng cho background job (hết giờ tự hủy).
        if (reservation.getStatus() == ReservationStatus.PENDING_PAYMENT ||
                reservation.getStatus() == ReservationStatus.CREATED) {
            reservation.setStatus(ReservationStatus.CANCELLED);
        } else {
            throw new BadRequestException("Không thể hủy đơn đặt bàn ở trạng thái hiện tại.");
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
        if (reservation.getStatus() == ReservationStatus.PENDING_PAYMENT) {
            reservation.setStatus(ReservationStatus.CANCELLED);
            reservationRepository.save(reservation);
            releaseLockedTable(reservation.getReservationId());
            return;
        }

        if (reservation.getStatus() == ReservationStatus.SEATED) {
            throw new BadRequestException("Khách đã nhận bàn (SEATED). Không thể hủy đơn. Nếu khách muốn rời đi, vui lòng sử dụng chức năng Check-out (Trả bàn).");
        }

        if (reservation.getStatus() == ReservationStatus.NO_SHOW ||
                reservation.getStatus() == ReservationStatus.CANCELLED) {
            throw new BadRequestException("Đơn đặt bàn này đã được xử lý (Hủy hoặc Vắng mặt) từ trước.");
        }

        if (reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException("Chỉ có thể hủy đơn đang chờ khách đến (Trạng thái RESERVED).");
        }

        reservation.cancel();
        reservationRepository.save(reservation);
        // Giải phóng bàn (cho đơn đã REVERVED có table mapping)
        releaseTablesByReservation(reservation);
    }

    // ────── Walk-in ────────────────────────────────────────────────────

    @Transactional
    public ReservationResponse createWalkIn(WalkInRequest req) {
        Customer customer = null;
        if (req.getCustomerPhone() != null && !req.getCustomerPhone().trim().isEmpty()) {
            customer = customerRepository.findByPhoneAndPasswordHashIsNull(req.getCustomerPhone())
                    .orElseGet(() -> customerRepository.save(Customer.builder()
                            .name(req.getCustomerName() != null && !req.getCustomerName().trim().isEmpty()
                                    ? req.getCustomerName() : "Khách Walk-in")
                            .phone(req.getCustomerPhone())
                            .isVerified(true) // walk-in không cần xác minh
                            .build()));
        }

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime defaultEnd = now.plusMinutes(durationMinutes);
        LocalDateTime checkEnd = req.getEndTime() != null ? req.getEndTime() : defaultEnd;

        // Chọn bàn và xly short seating
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
            LocalDateTime blockUntil = checkEnd.plusMinutes(bufferMinutes);
            Set<Long> occupiedIds = new HashSet<>(reservationRepository.findOccupiedTableIds(LocalDateTime.now(), blockUntil));

            // Lọc bàn đủ điều kiện ghép (không bị soft lock, không overlap, và sức chứa < guestCount)
            List<TableInfo> availableMerge = available.stream()
                    .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                    .filter(t -> t.getCapacity() < req.getGuestCount())
                    .sorted((t1, t2) -> Integer.compare(t2.getCapacity(), t1.getCapacity())) // Sort DESC
                    .collect(Collectors.toList());

            int total = 0;
            for (TableInfo t : availableMerge) {
                selectedTables.add(t);
                total += t.getCapacity();
                if (total >= req.getGuestCount()) break;
            }
            if (total < req.getGuestCount()) {
                throw new BadRequestException("Không đủ bàn để ghép. Cần " + req.getGuestCount() + " chỗ.");
            }
        } else {
            LocalDateTime blockUntil = checkEnd.plusMinutes(bufferMinutes);
            Set<Long> occupiedIds = new HashSet<>(reservationRepository.findOccupiedTableIds(now, blockUntil));
            // Ưu tiên 1: Tìm bàn FULL AVAILABLE (Trống hoàn toàn trong khung giờ dự kiến)
            List<TableInfo> fullAvailable = tableInfoRepository.findAvailableTablesForGuests(req.getGuestCount())
                    .stream()
                    .filter(t -> !occupiedIds.contains(t.getTableId()))
                    .collect(Collectors.toList());
            if (!fullAvailable.isEmpty()) {
                selectedTables.add(fullAvailable.get(0));
            } else {
                // Ưu tiên 2: Tìm bàn PARTIAL AVAILABLE (Hiện tại trống nhưng sắp có khách online)
                List<TableInfo> partialAvailable = tableInfoRepository.findAvailableTablesForGuests(req.getGuestCount());
                if (partialAvailable.isEmpty()) {
                    throw new BadRequestException("Nhà hàng hiện đã hết bàn trống phù hợp.");
                }
                selectedTables.add(partialAvailable.get(0));
            }
        }

        LocalDateTime calculatedEndTime = defaultEnd;

        if (req.getEndTime() != null) {
            calculatedEndTime = req.getEndTime();
        } else {
            // Backend tự rà soát nếu Frontend không truyền
            LocalDateTime earliestNextBooking = null;
            for (TableInfo t : selectedTables) {
                List<Reservation> nextBookings = reservationRepository.findNextBookingForTable(t.getTableId(), now);
                if (!nextBookings.isEmpty()) {
                    LocalDateTime nextStart = nextBookings.get(0).getStartTime();
                    if (earliestNextBooking == null || nextStart.isBefore(earliestNextBooking)) {
                        earliestNextBooking = nextStart;
                    }
                }
            }
            // Nếu có booking kế tiếp cắt ngang (PARTIAL AVAILABLE), cập nhật endTime thành thời gian booking đó bắt đầu
            if (earliestNextBooking != null) {
                LocalDateTime maxAllowedEndTime = earliestNextBooking.minusMinutes(bufferMinutes);
                if (maxAllowedEndTime.isBefore(calculatedEndTime)) {
                    calculatedEndTime = maxAllowedEndTime;
                }
            }
        }
        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.WALK_IN)
                .guestCount(req.getGuestCount())
                .startTime(now)
                .endTime(calculatedEndTime)
                .status(ReservationStatus.SEATED)
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        try {
            for (TableInfo t : selectedTables) {
                t.setStatus(TableStatus.OCCUPIED);

                // Ép Hibernate update DB ngay để kiểm tra @Version
                tableInfoRepository.saveAndFlush(t);

                mappingRepository.save(ReservationTableMapping.builder()
                        .reservation(reservation).tableInfo(t).build());
            }
        } catch (org.springframework.orm.ObjectOptimisticLockingFailureException e) {
            // Bắt lỗi Race Condition: 2 Lễ tân cùng bấm xếp 1 bàn trên 2 máy POS khác nhau
            throw new ConflictException("Bàn bạn chọn vừa được Lễ tân khác xếp cho khách. Vui lòng chọn lại bàn hoặc tải lại sơ đồ phòng.");
        }

        return toResponse(reservation);
    }

    // ────── Check-in / Check-out ───────────────────────────────────────

    @Transactional
    public ReservationResponse checkIn(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));

        if (reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException(
                    "Đơn không ở trạng thái RESERVED. (Lưu ý: Đơn Walk-in hoặc đơn khách đã vào bàn sẽ không thể check-in lại)."
            );
        }

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime startTime = reservation.getStartTime();

        // Lấy danh sách bàn đang được mapping với đơn đặt bàn này
        List<TableInfo> assignedTables = reservation.getTableMappings().stream()
                .map(ReservationTableMapping::getTableInfo)
                .toList();

        // ────── Kịch bản A: Quá grace period (No-Show) ──────
        if (now.isAfter(startTime.plusMinutes(gracePeriodMinutes))) {
            reservation.markNoShow();
            reservationRepository.save(reservation);
            releaseTablesByReservation(reservation);

            throw new BadRequestException("Đơn đặt bàn đã quá giờ giữ chỗ (" + gracePeriodMinutes + " phút). Đơn đã bị hủy theo chính sách No-Show.");
        }

        // ────── Kịch bản B & C: Kiểm tra trạng thái thực tế của bàn ──────
        boolean isOriginalTablesAvailable = assignedTables.stream()
                .allMatch(t -> t.getStatus() == TableStatus.AVAILABLE && !t.isSoftLocked());

        if (!isOriginalTablesAvailable) {
            // Cố gắng nhờ AssignmentService tìm và gán tạm bàn thay thế
            boolean hasAlternativeTable = assignmentService.findAlternativeTables(reservation);

            if (!hasAlternativeTable) {
                if (now.isBefore(startTime)) {
                    throw new ConflictException("Bàn đặt trước hiện chưa trống và không có bàn thay thế phù hợp. Mời quý khách ngồi chờ ở khu vực Waitlist.");
                } else {
                    throw new ConflictException("OVERSTAY_CONFLICT: Bàn gốc đang bị khách ca trước ngồi quá giờ và không có bàn thay thế trống. Vui lòng sử dụng chức năng Override để xử lý.");
                }
            } else {
                reservationRepository.flush();
                // Nếu tìm được bàn thay thế thành công, load lại danh sách bàn mới từ Mapping
                assignedTables = new ArrayList<>(reservation.getTableMappings().stream()
                        .map(ReservationTableMapping::getTableInfo)
                        .toList());
            }
        }

        // ────── Check-in Thành công  ──────

        reservation.checkIn();
        if (now.isBefore(startTime)) {
            reservation.setEndTime(now.plusMinutes(durationMinutes));
        } else {
            reservation.setEndTime(startTime.plusMinutes(durationMinutes));
        }
        //Cập nhật Table_Info.status = OCCUPIED cho TẤT CẢ bàn trong Mapping
        for (TableInfo t : assignedTables) {
            t.setStatus(TableStatus.OCCUPIED);
            tableInfoRepository.save(t);
        }
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
        // Đặc tả 3.4.4: endTime = lúc checkout thực tế. Scheduler sẽ lo vụ buffer sau.
        reservation.setEndTime(LocalDateTime.now());
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
        tableInfoRepository.findByLockedByReservationId(reservationId).forEach(t -> {
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
