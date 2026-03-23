package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.table.FloorMapTableResponse;
import com.hcmute.reservation.dto.table.TableRequest;
import com.hcmute.reservation.dto.table.TableResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class TableService {

    private final TableInfoRepository tableInfoRepository;

    private void rejectManualStatusChange(TableRequest req) {
        if (req.getStatus() != null) {
            throw new BadRequestException("Trạng thái bàn được hệ thống quản lý tự động, không thể cập nhật thủ công qua API này.");
        }
    }

    private boolean isBlockingReservationStatus(ReservationStatus status) {
        return status == ReservationStatus.CREATED
                || status == ReservationStatus.PENDING_PAYMENT
                || status == ReservationStatus.RESERVED
                || status == ReservationStatus.SEATED;
    }

    private TableResponse toResponse(TableInfo t) {
        return TableResponse.builder()
                .tableId(t.getTableId())
                .capacity(t.getCapacity())
                .status(t.getStatus())
                .isActive(t.getIsActive())
                .version(t.getVersion())
                .build();
    }

    @Transactional(readOnly = true)
    public List<TableResponse> getAllTables() {
        return tableInfoRepository.findAll().stream()
                .map(this::toResponse).collect(Collectors.toList());
    }

    @Transactional
    public TableResponse createTable(TableRequest req) {
        rejectManualStatusChange(req);
        if (req.getCapacity() == null || req.getCapacity() <= 0) {
            throw new BadRequestException("Sức chứa bàn phải lớn hơn 0.");
        }

        TableInfo table = TableInfo.builder()
                .capacity(req.getCapacity())
                .isActive(req.getIsActive() != null ? req.getIsActive() : true)
                .status(TableStatus.AVAILABLE)
                .build();
        return toResponse(tableInfoRepository.save(table));
    }

    @Transactional
    public TableResponse updateTable(Long id, TableRequest req) {
        rejectManualStatusChange(req);
        if (req.getVersion() == null) {
            throw new BadRequestException("Thiếu version của bàn. Vui lòng tải lại dữ liệu trước khi cập nhật.");
        }
        TableInfo table = tableInfoRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + id + " không tồn tại."));

        if (req.getCapacity() == null || req.getCapacity() <= 0) {
            throw new BadRequestException("Sức chứa bàn phải lớn hơn 0.");
        }

        //  So sánh chuẩn kiểu intValue() để tránh lỗi Type Mismatch
        if (req.getVersion().intValue() != table.getVersion()) {
            throw new ConflictException("Dữ liệu bàn đã bị thay đổi bởi một giao dịch khác. Vui lòng tải lại.");
        }
        if (req.getCapacity() < table.getCapacity() && table.isSoftLocked()) {
            throw new BadRequestException("Không thể giảm sức chứa khi bàn đang được giữ tạm cho một giao dịch thanh toán.");
        }
        if (req.getCapacity() < table.getCapacity() && table.getMappings() != null) {
            boolean hasConflict = table.getMappings().stream()
                    .filter(m -> m != null && m.getReservation() != null)
                    .anyMatch(m -> {
                        ReservationStatus status = m.getReservation().getStatus();
                        boolean isActiveRes = isBlockingReservationStatus(status);

                        if (!isActiveRes) return false;

                        int currentTotalCapacity = m.getReservation().getTableMappings().stream()
                                .mapToInt(mapping -> mapping.getTableInfo().getCapacity())
                                .sum();
                        int newTotalCapacity = currentTotalCapacity - table.getCapacity() + req.getCapacity();

                        return m.getReservation().getGuestCount() > newTotalCapacity;
                    });
            if (hasConflict) {
                throw new BadRequestException("Không thể giảm sức chứa do bàn đang có đơn đặt trước vượt quá tổng số lượng này.");
            }
        }

        // Fix vô hiệu hóa bàn đang bận
        if (req.getIsActive() != null && !req.getIsActive() && table.getIsActive()) {
            boolean hasActiveReservation = table.getMappings() != null && table.getMappings().stream()
                    .filter(m -> m != null && m.getReservation() != null)
                    .anyMatch(m -> isBlockingReservationStatus(m.getReservation().getStatus()));

            if (hasActiveReservation || table.isSoftLocked()) {
                throw new BadRequestException("Không thể vô hiệu hóa bàn đang được khóa tạm, có đơn đặt chỗ hoặc đang phục vụ.");
            }
        }

        table.setCapacity(req.getCapacity());
        if (req.getIsActive() != null) table.setIsActive(req.getIsActive());

        return toResponse(tableInfoRepository.save(table));
    }

    @Transactional
    public void deleteTable(Long id) {
        TableInfo table = tableInfoRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + id + " không tồn tại."));

        // Chặn ngay nếu bàn đang Soft Locked
        if (table.isSoftLocked()) {
            throw new BadRequestException("Không thể xóa bàn đang được khóa tạm chờ khách thanh toán.");
        }

        boolean hasActiveReservation = table.getMappings() != null && table.getMappings().stream()
                .filter(m -> m != null && m.getReservation() != null)
                .anyMatch(m -> isBlockingReservationStatus(m.getReservation().getStatus()));

        if (hasActiveReservation || table.getStatus() != TableStatus.AVAILABLE) {
            throw new BadRequestException("Không thể xóa bàn đang có đơn đặt chỗ hoặc không ở trạng thái trống.");
        }

        table.setIsActive(false);
        tableInfoRepository.save(table);
    }

    //Thêm @Transactional(readOnly = true) để tránh LazyInitializationException
    @Transactional(readOnly = true)
    public List<FloorMapTableResponse> getFloorMap() {
        LocalDateTime now = LocalDateTime.now();

        return tableInfoRepository.findByIsActiveTrue().stream().map(t -> {
            String customerName = null;
            Long resId = null;
            TableStatus currentStatus = t.getStatus();
            ReservationStatus currentResStatus = null;

            if (t.getMappings() != null && !t.getMappings().isEmpty()) {
                var safeMappings = t.getMappings().stream()
                        .filter(m -> m != null && m.getReservation() != null)
                        .collect(Collectors.toList());

                var seatedMapping = safeMappings.stream()
                        .filter(m -> m.getReservation().getStatus() == ReservationStatus.SEATED)
                        .findFirst();

                // Sắp xếp theo startTime tăng dần để luôn lấy đơn sớm nhất
                var reservedMapping = safeMappings.stream()
                        .filter(m -> m.getReservation().getStatus() == ReservationStatus.RESERVED)
                        .min(Comparator.comparing(m -> m.getReservation().getStartTime()));

                var activeMapping = seatedMapping.isPresent() ? seatedMapping : reservedMapping;

                if (activeMapping.isPresent()) {
                    var res = activeMapping.get().getReservation();
                    resId = res.getReservationId();
                    currentResStatus = res.getStatus();

                    if (res.getCustomer() != null) {
                        customerName = res.getCustomer().getName();
                    }
                    if (res.getStatus() == ReservationStatus.SEATED) {
                        if (res.getEndTime() != null && now.isAfter(res.getEndTime())) {
                            currentStatus = TableStatus.OVERSTAY;
                        } else {
                            currentStatus = TableStatus.OCCUPIED;
                        }
                    }
                }
            }

            return FloorMapTableResponse.builder()
                    .tableId(t.getTableId())
                    .capacity(t.getCapacity())
                    .status(currentStatus)
                    .isActive(t.getIsActive())
                    .currentReservationId(resId)
                    .currentReservationStatus(currentResStatus)
                    .currentCustomerName(customerName)
                    .build();
        }).collect(Collectors.toList());
    }
}
