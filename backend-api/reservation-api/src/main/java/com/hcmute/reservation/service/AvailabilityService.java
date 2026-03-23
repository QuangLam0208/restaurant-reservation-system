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
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
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
     * Tra ve danh sach ban kha dung va goi y gio thay the
     */
    public Map<String, Object> checkAvailability(LocalDate date, LocalTime time, int guests) {
        LocalDateTime requestedStart = LocalDateTime.of(date, time);
        if (requestedStart.isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Thoi gian dat ban phai trong tuong lai.");
        }
        LocalDateTime requestedEnd = requestedStart.plusMinutes(durationMinutes + bufferMinutes);

        List<Long> occupiedIds = reservationRepository.findOccupiedTableIds(requestedStart, requestedEnd);
        Set<Long> occupiedSet = new HashSet<>(occupiedIds);

        List<TableInfo> availableTables = tableInfoRepository.findAvailableTablesForGuests(guests)
                .stream()
                .filter(table -> !occupiedSet.contains(table.getTableId()))
                .collect(Collectors.toList());

        List<Map<String, Object>> tables = availableTables.stream().map(table -> {
            Map<String, Object> payload = new LinkedHashMap<>();
            payload.put("tableId", table.getTableId());
            payload.put("capacity", table.getCapacity());
            payload.put("status", table.getStatus());
            return payload;
        }).collect(Collectors.toList());

        Map<String, Object> result = new LinkedHashMap<>();
        result.put("requestedTime", requestedStart);
        result.put("guestCount", guests);
        result.put("availableTables", tables);
        result.put("hasAvailability", !tables.isEmpty());

        if (tables.isEmpty()) {
            result.put("alternativeTimes", List.of(
                    requestedStart.plusMinutes(30).toString(),
                    requestedStart.plusMinutes(60).toString()
            ));
        }
        return result;
    }

    /**
     * GET /api/tables/available-windows
     * Phan tich real-time cho POS
     */
    @Cacheable(value = "availableWindows", key = "#guests + '_' + #time")
    public List<AvailableWindowResponse> getAvailableWindows(int guests, LocalDateTime time) {
        validateAvailableWindowRequest(guests, time);

        List<TableInfo> availableCurrent = tableInfoRepository.findByIsActiveTrue().stream()
                .filter(table -> table.getStatus() == TableStatus.AVAILABLE && !table.isSoftLocked())
                .sorted(Comparator.comparingInt(TableInfo::getCapacity)
                        .thenComparing(TableInfo::getTableId))
                .collect(Collectors.toList());

        LocalDateTime maxEndTime = time.plusMinutes(durationMinutes + bufferMinutes);
        List<AvailableWindowResponse> fullyAvailable = new ArrayList<>();
        List<AvailableWindowResponse> partiallyAvailable = new ArrayList<>();

        for (TableInfo table : availableCurrent) {
            if (table.getCapacity() < guests) {
                continue;
            }

            LocalDateTime nextBookingStart = findNextBookingStart(table.getTableId(), time);
            if (nextBookingStart != null && nextBookingStart.isBefore(maxEndTime)) {
                partiallyAvailable.add(AvailableWindowResponse.builder()
                        .tableId(table.getTableId())
                        .capacity(table.getCapacity())
                        .availability(AvailableWindowResponse.Availability.PARTIAL_AVAILABLE)
                        .availableUntil(nextBookingStart)
                        .build());
            } else {
                fullyAvailable.add(AvailableWindowResponse.builder()
                        .tableId(table.getTableId())
                        .capacity(table.getCapacity())
                        .availability(AvailableWindowResponse.Availability.FULL_AVAILABLE)
                        .build());
            }
        }

        List<TableInfo> mergeCandidates = availableCurrent.stream()
                .filter(table -> table.getCapacity() < guests)
                .filter(table -> isFullyAvailableForWindow(table.getTableId(), time, maxEndTime))
                .collect(Collectors.toList());
        List<TableInfo> bestMerge = findBestMergeCombination(mergeCandidates, guests);

        List<AvailableWindowResponse> result = new ArrayList<>(fullyAvailable);
        result.addAll(partiallyAvailable);
        if (!bestMerge.isEmpty()) {
            result.add(AvailableWindowResponse.builder()
                    .tableId(null)
                    .capacity(bestMerge.stream().mapToInt(TableInfo::getCapacity).sum())
                    .availability(AvailableWindowResponse.Availability.FULL_AVAILABLE)
                    .mergeCandidateIds(bestMerge.stream()
                            .map(TableInfo::getTableId)
                            .sorted()
                            .collect(Collectors.toList()))
                    .build());
        }

        return result;
    }

    private void validateAvailableWindowRequest(int guests, LocalDateTime time) {
        if (guests <= 0) {
            throw new BadRequestException("So luong khach phai lon hon 0.");
        }
        if (time == null) {
            throw new BadRequestException("Thieu thoi diem can kiem tra ban.");
        }
        if (time.isBefore(LocalDateTime.now().minusSeconds(30))) {
            throw new BadRequestException("Thoi diem can kiem tra phai tu hien tai tro di.");
        }
    }

    private LocalDateTime findNextBookingStart(Long tableId, LocalDateTime time) {
        List<Reservation> nextBookings = reservationRepository.findNextBookingForTable(tableId, time);
        if (nextBookings.isEmpty()) {
            return null;
        }
        return nextBookings.get(0).getStartTime();
    }

    private boolean isFullyAvailableForWindow(Long tableId, LocalDateTime time, LocalDateTime maxEndTime) {
        LocalDateTime nextBookingStart = findNextBookingStart(tableId, time);
        return nextBookingStart == null || !nextBookingStart.isBefore(maxEndTime);
    }

    private List<TableInfo> findBestMergeCombination(List<TableInfo> candidates, int guests) {
        if (candidates.size() < 2) {
            return List.of();
        }

        List<TableInfo> sortedCandidates = candidates.stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity)
                        .thenComparing(TableInfo::getTableId))
                .collect(Collectors.toList());
        int maxCapacity = sortedCandidates.stream().mapToInt(TableInfo::getCapacity).sum();
        if (maxCapacity < guests) {
            return List.of();
        }

        List<List<TableInfo>> bestByCapacity = new ArrayList<>(Collections.nCopies(maxCapacity + 1, null));
        bestByCapacity.set(0, List.of());

        for (TableInfo table : sortedCandidates) {
            for (int current = maxCapacity - table.getCapacity(); current >= 0; current--) {
                List<TableInfo> existing = bestByCapacity.get(current);
                if (existing == null) {
                    continue;
                }

                int nextCapacity = current + table.getCapacity();
                List<TableInfo> candidateCombo = new ArrayList<>(existing);
                candidateCombo.add(table);

                List<TableInfo> stored = bestByCapacity.get(nextCapacity);
                if (stored == null || isPreferredForSameCapacity(candidateCombo, stored)) {
                    bestByCapacity.set(nextCapacity, List.copyOf(candidateCombo));
                }
            }
        }

        List<TableInfo> best = null;
        for (int capacity = guests; capacity <= maxCapacity; capacity++) {
            List<TableInfo> combo = bestByCapacity.get(capacity);
            if (combo == null || combo.size() < 2) {
                continue;
            }
            if (best == null || isBetterMergeResult(combo, best)) {
                best = combo;
            }
        }

        return best == null ? List.of() : best;
    }

    private boolean isPreferredForSameCapacity(List<TableInfo> candidate, List<TableInfo> existing) {
        if (candidate.size() != existing.size()) {
            return candidate.size() < existing.size();
        }
        return compareIdSequences(candidate, existing) < 0;
    }

    private boolean isBetterMergeResult(List<TableInfo> candidate, List<TableInfo> currentBest) {
        int candidateCapacity = candidate.stream().mapToInt(TableInfo::getCapacity).sum();
        int bestCapacity = currentBest.stream().mapToInt(TableInfo::getCapacity).sum();
        if (candidateCapacity != bestCapacity) {
            return candidateCapacity < bestCapacity;
        }
        if (candidate.size() != currentBest.size()) {
            return candidate.size() < currentBest.size();
        }
        return compareIdSequences(candidate, currentBest) < 0;
    }

    private int compareIdSequences(List<TableInfo> first, List<TableInfo> second) {
        List<Long> firstIds = first.stream().map(TableInfo::getTableId).sorted().collect(Collectors.toList());
        List<Long> secondIds = second.stream().map(TableInfo::getTableId).sorted().collect(Collectors.toList());
        for (int i = 0; i < Math.min(firstIds.size(), secondIds.size()); i++) {
            int cmp = firstIds.get(i).compareTo(secondIds.get(i));
            if (cmp != 0) {
                return cmp;
            }
        }
        return Integer.compare(firstIds.size(), secondIds.size());
    }
}
