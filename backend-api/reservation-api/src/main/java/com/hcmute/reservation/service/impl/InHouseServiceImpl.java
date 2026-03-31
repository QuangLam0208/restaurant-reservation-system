package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.mapper.ReservationMapper;
import com.hcmute.reservation.model.dto.reservation.ChangeTableRequest;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.ReservationTableMapping;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.ReservationTableMappingRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.AssignmentService;
import com.hcmute.reservation.service.InHouseService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

import static com.hcmute.reservation.model.enums.ReservationStatus.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class InHouseServiceImpl implements InHouseService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;
    private final AssignmentService assignmentService;
    private final ReservationMapper mapper;

    @Value("${reservation.grace-period-minutes:15}")
    private int gracePeriodMinutes;

    @Value("${reservation.duration-minutes:120}")
    private int durationMinutes;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Override
    @Transactional
    public ReservationResponse checkIn(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));

        if (reservation.getStatus() != RESERVED) {
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
        return mapper.toResponse(reservationRepository.save(reservation));
    }

    @Override
    @Transactional
    public ReservationResponse checkOut(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() != SEATED) {
            throw new BadRequestException("Đơn không ở trạng thái SEATED.");
        }
        reservation.checkOut();
        // endTime = lúc checkout thực tế. Scheduler sẽ lo vụ buffer sau.
        reservation.setEndTime(LocalDateTime.now());
        reservationRepository.save(reservation);
        return mapper.toResponse(reservation);
    }

    @Override
    @Transactional
    public ReservationResponse changeTable(Long reservationId, ChangeTableRequest req) {
        Reservation reservation = reservationRepository.findById(reservationId).orElseThrow(() -> new ResourceNotFoundException("Đơn #" + reservationId + " không tồn tại."));

        if (reservation.getStatus() != SEATED && reservation.getStatus() != RESERVED) {
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
                if (reservation.getStatus() == SEATED) {
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

        return mapper.toResponse(reservationRepository.save(reservation));
    }

    @Override
    @Transactional
    public void cancelReservation(Long id, Long customerId) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (customerId != null && (reservation.getCustomer() == null ||
                !reservation.getCustomer().getCustomerId().equals(customerId))) {
            throw new BadRequestException("Bạn không có quyền hủy đơn này.");
        }
        if (reservation.getStatus() == PENDING_PAYMENT) {
            reservation.setStatus(CANCELLED);
            reservationRepository.save(reservation);
            releaseLockedTable(reservation.getReservationId());
            return;
        }

        if (reservation.getStatus() == SEATED) {
            throw new BadRequestException(
                    "Khách đã nhận bàn (SEATED). Không thể hủy đơn. Nếu khách muốn rời đi, vui lòng sử dụng chức năng Check-out (Trả bàn).");
        }

        if (reservation.getStatus() == NO_SHOW ||
                reservation.getStatus() == CANCELLED) {
            throw new BadRequestException("Đơn đặt bàn này đã được xử lý (Hủy hoặc Vắng mặt) từ trước.");
        }

        if (reservation.getStatus() != RESERVED) {
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

    private void releaseTablesByReservation(Reservation reservation) {
        if (reservation.getTableMappings() != null) {
            reservation.getTableMappings().forEach(m -> {
                TableInfo t = m.getTableInfo();
                t.setStatus(TableStatus.AVAILABLE);
                tableInfoRepository.save(t);
            });
        }
    }

    private void releaseLockedTable(Long reservationId) {
        tableInfoRepository.findByLockedByReservationId(reservationId).forEach(t -> {
            t.releaseSoftLock();
            tableInfoRepository.save(t);
        });
    }
}
