package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.dto.table.AvailableWindowResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.AvailabilityApiService;
import com.hcmute.reservation.service.TableAvailabilityService;
import com.hcmute.reservation.strategy.TableAllocationStrategy;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class AvailabilityApiServiceImpl implements AvailabilityApiService {

    private final TableAvailabilityService tableAvailabilityService;
    private final TableAllocationStrategy optimalCapacityMergeStrategy;

    @Value("${reservation.duration-minutes:120}")
    private int durationMinutes;

    @Value("${reservation.buffer-minutes:10}")
    private int bufferMinutes;

    @Override
    public Map<String, Object> checkAvailability(LocalDate date, LocalTime time, int guests) {
        LocalDateTime requestedStart = LocalDateTime.of(date, time);
        validateFutureTime(requestedStart);

        LocalDateTime requestedEnd = requestedStart.plusMinutes(durationMinutes);

        // Lấy tất cả các bàn trống trong khung giờ này
        List<TableInfo> allFreeTables = tableAvailabilityService.getFreeTables(requestedStart, requestedEnd);

        // Chỉ lọc lại những bàn đủ sức chứa cho số lượng khách (guests)
        List<TableInfo> availableTables = allFreeTables.stream()
                .filter(table -> table.getCapacity() >= guests)
                .collect(Collectors.toList());

        List<Map<String, Object>> tablesPayload = availableTables.stream().map(table -> {
            Map<String, Object> payload = new LinkedHashMap<>();
            payload.put("tableId", table.getTableId());
            payload.put("capacity", table.getCapacity());
            payload.put("status", table.getStatus());
            return payload;
        }).collect(Collectors.toList());

        Map<String, Object> result = new LinkedHashMap<>();
        result.put("requestedTime", requestedStart);
        result.put("guestCount", guests);
        result.put("availableTables", tablesPayload);
        result.put("hasAvailability", !tablesPayload.isEmpty());

        if (tablesPayload.isEmpty()) {
            result.put("alternativeTimes", List.of(
                    requestedStart.plusMinutes(30).toString(),
                    requestedStart.plusMinutes(60).toString()
            ));
        }
        return result;
    }

    @Override
    @Cacheable(value = "availableWindows", key = "#guests + '_' + #time")
    public List<AvailableWindowResponse> getAvailableWindows(int guests, LocalDateTime time) {
        validateAvailableWindowRequest(guests, time);

        // GỌI CORE SERVICE lấy bàn trống hiện tại
        List<TableInfo> availableCurrent = tableAvailabilityService.getCurrentlyAvailableTables().stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).thenComparing(TableInfo::getTableId))
                .collect(Collectors.toList());

        LocalDateTime maxEndTime = time.plusMinutes(durationMinutes + bufferMinutes);
        List<AvailableWindowResponse> fullyAvailable = new ArrayList<>();
        List<AvailableWindowResponse> partiallyAvailable = new ArrayList<>();

        // 1. Phân loại bàn (Trống hoàn toàn & Trống một phần)
        for (TableInfo table : availableCurrent) {
            if (table.getCapacity() < guests) continue;

            LocalDateTime nextBookingStart = tableAvailabilityService.getNextBookingTime(table.getTableId(), time);
            if (nextBookingStart != null && nextBookingStart.isBefore(maxEndTime)) {
                partiallyAvailable.add(buildWindowResponse(table, AvailableWindowResponse.Availability.PARTIAL_AVAILABLE, nextBookingStart));
            } else {
                fullyAvailable.add(buildWindowResponse(table, AvailableWindowResponse.Availability.FULL_AVAILABLE, null));
            }
        }

        // 2. Tìm tổ hợp ghép bàn (Sử dụng Strategy)
        List<TableInfo> mergeCandidates = availableCurrent.stream()
                .filter(table -> table.getCapacity() < guests)
                .filter(table -> isFullyAvailableForWindow(table.getTableId(), time, maxEndTime))
                .collect(Collectors.toList());

        List<TableInfo> bestMerge = optimalCapacityMergeStrategy.allocate(guests, mergeCandidates);

        // 3. Tổng hợp kết quả
        List<AvailableWindowResponse> result = new ArrayList<>(fullyAvailable);
        result.addAll(partiallyAvailable);

        if (!bestMerge.isEmpty()) {
            result.add(AvailableWindowResponse.builder()
                    .tableId(null)
                    .capacity(bestMerge.stream().mapToInt(TableInfo::getCapacity).sum())
                    .availability(AvailableWindowResponse.Availability.FULL_AVAILABLE)
                    .mergeCandidateIds(bestMerge.stream().map(TableInfo::getTableId).sorted().collect(Collectors.toList()))
                    .build());
        }

        return result;
    }

    // --- CÁC HÀM PRIVATE PHỤ TRỢ ---

    private void validateFutureTime(LocalDateTime requestedStart) {
        if (requestedStart.isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Thời gian đặt bàn phải trong tương lai.");
        }
    }

    private void validateAvailableWindowRequest(int guests, LocalDateTime time) {
        if (guests <= 0) throw new BadRequestException("Số lượng khách phải lớn hơn 0.");
        if (time == null) throw new BadRequestException("Thiếu thời điểm cần kiểm tra bàn.");
        if (time.isBefore(LocalDateTime.now().minusSeconds(30))) {
            throw new BadRequestException("Thời điểm cần kiểm tra phải từ hiện tại trở đi.");
        }
    }

    private boolean isFullyAvailableForWindow(Long tableId, LocalDateTime time, LocalDateTime maxEndTime) {
        LocalDateTime nextBookingStart = tableAvailabilityService.getNextBookingTime(tableId, time);
        return nextBookingStart == null || !nextBookingStart.isBefore(maxEndTime);
    }

    private AvailableWindowResponse buildWindowResponse(TableInfo table, AvailableWindowResponse.Availability availability, LocalDateTime until) {
        return AvailableWindowResponse.builder()
                .tableId(table.getTableId())
                .capacity(table.getCapacity())
                .availability(availability)
                .availableUntil(until)
                .build();
    }
}
