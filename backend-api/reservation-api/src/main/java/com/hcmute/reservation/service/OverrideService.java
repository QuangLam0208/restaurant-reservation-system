package com.hcmute.reservation.service;

import com.hcmute.reservation.dto.override.OverrideLogResponse;
import com.hcmute.reservation.dto.override.OverrideRequest;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.Account;
import com.hcmute.reservation.model.OverrideLog;
import com.hcmute.reservation.model.Reservation;
import com.hcmute.reservation.model.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.AccountRepository;
import com.hcmute.reservation.repository.OverrideLogRepository;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class OverrideService {

    private final ReservationRepository reservationRepository;
    private final AccountRepository accountRepository;
    private final OverrideLogRepository overrideLogRepository;
    private final TableInfoRepository tableInfoRepository;

    private OverrideLogResponse toResponse(OverrideLog log) {
        return OverrideLogResponse.builder()
                .logId(log.getLogId())
                .reservationId(log.getReservation().getReservationId())
                .accountId(log.getAccount().getAccountId())
                .accountUsername(log.getAccount().getUsername())
                .reason(log.getReason())
                .createdAt(log.getCreatedAt())
                .build();
    }

    @Transactional
    public OverrideLogResponse override(Long reservationId, OverrideRequest req, Long accountId) {
        Reservation reservation = reservationRepository.findById(reservationId)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn #" + reservationId + " không tồn tại."));
        if (reservation.getStatus() != ReservationStatus.SEATED) {
            throw new BadRequestException("Chỉ có thể ghi đè đơn đang ở trạng thái SEATED (Overstay).");
        }
        Account account = accountRepository.findById(accountId)
                .orElseThrow(() -> new ResourceNotFoundException("Nhân viên #" + accountId + " không tồn tại."));

        // Ghi đè → COMPLETED ngay, bỏ qua buffer_time
        reservation.setStatus(ReservationStatus.COMPLETED);
        reservation.setEndTime(LocalDateTime.now());
        reservationRepository.save(reservation);

        // Giải phóng bàn ngay
        if (reservation.getTableMappings() != null) {
            reservation.getTableMappings().forEach(m -> {
                TableInfo t = m.getTableInfo();
                t.setStatus(TableStatus.AVAILABLE);
                tableInfoRepository.save(t);
            });
        }

        // Ghi log
        OverrideLog log = OverrideLog.builder()
                .reservation(reservation)
                .account(account)
                .reason(req.getReason())
                .build();
        return toResponse(overrideLogRepository.save(log));
    }

    public List<OverrideLogResponse> getLogs(LocalDateTime from, LocalDateTime to, Long accountId) {
        return overrideLogRepository.findByFilters(from, to, accountId)
                .stream().map(this::toResponse).collect(Collectors.toList());
    }
}
