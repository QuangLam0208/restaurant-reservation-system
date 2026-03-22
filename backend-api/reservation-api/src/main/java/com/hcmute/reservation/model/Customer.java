package com.hcmute.reservation.model;

import jakarta.persistence.*;
import lombok.*;
import java.util.List;

@Entity
@Table(name = "customer")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Customer {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "customer_id")
    private Long customerId;

    @Column(name = "name", nullable = false)
    private String name;

    @Column(name = "phone", nullable = false)
    private String phone;

    // Nullable: Walk-in vãng lai không cần email
    @Column(name = "email", unique = true)
    private String email;

    // Nullable: Walk-in vãng lai không có password
    @Column(name = "password_hash")
    private String passwordHash;

    // Mặc định FALSE, chỉ TRUE sau khi xác minh email (mục 3.1.1)
    @Column(name = "is_verified", nullable = false)
    @Builder.Default
    private Boolean isVerified = Boolean.FALSE;

    // Quan hệ 1-N với Reservation (optional từ phía Customer - walk-in)
    @OneToMany(mappedBy = "customer", fetch = FetchType.LAZY)
    private List<Reservation> reservations;

    // Quan hệ 1-N với Waitlist
    @OneToMany(mappedBy = "customer", fetch = FetchType.LAZY)
    private List<Waitlist> waitlistEntries;
}