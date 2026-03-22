package com.hcmute.reservation.model;

import com.hcmute.reservation.model.enums.TableStatus;
import jakarta.persistence.*;
import lombok.*;
import java.util.List;

@Entity
@Table(name = "table_info")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class TableInfo {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "table_id")
    private Long tableId;

    @Column(name = "capacity", nullable = false)
    private int capacity;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false)
    @Builder.Default
    private TableStatus status = TableStatus.AVAILABLE;

    // Optimistic Locking — JPA tự động tăng version mỗi lần UPDATE
    // Nếu 2 lễ tân cùng đọc version=1 và cùng ghi → người thứ 2 nhận OptimisticLockException
    @Version
    @Column(name = "version", nullable = false)
    private int version;

    // Quan hệ N-N với Reservation thông qua bảng trung gian
    @OneToMany(mappedBy = "tableInfo", fetch = FetchType.LAZY)
    private List<ReservationTableMapping> mappings;
}
