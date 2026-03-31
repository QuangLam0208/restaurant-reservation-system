package com.hcmute.reservation.service;

import com.hcmute.reservation.event.ReservationConfirmedEvent;
import com.hcmute.reservation.model.dto.reservation.*;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.entity.Customer;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.ReservationTableMapping;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.ReservationType;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.*;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.orm.ObjectOptimisticLockingFailureException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Duration;
import java.time.LocalDateTime;
import java.time.LocalTime;
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
    private final ApplicationEventPublisher eventPublisher;

    @Value("${reservation.grace-period-minutes:15}")
    private int gracePeriodMinutes;

    @Value("${reservation.duration-minutes:120}")
    private int durationMinutes;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Value("${reservation.soft-lock-minutes:5}")
    private int softLockMinutes;

    @Value("${reservation.max-capacity-overflow}")
    private int maxCapacityOverflow;

    @Value("${restaurant.opening-time:10:00}")
    private String openingTimeStr;

    @Value("${restaurant.closing-time:22:30}")
    private String closingTimeStr;

    @Value("${reservation.max-merge-tables:4}")
    private int maxMergeTables;

    @Value("${reservation.deposit-per-guest:50000}")
    private Double depositPerGuest;

    // ────── Helpers ──────────────────────────────────────────────────

    private ReservationResponse toResponse(Reservation r) {
        List<Long> tableIds = r.getTableMappings() == null ? List.of()
                : r.getTableMappings().stream()
                        .map(m -> m.getTableInfo().getTableId())
                        .collect(Collectors.toList());
        return ReservationResponse.builder()
                .reservationId(r.getReservationId())
                .status(r.getStatus())
                .type(r.getType())
                .guestCount(r.getGuestCount())
                .startTime(r.getStartTime())
                .endTime(r.getEndTime())
                .depositAmount(r.getDepositAmount())
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
        int[] bestDiff = { Integer.MAX_VALUE };
        int[] minTables = { Integer.MAX_VALUE };

        backtrack(sortedTables, targetGuests, 0, new ArrayList<>(), 0, bestCombination, bestDiff, minTables);

        return bestCombination;
    }

    private void backtrack(List<TableInfo> tables, int target, int start,
            List<TableInfo> currentCombo, int currentSum,
            List<TableInfo> bestCombo, int[] bestDiff, int[] minTables) {
        if (currentSum >= target) {
            int diff = currentSum - target;
            if (diff <= maxCapacityOverflow) {
                if (diff < bestDiff[0] || (diff == bestDiff[0] && currentCombo.size() < minTables[0])) {
                    bestDiff[0] = diff;
                    minTables[0] = currentCombo.size();
                    bestCombo.clear();
                    bestCombo.addAll(currentCombo);
                }
            }
            return;
        }

        // Tối đa cho phép ghép 4 bàn
        if (currentCombo.size() > maxMergeTables)
            return;

        for (int i = start; i < tables.size(); i++) {
            currentCombo.add(tables.get(i));
            backtrack(tables, target, i + 1, currentCombo, currentSum + tables.get(i).getCapacity(), bestCombo,
                    bestDiff, minTables);
            currentCombo.remove(currentCombo.size() - 1);
        }
    }

    // ────── 3.2.2 Online booking ──────────────────────────────────────

    @Transactional
    public ReservationResponse createOnlineReservation(OnlineReservationRequest req, Long customerId) {
        Customer customer = customerRepository.findById(customerId)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng."));

        LocalDateTime start = req.getStartTime();

        LocalTime openingTime = LocalTime.parse(openingTimeStr);
        LocalTime closingTime = LocalTime.parse(closingTimeStr);
        LocalDateTime closingDateTime = LocalDateTime.of(start.toLocalDate(), closingTime);

        if (start.isBefore(LocalDateTime.now().plusHours(1))) {
            throw new BadRequestException("Vui lòng đặt bàn trước ít nhất 1 tiếng.");
        }
        if (start.toLocalTime().isBefore(openingTime) || !start.isBefore(closingDateTime)) {
            throw new BadRequestException("Giờ đến nằm ngoài thời gian hoạt động của nhà hàng (" + openingTimeStr
                    + " - " + closingTimeStr + ").");
        }

        // Kiểm tra Soft seating >= 60 phút
        long minutesUntilClose = Duration.between(start, closingDateTime).toMinutes();
        if (minutesUntilClose < 60) {
            throw new BadRequestException("Thời gian dùng bữa tối thiểu là 60 phút. Nhà hàng đóng cửa lúc "
                    + closingTimeStr + ", vui lòng chọn giờ đến sớm hơn.");
        }

        LocalDateTime defaultEnd = start.plusMinutes(durationMinutes);
        LocalDateTime end = defaultEnd.isAfter(closingDateTime) ? closingDateTime : defaultEnd;

        List<Reservation> overlappingReservations = reservationRepository
                .findOverlappingByCustomerId(customerId, start, end);

        if (overlappingReservations.size() >= 2) {
            throw new BadRequestException("Bạn chỉ được phép đặt tối đa 2 đơn trong cùng một khung giờ. " +
                    "Vui lòng chọn khung giờ khác hoặc hoàn tất/hủy các đơn hiện tại.");
        }

        LocalDateTime blockUntil = end.plusMinutes(bufferMinutes);

        // Lọc bàn theo overlap
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
                throw new BadRequestException("Hiện không có tổ hợp bàn trống phù hợp để ghép cho "
                        + req.getGuestCount() + " người. Vui lòng chọn giờ khác.");
            }
        }

        // Tạo Reservation với status = CREATED
        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.ONLINE)
                .guestCount(req.getGuestCount())
                .startTime(start)
                .endTime(end)
                .depositAmount(req.getGuestCount() * depositPerGuest)
                .status(ReservationStatus.CREATED)
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        List<ReservationTableMapping> mappings = new ArrayList<>();
        try {
            for (TableInfo t : selectedTables) {
                t.applySoftLock(reservation.getReservationId(), softLockMinutes);
                tableInfoRepository.saveAndFlush(t);

                ReservationTableMapping mapping = ReservationTableMapping.builder()
                        .reservation(reservation)
                        .tableInfo(t)
                        .build();
                mappings.add(mappingRepository.save(mapping));
            }
        } catch (org.springframework.orm.ObjectOptimisticLockingFailureException e) {
            throw new ConflictException(
                    "Rất tiếc, bàn bạn chọn vừa được khách hàng khác đặt thành công. Vui lòng chọn lại khung giờ hoặc bàn khác!");
        }

        // Gắn mapping vào reservation
        reservation.setTableMappings(mappings);

        // Chuyển sang status = PENDING_PAYMENT
        reservation.setStatus(ReservationStatus.PENDING_PAYMENT);
        reservation = reservationRepository.save(reservation);

        return toResponse(reservation);
    }

    // ────── 3.2.6 Payment Webhook ─────────────────────────────────────

    @Transactional
    public ReservationResponse confirmPayment(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Reservation not found"));
        if (reservation.getStatus() != ReservationStatus.PENDING_PAYMENT) {
            throw new BadRequestException(
                    "Only PENDING_PAYMENT reservation can be confirmed. Current status: " + reservation.getStatus());
        }

        // Lấy danh sách bàn đang được soft-lock
        final Long reservationId = reservation.getReservationId();
        List<TableInfo> lockedTables = tableInfoRepository.findByLockedByReservationId(reservationId);

        // Xử lý Race Condition với Scheduler
        if (lockedTables.isEmpty()) {
            throw new ConflictException(
                    "Giao dịch thanh toán mất quá nhiều thời gian. Thời gian giữ bàn (5 phút) đã hết và bàn đã bị giải phóng. Vui lòng liên hệ nhà hàng để được hỗ trợ hoàn tiền hoặc xếp bàn mới.");
        }

        // Đánh dấu RESERVED (Hibernate tự động dirty check và update vào DB cuối
        // Transaction)
        reservation.setStatus(ReservationStatus.RESERVED);

        for (TableInfo t : lockedTables) {
            t.setSoftLockUntil(null);
            t.setLockedByReservationId(null);
            t.setStatus(TableStatus.AVAILABLE);
            tableInfoRepository.save(t);
        }

        // Chuẩn bị dữ liệu gửi Email (Tránh lazy loading trong Async thread sau này)
        String customerEmail = reservation.getCustomer() != null ? reservation.getCustomer().getEmail() : null;
        String customerName = reservation.getCustomer() != null ? reservation.getCustomer().getName() : "Guest";

        // Bắn sự kiện (Event) ra ngoài theo format mới
        eventPublisher.publishEvent(new ReservationConfirmedEvent(
                this,
                customerEmail,
                customerName,
                reservation.getReservationId(),
                reservation.getStartTime()));

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

    @Transactional(readOnly = true)
    public List<ReservationResponse> getReservationsByCustomer(Long customerId) {
        return reservationRepository.findByCustomer_CustomerIdOrderByStartTimeDesc(customerId)
                .stream()
                .map(this::toResponse)
                .collect(Collectors.toList());
    }

    // ────── Get / Cancel ──────────────────────────────────────────────

    @Transactional(readOnly = true)
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
            throw new BadRequestException(
                    "Khách đã nhận bàn (SEATED). Không thể hủy đơn. Nếu khách muốn rời đi, vui lòng sử dụng chức năng Check-out (Trả bàn).");
        }

        if (reservation.getStatus() == ReservationStatus.NO_SHOW ||
                reservation.getStatus() == ReservationStatus.CANCELLED) {
            throw new BadRequestException("Đơn đặt bàn này đã được xử lý (Hủy hoặc Vắng mặt) từ trước.");
        }

        if (reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException("Chỉ có thể hủy đơn đang chờ khách đến (Trạng thái RESERVED).");
        }

        // Kiểm tra thời gian: Chỉ khách hàng mới bị giới hạn 3 tiếng
        if (customerId != null) {
            if (reservation.getStartTime().isBefore(LocalDateTime.now().plusHours(3))) {
                throw new BadRequestException(
                        "Bạn chỉ có thể hủy đơn đặt bàn tối thiểu 3 tiếng trước giờ hẹn. Vui lòng liên hệ nhà hàng để được hỗ trợ.");
            }
        }

        reservation.cancel();
        reservationRepository.save(reservation);
        // Giải phóng bàn (cho đơn đã REVERVED có table mapping)
        releaseTablesByReservation(reservation);
    }

    // ────── Walk-in ────────────────────────────────────────────────────

    @Transactional(readOnly = true)
    public WalkInOptionResponse getWalkInOptions(int guestCount) {
        if (guestCount <= 0) {
            throw new BadRequestException("So luong khach phai lon hon 0.");
        }

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime blockUntil = now.plusMinutes(durationMinutes + bufferMinutes);

        List<TableInfo> candidateTables = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE);
        Set<Long> occupiedTableIds = new HashSet<>(reservationRepository.findOccupiedTableIds(now, blockUntil));
        List<TableInfo> cleanTables = candidateTables.stream()
                .filter(table -> !occupiedTableIds.contains(table.getTableId()))
                .collect(Collectors.toList());
        List<TableInfo> partialTables = candidateTables.stream()
                .filter(table -> occupiedTableIds.contains(table.getTableId()))
                .collect(Collectors.toList());

        List<WalkInOptionResponse.TableOption> preferredOptions = new ArrayList<>();

        cleanTables.stream()
                .filter(table -> table.getCapacity() >= guestCount)
                .filter(table -> table.getCapacity() <= guestCount + maxCapacityOverflow)
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).thenComparing(TableInfo::getTableId))
                .limit(5)
                .forEach(table -> preferredOptions.add(WalkInOptionResponse.TableOption.builder()
                        .tableIds(List.of(table.getTableId()))
                        .totalCapacity(table.getCapacity())
                        .type("FULL_AVAILABLE")
                        .availableUntil(null)
                        .build()));

        List<List<TableInfo>> cleanCombinations = findWalkInOptionCombinations(cleanTables, guestCount);
        cleanCombinations.stream()
                .filter(combo -> combo.size() > 1)
                .sorted(Comparator.<List<TableInfo>>comparingInt(List::size)
                        .thenComparingInt(combo -> combo.stream().mapToInt(TableInfo::getCapacity).sum()))
                .limit(5)
                .forEach(combo -> preferredOptions.add(buildWalkInOption(combo, "MERGE_AVAILABLE", null)));

        if (!preferredOptions.isEmpty()) {
            return WalkInOptionResponse.builder()
                    .groups(List.of(WalkInOptionResponse.OptionGroup.builder()
                            .groupName("Ưu tiên (Trống hoàn toàn)")
                            .options(preferredOptions)
                            .build()))
                    .build();
        }

        Map<Long, LocalDateTime> partialAvailableUntilByTableId = new HashMap<>();
        List<Long> partialTableIds = partialTables.stream().map(TableInfo::getTableId).collect(Collectors.toList());
        if (!partialTableIds.isEmpty()) {
            List<Object[]> nextBookings = reservationRepository.findNextBookingForTables(partialTableIds, now);
            for (Object[] row : nextBookings) {
                Long tableId = (Long) row[0];
                LocalDateTime nextStart = (LocalDateTime) row[1];
                partialAvailableUntilByTableId.put(tableId, nextStart);
            }
        }

        List<WalkInOptionResponse.TableOption> fallbackOptions = new ArrayList<>();
        partialTables.stream()
                .filter(table -> table.getCapacity() >= guestCount)
                .filter(table -> table.getCapacity() <= guestCount + maxCapacityOverflow)
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).thenComparing(TableInfo::getTableId))
                .limit(5)
                .forEach(table -> {
                    LocalDateTime availableUntil = partialAvailableUntilByTableId.get(table.getTableId());
                    if (availableUntil != null) {
                        fallbackOptions.add(WalkInOptionResponse.TableOption.builder()
                                .tableIds(List.of(table.getTableId()))
                                .totalCapacity(table.getCapacity())
                                .type("PARTIAL_AVAILABLE")
                                .availableUntil(availableUntil)
                                .build());
                    }
                });
        Set<Long> partialTableIdSet = partialTables.stream().map(TableInfo::getTableId).collect(Collectors.toSet());
        List<TableInfo> cleanAndPartial = new ArrayList<>(cleanTables);
        cleanAndPartial.addAll(partialTables);

        List<List<TableInfo>> mixedCombinations = findWalkInOptionCombinations(cleanAndPartial, guestCount);
        mixedCombinations.stream()
                .filter(combo -> combo.size() > 1)
                .filter(combo -> combo.stream().map(TableInfo::getTableId).anyMatch(partialTableIdSet::contains))
                .sorted(Comparator.<List<TableInfo>>comparingInt(List::size)
                        .thenComparingInt(combo -> combo.stream().mapToInt(TableInfo::getCapacity).sum()))
                .limit(5)
                .forEach(combo -> {
                    LocalDateTime availableUntil = combo.stream()
                            .map(TableInfo::getTableId)
                            .filter(partialTableIdSet::contains)
                            .map(partialAvailableUntilByTableId::get)
                            .filter(Objects::nonNull)
                            .min(LocalDateTime::compareTo)
                            .orElse(null);
                    if (availableUntil != null) {
                        fallbackOptions.add(buildWalkInOption(combo, "PARTIAL_MERGED_AVAILABLE", availableUntil));
                    }
                });

        return WalkInOptionResponse.builder()
                .groups(List.of(WalkInOptionResponse.OptionGroup.builder()
                        .groupName("Dự phòng (Vướng lịch sau)")
                        .options(fallbackOptions)
                        .build()))
                .build();
    }

    // ────── Suggest — Gợi ý bàn + Soft Lock ──────────────────

    @Transactional
    public WalkInSuggestionResponse suggestWalkIn(WalkInRequest req) {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime defaultEnd = now.plusMinutes(durationMinutes);
        LocalDateTime checkEnd = req.getEndTime() != null ? req.getEndTime() : defaultEnd;
        LocalDateTime blockUntil = checkEnd.plusMinutes(bufferMinutes);

        // ── Chọn bàn theo 3 case ──────────────────────────────────────

        List<TableInfo> selectedTables = new ArrayList<>();
        String availabilityType = "FULL_AVAILABLE";
        Set<Long> occupiedIds = new HashSet<>(
                reservationRepository.findOccupiedTableIds(now, blockUntil));

        if (req.getTableId() != null && !req.getTableId().isEmpty()) {
            // Case A: Lễ tân chỉ định bàn cụ thể
            for (Long tableId : req.getTableId()) {
                TableInfo t = tableInfoRepository.findById(tableId)
                        .orElseThrow(() -> new ResourceNotFoundException("Ban #" + tableId + " khong ton tai."));
                if (t.getStatus() != TableStatus.AVAILABLE || t.isSoftLocked()) {
                    throw new ConflictException("Ban #" + tableId + " hien khong kha dung.");
                }
                selectedTables.add(t);
            }

            // Kiểm tra xem có CÓ BẤT KỲ bàn nào trong danh sách dính lịch tương lai không?
            boolean hasPartialTable = selectedTables.stream()
                    .anyMatch(t -> occupiedIds.contains(t.getTableId()));

            // Xác định availabilityType
            if (selectedTables.size() > 1) {
                availabilityType = hasPartialTable ? "PARTIAL_MERGED_AVAILABLE" : "MERGED_AVAILABLE";
                ;
            } else {
                availabilityType = hasPartialTable ? "PARTIAL_AVAILABLE" : "FULL_AVAILABLE";
            }

        } else if (req.isMergeTables()) {
            // Case B: Ghép bàn tự động
            // Lấy danh sách bàn trống, chỉ lấy các bàn nhỏ hơn guestCount để ghép
            List<TableInfo> availableMerge = tableInfoRepository
                    .findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                    .stream()
                    .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                    .filter(t -> t.getCapacity() < req.getGuestCount())
                    .collect(Collectors.toList());

            selectedTables = findBestTableCombination(availableMerge, req.getGuestCount());

            if (selectedTables.isEmpty()) {
                throw new BadRequestException("Không có tổ hợp bàn ghép nào phù hợp (yêu cầu sức chứa từ "
                        + req.getGuestCount() + " đến " + (req.getGuestCount() + 2) + " chỗ).");
            }
            availabilityType = "MERGED_AVAILABLE";
        } else {
            // Case C: Hệ thống tự chọn — FULL ưu tiên, fallback merge, sau đó PARTIAL
            // 1. Tìm bàn đơn FULL_AVAILABLE (Giới hạn sức chứa +2)
            List<TableInfo> fullAvailable = tableInfoRepository
                    .findAvailableTablesForGuests(req.getGuestCount())
                    .stream()
                    .filter(t -> !occupiedIds.contains(t.getTableId()))
                    .filter(t -> t.getCapacity() <= req.getGuestCount() + 2)
                    .collect(Collectors.toList());

            if (!fullAvailable.isEmpty()) {
                selectedTables.add(fullAvailable.get(0));
                availabilityType = "FULL_AVAILABLE";
            } else {
                // 2. Thử ghép bàn (nếu không có bàn đơn thỏa mãn)
                List<TableInfo> allFree = tableInfoRepository
                        .findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                        .stream()
                        .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                        .filter(t -> t.getCapacity() < req.getGuestCount())
                        .collect(Collectors.toList());

                List<TableInfo> merged = findBestTableCombination(allFree, req.getGuestCount());

                if (!merged.isEmpty()) {
                    selectedTables.addAll(merged);
                    availabilityType = "MERGED_AVAILABLE";
                } else {
                    // 3. Fallback: PARTIAL AVAILABLE (Bàn trống tạm thời, vướng lịch đặt sau)
                    List<TableInfo> partialAvailable = tableInfoRepository
                            .findAvailableTablesForGuests(req.getGuestCount())
                            .stream()
                            .filter(t -> t.getCapacity() <= req.getGuestCount() + 2)
                            .collect(Collectors.toList());
                    if (partialAvailable.isEmpty()) {
                        throw new BadRequestException("Nha hang hien da het ban trong phu hop.");
                    }
                    selectedTables.add(partialAvailable.get(0));
                    availabilityType = "PARTIAL_AVAILABLE";
                }
            }
        }

        // ── Tính endTime chính xác (kiểm tra booking kế tiếp) ─────────

        LocalDateTime calculatedEndTime = defaultEnd;
        if (req.getEndTime() != null) {
            calculatedEndTime = req.getEndTime();
        } else {
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
            if (earliestNextBooking != null) {
                LocalDateTime maxAllowed = earliestNextBooking.minusMinutes(bufferMinutes);
                if (maxAllowed.isBefore(calculatedEndTime)) {
                    calculatedEndTime = maxAllowed;
                }
            }
        }

        // ── Resolve customer (tùy chọn với walk-in) ───────────────────
        Customer customer = resolveWalkInCustomer(req);

        // ── Tạo Reservation tạm (CREATED) + soft-lock các bàn ─────────
        // Scheduler sẽ tự hủy nếu lễ tân không confirm trong softLockMinutes.
        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.WALK_IN)
                .guestCount(req.getGuestCount())
                .startTime(now)
                .endTime(calculatedEndTime)
                .status(ReservationStatus.CREATED)
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        List<ReservationTableMapping> mappings = new ArrayList<>();
        try {
            for (TableInfo t : selectedTables) {
                t.applySoftLock(reservation.getReservationId(), softLockMinutes);
                tableInfoRepository.saveAndFlush(t);

                ReservationTableMapping mapping = ReservationTableMapping.builder()
                        .reservation(reservation)
                        .tableInfo(t)
                        .build();
                mappings.add(mappingRepository.save(mapping));
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            // Bàn vừa bị lễ tân khác cướp trong tích tắc — hủy reservation tạm
            reservation.setStatus(ReservationStatus.CANCELLED);
            reservationRepository.save(reservation);
            throw new ConflictException(
                    "Mot trong cac ban goi y vua duoc xep cho khach khac. Vui long thu lai.");
        }

        reservation.setTableMappings(mappings);

        // ── Build response ─────────────────────────────────────────────

        final String finalAvailabilityType = availabilityType;
        List<WalkInSuggestionResponse.SuggestedTable> suggestedTableDtos = selectedTables.stream()
                .map(t -> WalkInSuggestionResponse.SuggestedTable.builder()
                        .tableId(t.getTableId())
                        .capacity(t.getCapacity())
                        .availabilityType(finalAvailabilityType)
                        .build())
                .collect(Collectors.toList());

        return WalkInSuggestionResponse.builder()
                .suggestionId(reservation.getReservationId())
                .suggestedTables(suggestedTableDtos)
                .lockExpiresAt(LocalDateTime.now().plusMinutes(softLockMinutes))
                .guestCount(req.getGuestCount())
                .startTime(reservation.getStartTime())
                .endTime(reservation.getEndTime())
                .availabilityType(availabilityType)
                .build();
    }

    // ────── Confirm — Xác nhận xếp bàn ─────────────────────
    @Transactional
    public ReservationResponse confirmWalkIn(Long suggestionId) {
        Reservation reservation = reservationRepository.findById(suggestionId)
                .orElseThrow(() -> new ResourceNotFoundException(
                        "Goi y #" + suggestionId + " khong ton tai hoac da het han."));

        // Guard: chỉ cho confirm đúng loại và đúng trạng thái
        if (reservation.getType() != ReservationType.WALK_IN) {
            throw new BadRequestException("Chi co the xac nhan goi y Walk-in qua endpoint nay.");
        }
        if (reservation.getStatus() == ReservationStatus.EXPIRED ||
                reservation.getStatus() == ReservationStatus.CANCELLED) {
            throw new BadRequestException(
                    "Goi y ban da het han hoac bi huy. Vui long tim ban lai.");
        }
        if (reservation.getStatus() != ReservationStatus.CREATED) {
            throw new BadRequestException(
                    "Trang thai khong hop le de xac nhan: " + reservation.getStatus());
        }

        // Lấy bàn đang soft-lock
        List<TableInfo> lockedTables = tableInfoRepository.findByLockedByReservationId(suggestionId);

        if (lockedTables.isEmpty()) {
            // Scheduler đã giải phóng soft-lock trước khi lễ tân bấm confirm
            reservation.setStatus(ReservationStatus.EXPIRED);
            reservationRepository.save(reservation);
            throw new ConflictException(
                    "Thoi gian giu ban da het (" + softLockMinutes + " phut). Vui long tim ban lai.");
        }

        // CREATED -> SEATED (walk-in không cần qua RESERVED)
        reservation.setStatus(ReservationStatus.SEATED);
        reservation = reservationRepository.save(reservation);

        try {
            for (TableInfo t : lockedTables) {
                t.setSoftLockUntil(null);
                t.setLockedByReservationId(null);
                t.setStatus(TableStatus.OCCUPIED);
                tableInfoRepository.saveAndFlush(t);
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            throw new ConflictException(
                    "Ban vua bi thay doi boi giao dich khac. Vui long tai lai va thu lai.");
        }

        // Reload để toResponse có đủ tableMappings
        return toResponse(reservationRepository.findById(reservation.getReservationId())
                .orElse(reservation));
    }

    // ────── Cancel suggestion — Hủy gợi ý ──────────────────

    @Transactional
    public void cancelWalkInSuggestion(Long suggestionId) {
        Reservation reservation = reservationRepository.findById(suggestionId)
                .orElseThrow(() -> new ResourceNotFoundException(
                        "Goi y #" + suggestionId + " khong ton tai."));

        if (reservation.getType() != ReservationType.WALK_IN) {
            throw new BadRequestException("Chi co the huy goi y Walk-in qua endpoint nay.");
        }

        if (reservation.getStatus() == ReservationStatus.CREATED) {
            reservation.setStatus(ReservationStatus.CANCELLED);
            reservationRepository.save(reservation);
            releaseLockedTable(suggestionId);
        }
        // CANCELLED / EXPIRED: idempotent - bỏ qua yên lặng
    }

    // ────── Change Table ────────────────────────────────────────────────────
    @Transactional
    public ReservationResponse changeTable(Long reservationId, ChangeTableRequest req) {
        // Lấy reservation
        Reservation reservation = reservationRepository.findById(reservationId)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn #" + reservationId + " không tồn tại."));

        // Chỉ cho phép change khi RESERVED hoặc SEATED
        if (reservation.getStatus() != ReservationStatus.SEATED
                && reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException(
                    "Chỉ có thể đổi bàn khi đơn ở trạng thái RESERVED hoặc SEATED. " +
                            "Trạng thái hiện tại: " + reservation.getStatus());
        }

        // Tổng sức chứa của bàn cũ
        int oldTotalCapacity = reservation.getTableMappings().stream()
                .mapToInt(m -> m.getTableInfo().getCapacity())
                .sum();

        int newTotalCapacity = 0;

        // Lấy ID bàn cũ của đơn này để loại trừ khi kiểm tra conflict
        Set<Long> currentTableIds = reservation.getTableMappings().stream()
                .map(m -> m.getTableInfo().getTableId())
                .collect(Collectors.toSet());

        // Validate new list table
        List<TableInfo> newTables = new ArrayList<>();
        for (Long tableId : req.getTableIds()) {
            TableInfo table = tableInfoRepository.findById(tableId)
                    .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + tableId + " không tồn tại."));

            if (!table.getIsActive()) {
                throw new BadRequestException("Bàn #" + tableId + " đang bị vô hiệu hóa.");
            }

            // Tính sức chứa của bàn mới
            newTotalCapacity += table.getCapacity();

            // Nếu bàn mới không phải bàn cũ → kiểm tra trạng thái
            if (!currentTableIds.contains(tableId)) {
                if (table.isSoftLocked()) {
                    throw new ConflictException(
                            "Bàn #" + tableId + " đang được giữ tạm cho giao dịch thanh toán khác.");
                }
                if (table.getStatus() != TableStatus.AVAILABLE) {
                    throw new ConflictException(
                            "Bàn #" + tableId + " hiện không trống (Trạng thái: " + table.getStatus() + ").");
                }

                // Kiểm tra overlap với booking khác (trừ chính đơn này)
                LocalDateTime start = reservation.getStartTime();
                LocalDateTime end = reservation.getEndTime().plusMinutes(bufferMinutes);
                List<Long> overlappingReservationTableIds = reservationRepository.findOccupiedTableIds(start, end);
                // Loại bỏ chính các bàn của đơn này khỏi danh sách "đang bận"
                overlappingReservationTableIds.removeAll(currentTableIds);

                if (overlappingReservationTableIds.contains(tableId)) {
                    throw new ConflictException("Bàn #" + tableId + " đã có lịch đặt trùng trong khung giờ này.");
                }
            }
            newTables.add(table);
        }

        if (newTotalCapacity > reservation.getGuestCount() + 2) {
            throw new BadRequestException(
                    "Không thể đổi bàn! Chỉ được phép chuyển sang bàn lớn hơn tối đa 2 chỗ so với sức chứa hiện tại. (Sức chứa cũ: "
                            + oldTotalCapacity + " chỗ, yêu cầu mới: " + newTotalCapacity + " chỗ).");
        }

        // Giải phóng bàn cũ
        if (reservation.getTableMappings() != null) {
            for (ReservationTableMapping mapping : reservation.getTableMappings()) {
                TableInfo oldTable = mapping.getTableInfo();
                // Chỉ giải phóng nếu bàn cũ KHÔNG nằm trong danh sách bàn mới
                boolean isKeptTable = req.getTableIds().contains(oldTable.getTableId());
                if (!isKeptTable) {
                    oldTable.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.save(oldTable);
                }
            }
            mappingRepository.deleteAll(reservation.getTableMappings());
            reservation.getTableMappings().clear();
        }

        // Gán bàn mới và tạo mapping
        List<ReservationTableMapping> newMappings = new ArrayList<>(); // Tạo 1 list hứng mapping mới
        try {
            for (TableInfo table : newTables) {
                // Chỉ set OCCUPIED nếu khách đang ngồi thực tế
                if (reservation.getStatus() == ReservationStatus.SEATED) {
                    table.setStatus(TableStatus.OCCUPIED);
                }
                // Nếu RESERVED thì giữ nguyên AVAILABLE — bàn chỉ bị "chiếm" khi check-in
                tableInfoRepository.saveAndFlush(table);

                ReservationTableMapping mapping = ReservationTableMapping.builder()
                        .reservation(reservation)
                        .tableInfo(table)
                        .build();

                newMappings.add(mappingRepository.save(mapping));
            }
        } catch (org.springframework.orm.ObjectOptimisticLockingFailureException e) {
            throw new ConflictException(
                    "Một trong các bàn vừa bị thay đổi bởi giao dịch khác. Vui lòng tải lại và thử lại.");
        }

        reservation.setTableMappings(newMappings);

        return toResponse(reservationRepository.save(reservation));
    }

    // ────── Check-in / Check-out ───────────────────────────────────────

    @Transactional
    public ReservationResponse checkIn(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));

        if (reservation.getStatus() != ReservationStatus.RESERVED) {
            throw new BadRequestException(
                    "Đơn không ở trạng thái RESERVED. (Lưu ý: Đơn Walk-in hoặc đơn khách đã vào bàn sẽ không thể check-in lại).");
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

            throw new BadRequestException("Đơn đặt bàn đã quá giờ giữ chỗ (" + gracePeriodMinutes
                    + " phút). Đơn đã bị hủy theo chính sách No-Show.");
        }

        // ────── Kịch bản B & C: Kiểm tra trạng thái thực tế của bàn ──────
        boolean isOriginalTablesAvailable = assignedTables.stream()
                .allMatch(t -> t.getStatus() == TableStatus.AVAILABLE && !t.isSoftLocked());

        if (!isOriginalTablesAvailable) {
            // Cố gắng nhờ AssignmentService tìm và gán tạm bàn thay thế
            boolean hasAlternativeTable = assignmentService.findAlternativeTables(reservation);

            if (!hasAlternativeTable) {
                if (now.isBefore(startTime)) {
                    throw new ConflictException(
                            "Bàn đặt trước hiện chưa trống và không có bàn thay thế phù hợp. Mời quý khách ngồi chờ ở khu vực Waitlist.");
                } else {
                    throw new ConflictException(
                            "OVERSTAY_CONFLICT: Bàn gốc đang bị khách ca trước ngồi quá giờ và không có bàn thay thế trống. Vui lòng sử dụng chức năng Override để xử lý.");
                }
            } else {
                reservationRepository.flush();
                // Nếu tìm được bàn thay thế thành công, load lại danh sách bàn mới từ Mapping
                assignedTables = new ArrayList<>(reservation.getTableMappings().stream()
                        .map(ReservationTableMapping::getTableInfo)
                        .toList());
            }
        }

        // ────── Check-in Thành công ──────

        reservation.checkIn();
        if (now.isBefore(startTime)) {
            reservation.setEndTime(now.plusMinutes(durationMinutes));
        } else {
            reservation.setEndTime(startTime.plusMinutes(durationMinutes));
        }
        // Cập nhật Table_Info.status = OCCUPIED cho TẤT CẢ bàn trong Mapping
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

    @Transactional(readOnly = true)
    public List<ReservationResponse> getActiveReservations() {
        return reservationRepository.findByStatusOrderByStartTimeAsc(ReservationStatus.SEATED)
                .stream().map(this::toResponse).collect(Collectors.toList());
    }

    @Transactional(readOnly = true)
    public List<ReservationResponse> getUpcomingReservations(int minutes) {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime startWindow = now.minusMinutes(15);
        LocalDateTime endWindow = now.plusMinutes(minutes);
        return reservationRepository.findUpcoming(startWindow, endWindow)
                .stream().map(this::toResponse).collect(Collectors.toList());
    }

    // ────── Private helpers ────────────────────────────────────────────

    private List<List<TableInfo>> findWalkInOptionCombinations(List<TableInfo> availableTables, int targetGuests) {
        List<TableInfo> sortedTables = availableTables.stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).reversed()
                        .thenComparing(TableInfo::getTableId))
                .collect(Collectors.toList());

        List<List<TableInfo>> combinations = new ArrayList<>();
        backtrackWalkInOptionCombinations(sortedTables, targetGuests, 0, new ArrayList<>(), 0, combinations);
        return combinations;
    }

    private void backtrackWalkInOptionCombinations(List<TableInfo> tables, int target, int start,
            List<TableInfo> currentCombo, int currentSum,
            List<List<TableInfo>> combinations) {
        if (combinations.size() >= 20) {
            return;
        }
        if (currentSum >= target) {
            int diff = currentSum - target;
            if (diff <= maxCapacityOverflow) {
                combinations.add(new ArrayList<>(currentCombo));
            }
            return;
        }

        if (currentCombo.size() >= 4) {
            return;
        }

        for (int i = start; i < tables.size(); i++) {
            currentCombo.add(tables.get(i));
            backtrackWalkInOptionCombinations(tables, target, i + 1, currentCombo,
                    currentSum + tables.get(i).getCapacity(), combinations);
            currentCombo.remove(currentCombo.size() - 1);
        }
    }

    private WalkInOptionResponse.TableOption buildWalkInOption(List<TableInfo> tables,
            String type,
            LocalDateTime availableUntil) {
        List<Long> tableIds = tables.stream()
                .map(TableInfo::getTableId)
                .sorted()
                .collect(Collectors.toList());
        int totalCapacity = tables.stream()
                .mapToInt(TableInfo::getCapacity)
                .sum();

        return WalkInOptionResponse.TableOption.builder()
                .tableIds(tableIds)
                .totalCapacity(totalCapacity)
                .type(type)
                .availableUntil(availableUntil)
                .build();
    }

    private Customer resolveWalkInCustomer(WalkInRequest req) {
        if (req.getCustomerPhone() == null || req.getCustomerPhone().isBlank()) {
            return null;
        }
        return customerRepository.findByPhoneAndPasswordHashIsNull(req.getCustomerPhone())
                .orElseGet(() -> customerRepository.save(Customer.builder()
                        .name(req.getCustomerName() != null && !req.getCustomerName().isBlank()
                                ? req.getCustomerName()
                                : "Khach Walk-in")
                        .phone(req.getCustomerPhone())
                        .isVerified(true)
                        .build()));
    }

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
