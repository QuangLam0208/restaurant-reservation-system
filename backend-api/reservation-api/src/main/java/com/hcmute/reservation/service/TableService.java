package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.table.FloorMapTableResponse;
import com.hcmute.reservation.dto.table.TableRequest;
import com.hcmute.reservation.dto.table.TableResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class TableService {

    private final TableInfoRepository tableInfoRepository;

    private TableResponse toResponse(TableInfo t) {
        return TableResponse.builder()
                .tableId(t.getTableId())
                .capacity(t.getCapacity())
                .status(t.getStatus())
                .isActive(t.getIsActive())
                .version(t.getVersion())
                .build();
    }

    public List<TableResponse> getAllTables() {
        return tableInfoRepository.findAll().stream()
                .map(this::toResponse).collect(Collectors.toList());
    }

    @Transactional
    public TableResponse createTable(TableRequest req) {
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
        TableInfo table = tableInfoRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + id + " không tồn tại."));

        if (req.getCapacity() == null || req.getCapacity() <= 0) {
            throw new BadRequestException("Sức chứa bàn phải lớn hơn 0.");
        }

        if (req.getVersion() != null && !req.getVersion().equals(table.getVersion())) {
            throw new BadRequestException("Dữ liệu bàn đã bị thay đổi bởi một giao dịch khác. Vui lòng tải lại.");
        }

        if (req.getCapacity() < table.getCapacity() && table.getMappings() != null) {
            boolean hasConflict = table.getMappings().stream()
                    .filter(m -> m != null && m.getReservation() != null)
                    .anyMatch(m -> {
                        ReservationStatus status = m.getReservation().getStatus();
                        boolean isActiveRes = status == ReservationStatus.CREATED
                                || status == ReservationStatus.PENDING_PAYMENT
                                || status == ReservationStatus.RESERVED
                                || status == ReservationStatus.SEATED;
                        return isActiveRes && m.getReservation().getGuestCount() > req.getCapacity();
                    });
            if (hasConflict) {
                throw new BadRequestException("Không thể giảm sức chứa do bàn đang có đơn đặt trước vượt quá số lượng này.");
            }
        }

        table.setCapacity(req.getCapacity());
        if (req.getIsActive() != null) table.setIsActive(req.getIsActive());

        if (req.getStatus() != null) {
            table.setStatus(req.getStatus());
        }

        return toResponse(tableInfoRepository.save(table));
    }

    @Transactional
    public void deleteTable(Long id) {
        TableInfo table = tableInfoRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + id + " không tồn tại."));

        boolean hasActiveReservation = table.getMappings() != null && table.getMappings().stream()
                .filter(m -> m != null && m.getReservation() != null)
                .anyMatch(m -> {
                    ReservationStatus status = m.getReservation().getStatus();
                    return status == ReservationStatus.CREATED
                            || status == ReservationStatus.PENDING_PAYMENT
                            || status == ReservationStatus.RESERVED
                            || status == ReservationStatus.SEATED;
                });

        if (hasActiveReservation) {
            throw new BadRequestException("Không thể xóa bàn đang được khóa tạm, có đơn đặt chỗ hoặc đang phục vụ.");
        }

        table.setIsActive(false);
        tableInfoRepository.save(table);
    }

    public List<FloorMapTableResponse> getFloorMap() {
        LocalDateTime now = LocalDateTime.now();

        return tableInfoRepository.findByIsActiveTrue().stream().map(t -> {
            String customerName = null;
            Long resId = null;
            TableStatus currentStatus = t.getStatus(); // Physical status: AVAILABLE, OCCUPIED, OVERSTAY
            ReservationStatus currentResStatus = null; // Trạng thái của đơn đặt bàn (nếu có)

            if (t.getMappings() != null) {
                var safeMappings = t.getMappings().stream()
                        .filter(m -> m != null && m.getReservation() != null)
                        .collect(Collectors.toList());

                // Ưu tiên 1: Đang ngồi
                var seatedMapping = safeMappings.stream()
                        .filter(m -> m.getReservation().getStatus() == ReservationStatus.SEATED)
                        .findFirst();

                // Ưu tiên 2: Đã đặt trước
                var reservedMapping = safeMappings.stream()
                        .filter(m -> m.getReservation().getStatus() == ReservationStatus.RESERVED)
                        .findFirst();

                var activeMapping = seatedMapping.isPresent() ? seatedMapping : reservedMapping;

                if (activeMapping.isPresent()) {
                    var res = activeMapping.get().getReservation();
                    resId = res.getReservationId();
                    currentResStatus = res.getStatus(); // Lấy trạng thái để trả về DTO

                    if (res.getCustomer() != null) {
                        customerName = res.getCustomer().getName();
                    }

                    // Chỉ can thiệp TableStatus khi khách ĐANG NGỒI (SEATED)
                    if (res.getStatus() == ReservationStatus.SEATED) {
                        if (res.getEndTime() != null && now.isAfter(res.getEndTime())) {
                            currentStatus = TableStatus.OVERSTAY;
                        } else {
                            currentStatus = TableStatus.OCCUPIED;
                        }
                    }
                    // Nếu res.getStatus() == RESERVED -> Không làm gì cả, currentStatus vẫn giữ nguyên (thường là AVAILABLE)
                }
            }

            return FloorMapTableResponse.builder()
                    .tableId(t.getTableId())
                    .capacity(t.getCapacity())
                    .status(currentStatus) // Chỉ có AVAILABLE, OCCUPIED, OVERSTAY
                    .isActive(t.getIsActive())
                    .currentReservationId(resId)
                    .currentReservationStatus(currentResStatus) // Frontend dùng trường này để render màu "Đặt trước"
                    .currentCustomerName(customerName)
                    .build();
        }).collect(Collectors.toList());
    }
}