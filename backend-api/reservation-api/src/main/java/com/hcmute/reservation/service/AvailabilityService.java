package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.table.AvailableWindowResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.*;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class AvailabilityService {

    private final TableInfoRepository tableInfoRepository;
    private final ReservationRepository reservationRepository;

    @Value("${reservation.duration-minutes:120}")
    private int durationMinutes;

    /**
     * GET /api/reservations/availability
     * Trả về danh sách bàn khả dụng và gợi ý giờ thay thế
     */
    public Map<String, Object> checkAvailability(LocalDate date, LocalTime time, int guests) {
        LocalDateTime requestedStart = LocalDateTime.of(date, time);
        if (requestedStart.isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Thời gian đặt bàn phải trong tương lai.");
        }
        LocalDateTime requestedEnd = requestedStart.plusMinutes(durationMinutes);

        // Tìm tableId đã bị đặt trong khoảng thời gian này
        List<Long> occupiedIds = reservationRepository.findOccupiedTableIds(requestedStart, requestedEnd);
        Set<Long> occupiedSet = new HashSet<>(occupiedIds);

        List<TableInfo> availableTables = tableInfoRepository.findAvailableTablesForGuests(guests)
                .stream()
                .filter(t -> !occupiedSet.contains(t.getTableId()))
                .collect(Collectors.toList());

        List<Map<String, Object>> tables = availableTables.stream().map(t -> {
            Map<String, Object> m = new LinkedHashMap<>();
            m.put("tableId", t.getTableId());
            m.put("capacity", t.getCapacity());
            m.put("status", t.getStatus());
            return m;
        }).collect(Collectors.toList());

        Map<String, Object> result = new LinkedHashMap<>();
        result.put("requestedTime", requestedStart);
        result.put("guestCount", guests);
        result.put("availableTables", tables);
        result.put("hasAvailability", !tables.isEmpty());

        if (tables.isEmpty()) {
            List<String> suggestions = List.of(
                requestedStart.plusMinutes(30).toString(),
                requestedStart.plusMinutes(60).toString()
            );
            result.put("alternativeTimes", suggestions);
        }
        return result;
    }

    /**
     * GET /api/tables/available-windows
     * Phân tích real-time cho WinForm POS
     */
    @Cacheable(value = "availableWindows", key = "#guests + '_' + #time")
    public List<AvailableWindowResponse> getAvailableWindows(int guests, LocalDateTime time) {
        List<TableInfo> allActiveTables = tableInfoRepository.findByIsActiveTrue();
        List<AvailableWindowResponse> result = new ArrayList<>();

        // Bàn đủ chỗ và hoàn toàn trống
        List<TableInfo> fullAvailable = allActiveTables.stream()
                .filter(t -> t.getCapacity() >= guests
                        && t.getStatus() == TableStatus.AVAILABLE
                        && !t.isSoftLocked())
                .collect(Collectors.toList());

        for (TableInfo t : fullAvailable) {
            result.add(AvailableWindowResponse.builder()
                    .tableId(t.getTableId())
                    .capacity(t.getCapacity())
                    .availability(AvailableWindowResponse.Availability.FULL_AVAILABLE)
                    .build());
        }

        // Bàn đang bị soft-lock (sắp trống)
        List<TableInfo> partialAvailable = allActiveTables.stream()
                .filter(t -> t.getCapacity() >= guests && t.isSoftLocked())
                .collect(Collectors.toList());

        for (TableInfo t : partialAvailable) {
            result.add(AvailableWindowResponse.builder()
                    .tableId(t.getTableId())
                    .capacity(t.getCapacity())
                    .availability(AvailableWindowResponse.Availability.PARTIAL_AVAILABLE)
                    .availableUntil(t.getSoftLockUntil())
                    .build());
        }

        // Ghép bàn: tìm tổ hợp 2 bàn đủ chỗ
        List<TableInfo> smallAvailable = allActiveTables.stream()
                .filter(t -> t.getStatus() == TableStatus.AVAILABLE && !t.isSoftLocked()
                        && t.getCapacity() < guests)
                .collect(Collectors.toList());

        List<Long> mergeCandidates = new ArrayList<>();
        int total = 0;
        for (TableInfo t : smallAvailable) {
            mergeCandidates.add(t.getTableId());
            total += t.getCapacity();
            if (total >= guests) break;
        }
        if (total >= guests && mergeCandidates.size() > 1) {
            // Thêm entry gợi ý ghép bàn
            result.add(AvailableWindowResponse.builder()
                    .tableId(null)
                    .capacity(total)
                    .availability(AvailableWindowResponse.Availability.FULL_AVAILABLE)
                    .mergeCandidateIds(mergeCandidates)
                    .build());
        }

        return result;
    }
}
