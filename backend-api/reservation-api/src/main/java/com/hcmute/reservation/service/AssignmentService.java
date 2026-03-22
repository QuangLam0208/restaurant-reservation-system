package com.hcmute.reservation.service;

import com.hcmute.reservation.model.Reservation;
import com.hcmute.reservation.model.ReservationTableMapping;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.ReservationTableMappingRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class AssignmentService {

    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;
    private final ReservationRepository reservationRepository;

    @Transactional
    public boolean findAlternativeTables(Reservation reservation) {
        // Tìm bàn trống có đủ capacity hoặc ghép bàn
        // Loại trừ các bàn overlap với khoảng thời gian của reservation
        LocalDateTime start = reservation.getStartTime();
        LocalDateTime end = reservation.getEndTime();
        Set<Long> occupiedIds = new HashSet<>(reservationRepository.findOccupiedTableIds(start, end));

        // Giải phóng mapping cũ (bàn ảo ban đầu)
        if (reservation.getTableMappings() != null) {
            for (ReservationTableMapping m : reservation.getTableMappings()) {
                mappingRepository.delete(m);
            }
            reservation.getTableMappings().clear();
        }

        // Ưu tiên bàn đơn
        List<TableInfo> availableSingle = tableInfoRepository.findAvailableTablesForGuests(reservation.getGuestCount())
                .stream()
                .filter(t -> !occupiedIds.contains(t.getTableId()))
                .collect(Collectors.toList());

        List<TableInfo> selectedTables = new ArrayList<>();
        if (!availableSingle.isEmpty()) {
            selectedTables.add(availableSingle.get(0));
        } else {
            // Thử ghép bàn
            List<TableInfo> availableMerge = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                    .stream()
                    .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                    .collect(Collectors.toList());
            int total = 0;
            for (TableInfo t : availableMerge) {
                selectedTables.add(t);
                total += t.getCapacity();
                if (total >= reservation.getGuestCount()) break;
            }
            if (total < reservation.getGuestCount()) {
                // Không tìm được bàn thay thế
                return false;
            }
        }

        // Tạo mapping mới
        for (TableInfo t : selectedTables) {
            ReservationTableMapping mapping = ReservationTableMapping.builder()
                    .reservation(reservation)
                    .tableInfo(t)
                    .build();
            mappingRepository.save(mapping);
            reservation.getTableMappings().add(mapping);
        }

        return true;
    }
}
