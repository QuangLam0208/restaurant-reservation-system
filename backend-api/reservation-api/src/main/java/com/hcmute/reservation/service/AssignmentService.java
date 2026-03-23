package com.hcmute.reservation.service;

import com.hcmute.reservation.model.Reservation;
import com.hcmute.reservation.model.ReservationTableMapping;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.ReservationTableMappingRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
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

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Transactional
    public boolean findAlternativeTables(Reservation reservation) {
        LocalDateTime now = LocalDateTime.now();
        // Thời điểm bắt đầu cần kiểm tra là thời gian khách thực tế muốn vào ngồi (NOW)
        // Nếu khách đến sớm, phải đảm bảo bàn trống từ NOW đến lúc kết thúc.
        LocalDateTime actualStart = now.isBefore(reservation.getStartTime()) ? now : reservation.getStartTime();
        LocalDateTime end = reservation.getEndTime();
        Set<Long> occupiedIds = new HashSet<>(reservationRepository.findOccupiedTableIds(actualStart, end.plusMinutes(bufferMinutes)));

        // ĐẦU TIÊN PHẢI TÌM XEM CÓ BÀN THAY KHÔNG
        List<TableInfo> selectedTables = new ArrayList<>();
        // Ưu tiên bàn đơn
        List<TableInfo> availableSingle = tableInfoRepository.findAvailableTablesForGuests(reservation.getGuestCount())
                .stream()
                // Thêm !t.isSoftLocked() để không cướp bàn của khách online đang thanh toán
                .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                .collect(Collectors.toList());

        if (!availableSingle.isEmpty()) {
            selectedTables.add(availableSingle.get(0));
        } else {
            // Thử ghép bàn
            List<TableInfo> availableMerge = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                    .stream()
                    .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                    .sorted((t1, t2) -> Integer.compare(t2.getCapacity(), t1.getCapacity()))
                    .collect(Collectors.toList());


            int total = 0;
            for (TableInfo t : availableMerge) {
                selectedTables.add(t);
                total += t.getCapacity();
                if (total >= reservation.getGuestCount()) break;
            }
            // Nếu không đủ bàn ghép, THẤT BẠI VÀ RETURN NGAY, bảo toàn 100% Mapping cũ
            if (total < reservation.getGuestCount()) {
                return false;
            }
        }

        // TÌM THẤY BÀN -> XÓA MAPPING CŨ & LƯU MAPPING MỚI
        if (reservation.getTableMappings() != null && !reservation.getTableMappings().isEmpty()) {
            mappingRepository.deleteAll(reservation.getTableMappings());
            reservation.getTableMappings().clear();
        } else {
            reservation.setTableMappings(new ArrayList<>());
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
