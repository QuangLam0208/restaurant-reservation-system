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
        table.setCapacity(req.getCapacity());
        if (req.getIsActive() != null) table.setIsActive(req.getIsActive());
        return toResponse(tableInfoRepository.save(table));
    }

    @Transactional
    public void deleteTable(Long id) {
        TableInfo table = tableInfoRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + id + " không tồn tại."));
        // Chỉ xóa khi không có đơn đang active
        boolean hasActiveReservation = table.getMappings() != null && table.getMappings().stream()
                .anyMatch(m -> m.getReservation().getStatus() == ReservationStatus.SEATED
                           || m.getReservation().getStatus() == ReservationStatus.RESERVED);
        if (hasActiveReservation) {
            throw new BadRequestException("Không thể xóa bàn đang có đơn đặt chỗ hoặc đang phục vụ.");
        }
        tableInfoRepository.delete(table);
    }

    public List<FloorMapTableResponse> getFloorMap() {
        return tableInfoRepository.findByIsActiveTrue().stream().map(t -> {
            // Tìm đơn SEATED đang liên kết với bàn này
            String customerName = null;
            Long resId = null;
            if (t.getMappings() != null) {
                var activeMapping = t.getMappings().stream()
                        .filter(m -> m.getReservation().getStatus() == ReservationStatus.SEATED)
                        .findFirst();
                if (activeMapping.isPresent()) {
                    resId = activeMapping.get().getReservation().getReservationId();
                    if (activeMapping.get().getReservation().getCustomer() != null) {
                        customerName = activeMapping.get().getReservation().getCustomer().getName();
                    }
                }
            }
            return FloorMapTableResponse.builder()
                    .tableId(t.getTableId())
                    .capacity(t.getCapacity())
                    .status(t.getStatus())
                    .isActive(t.getIsActive())
                    .currentReservationId(resId)
                    .currentCustomerName(customerName)
                    .build();
        }).collect(Collectors.toList());
    }
}
