package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.TableAvailabilityService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class TableAvailabilityServiceImpl implements TableAvailabilityService {

    private final TableInfoRepository tableInfoRepository;
    private final ReservationRepository reservationRepository;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Override
    public List<TableInfo> getFreeTables(Reservation reservation) {
        // 1. Chỉ xử lý logic tính toán thời gian đặc thù của Reservation
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime actualStart = now.isBefore(reservation.getStartTime()) ? now : reservation.getStartTime();
        LocalDateTime end = reservation.getEndTime();

        // 2. Tái sử dụng lại logic truy vấn DB ở hàm bên dưới
        return getFreeTables(actualStart, end);
    }

    @Override
    public List<TableInfo> getFreeTables(LocalDateTime startTime, LocalDateTime endTime) {
        // Cộng thêm buffer time cho mọi truy vấn tìm bàn trống
        LocalDateTime endWithBuffer = endTime.plusMinutes(bufferMinutes);

        Set<Long> occupiedIds = new HashSet<>(reservationRepository.findOccupiedTableIds(startTime, endWithBuffer));

        // Lấy tất cả bàn đang ở trạng thái AVAILABLE, kích hoạt, không bị soft-lock và không bị occupied
        return tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                .stream()
                .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                .collect(Collectors.toList());
    }

    @Override
    public List<TableInfo> getCurrentlyAvailableTables() {
        return tableInfoRepository.findByIsActiveTrue().stream()
                .filter(table -> table.getStatus() == TableStatus.AVAILABLE && !table.isSoftLocked())
                .collect(Collectors.toList());
    }

    @Override
    public LocalDateTime getNextBookingTime(Long tableId, LocalDateTime fromTime) {
        List<Reservation> nextBookings = reservationRepository.findNextBookingForTable(tableId, fromTime);
        return nextBookings.isEmpty() ? null : nextBookings.get(0).getStartTime();
    }
}
