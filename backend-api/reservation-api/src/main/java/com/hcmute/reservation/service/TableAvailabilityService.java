package com.hcmute.reservation.service;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
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
public class TableAvailabilityService {

    private final TableInfoRepository tableInfoRepository;
    private final ReservationRepository reservationRepository;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    /**
     * Lấy danh sách toàn bộ các bàn đang thực sự trống trong khoảng thời gian của Reservation
     */
    public List<TableInfo> getFreeTables(Reservation reservation) {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime actualStart = now.isBefore(reservation.getStartTime()) ? now : reservation.getStartTime();
        LocalDateTime end = reservation.getEndTime().plusMinutes(bufferMinutes);

        Set<Long> occupiedIds = new HashSet<>(reservationRepository.findOccupiedTableIds(actualStart, end));

        // Lấy tất cả bàn đang ở trạng thái AVAILABLE, kích hoạt, không bị soft-lock và không bị occupied
        return tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                .stream()
                .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                .collect(Collectors.toList());
    }
}