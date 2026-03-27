package com.hcmute.reservation.service;

import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class EmailService {

    private final JavaMailSender mailSender;

    @Value("${spring.mail.username}")
    private String from;

    @Value("${app.base-url}")
    private String baseUrl;

    @Value("${server.port:8081}")
    private String serverPort;

    public void sendVerificationEmail(String toEmail, String token) {
        String link = "http://localhost:" + serverPort + "/api/auth/verify-email?token=" + token;
        send(toEmail,
                "Xác minh email - Nhà Hàng Đặt Bàn",
                "Chào bạn,\n\nVui lòng click vào link sau để xác minh tài khoản:\n" + link +
                        "\n\nLink có hiệu lực trong 24 giờ.\n\nTrân trọng.");
    }

    public void sendResetPasswordEmail(String toEmail, String token) {
        String link = "http://localhost:" + serverPort + "/api/auth/reset-password-page?token=" + token;
        send(toEmail,
                "Đặt lại mật khẩu - Nhà Hàng Đặt Bàn",
                "Chào bạn,\n\nVui lòng click vào link sau để đặt lại mật khẩu:\n" + link +
                        "\n\nLink có hiệu lực trong 15 phút. Nếu bạn không yêu cầu, hãy bỏ qua email này.\n\nTrân trọng.");
    }

    private void send(String to, String subject, String body) {
        SimpleMailMessage message = new SimpleMailMessage();
        message.setFrom(from);
        message.setTo(to);
        message.setSubject(subject);
        message.setText(body);
        mailSender.send(message);
    }
}
