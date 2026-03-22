package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.table.AvailableWindowResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.model.Reservation;
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

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    /**
     * GET /api/reservations/availability
     * Trả về danh sách bàn khả dụng và gợi ý giờ thay thế
     */
    public Map<String, Object> checkAvailability(LocalDate date, LocalTime time, int guests) {
        LocalDateTime requestedStart = LocalDateTime.of(date, time);
        if (requestedStart.isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Thời gian đặt bàn phải trong tương lai.");
        }
        LocalDateTime requestedEnd = requestedStart.plusMinutes(durationMinutes + bufferMinutes); // 130p: duration + buffer

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

        // Chỉ xét những bàn hiện đang thực sự AVAILABLE và không bị soft-lock
        List<TableInfo> availableCurrent = allActiveTables.stream()
                .filter(t -> t.getStatus() == TableStatus.AVAILABLE && !t.isSoftLocked())
                .collect(Collectors.toList());

        LocalDateTime maxEndTime = time.plusMinutes(durationMinutes + bufferMinutes);

        List<TableInfo> singleMatch = availableCurrent.stream()
                .filter(t -> t.getCapacity() >= guests)
                .collect(Collectors.toList());

        for (TableInfo t : singleMatch) {
            List<Reservation> nextBookings = reservationRepository.findNextBookingForTable(t.getTableId(), time);
            
            if (!nextBookings.isEmpty() && nextBookings.get(0).getStartTime().isBefore(maxEndTime)) {
                // Có booking kế tiếp cắt ngang -> PARTIAL
                result.add(AvailableWindowResponse.builder()
                        .tableId(t.getTableId())
                        .capacity(t.getCapacity())
                        .availability(AvailableWindowResponse.Availability.PARTIAL_AVAILABLE)
                        .availableUntil(nextBookings.get(0).getStartTime())
                        .build());
            } else {
                // Trống hoàn toàn trong suốt khoảng thời gian mặc định
                result.add(AvailableWindowResponse.builder()
                        .tableId(t.getTableId())
                        .capacity(t.getCapacity())
                        .availability(AvailableWindowResponse.Availability.FULL_AVAILABLE)
                        .build());
            }
        }

        // Ghép bàn: tìm tổ hợp các bàn nhỏ hơn
        List<TableInfo> smallAvailable = availableCurrent.stream()
                .filter(t -> t.getCapacity() < guests)
                .collect(Collectors.toList());

        List<Long> mergeCandidates = new ArrayList<>();
        int total = 0;
        // Bỏ qua những bàn có booking kế tiếp cắt ngang để việc ghép bàn an toàn hơn
        for (TableInfo t : smallAvailable) {
            List<Reservation> nextBookings = reservationRepository.findNextBookingForTable(t.getTableId(), time);
            if (!nextBookings.isEmpty() && nextBookings.get(0).getStartTime().isBefore(maxEndTime)) {
                continue; // Bàn này sắp bị chiếm, không khuyên ghép cho FULL_AVAILABLE
            }
            mergeCandidates.add(t.getTableId());
            total += t.getCapacity();
            if (total >= guests) break;
        }

        if (total >= guests && mergeCandidates.size() > 1) {
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
