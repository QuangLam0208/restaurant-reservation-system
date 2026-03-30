package com.hcmute.reservation.event;

import com.hcmute.reservation.service.EmailService;
import org.springframework.context.event.EventListener;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Component;

import java.time.format.DateTimeFormatter;

@Component
public class EmailNotificationListener {

    private final EmailService emailService;

    public EmailNotificationListener(EmailService emailService) {
        this.emailService = emailService;
    }

    // @Async giúp hàm này chạy ở một luồng (thread) khác, không làm chậm response thanh toán
    // Không dùng @Transactional ở đây nữa vì data đã được truyền trực tiếp qua Event, không cần truy vấn DB
    @Async
    @EventListener
    public void handleReservationConfirmedEvent(ReservationConfirmedEvent event) {
        String customerEmail = event.getCustomerEmail();
        if (customerEmail == null) return;

        String customerName = event.getCustomerName();
        Long reservationId = event.getReservationId();
        String startTimeFormatted = event.getStartTime().format(DateTimeFormatter.ofPattern("HH:mm dd/MM/yyyy"));

        // Gọi service gửi Email
        emailService.send(
                customerEmail,
                "Xác nhận đặt bàn thành công - San-Lorenzo Restaurant",
                String.format("Chào %s,\n\nChúc mừng bạn đã đặt bàn thành công tại San-Lorenzo!\n\n" +
                        "Mã đặt bàn của bạn: SL-%d\n" +
                        "Thời gian: %s\n\n" +
                        "Hẹn gặp lại bạn sớm.\nTrân trọng.", customerName, reservationId, startTimeFormatted)
        );
    }
}