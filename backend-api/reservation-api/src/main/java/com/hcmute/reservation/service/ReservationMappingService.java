package com.hcmute.reservation.service;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.ReservationTableMapping;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.repository.ReservationTableMappingRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Propagation;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;

@Service
@RequiredArgsConstructor
public class ReservationMappingService {

    private final ReservationTableMappingRepository mappingRepository;

    @Transactional(propagation = Propagation.MANDATORY) // Bắt buộc chạy trong Transaction của hàm gọi nó
    public void updateTableMappings(Reservation reservation, List<TableInfo> newTables) {
        // Xóa mapping cũ
        if (reservation.getTableMappings() != null && !reservation.getTableMappings().isEmpty()) {
            mappingRepository.deleteAll(reservation.getTableMappings());
            reservation.getTableMappings().clear();
        } else {
            reservation.setTableMappings(new ArrayList<>());
        }

        // Tạo mapping mới
        for (TableInfo t : newTables) {
            ReservationTableMapping mapping = ReservationTableMapping.builder()
                    .reservation(reservation)
                    .tableInfo(t)
                    .build();
            mappingRepository.save(mapping);
            reservation.getTableMappings().add(mapping);
        }
    }
}