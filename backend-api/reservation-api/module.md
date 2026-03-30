
---

# Tài liệu Thiết kế Kiến trúc: Hệ thống Quản lý & Đặt bàn Nhà hàng (Reservation System)

Hệ thống được thiết kế dựa trên các nguyên lý **OOSE** hiện đại và tuân thủ chặt chẽ 5 nguyên lý **SOLID**. Điểm nhấn của kiến trúc là sự phân chia rõ ràng giữa các tầng (Layered Architecture), áp dụng **Strategy Pattern** để dễ mở rộng, và tuân thủ tuyệt đối **DIP (Dependency Inversion Principle)** thông qua cấu trúc `Interface -> Implementation`.

## I. KIẾN TRÚC TỔNG THỂ (UML)

Hệ thống chia làm 3 nhóm chính: **Core Domain** (Logic nghiệp vụ lõi), **Application Services** (Luồng Use Case ứng dụng) và **Infrastructure / Notification** (Hạ tầng và Giao tiếp ngoài). Tất cả tương tác giữa các module đều thông qua **Interface (Bản hợp đồng)**.

```plantuml
@startuml
skinparam packageStyle rectangle
skinparam classAttributeIconSize 0

package "Core Domain Services (Dịch vụ lõi)" {
  interface TableAvailabilityService <<Interface>>
  class TableAvailabilityServiceImpl
  
  interface TableAllocationStrategy <<Strategy>>
  class SingleTableStrategy {Order 1}
  class OptimalCapacityMergeStrategy {Order 2}
}

package "Application Services (Dịch vụ ứng dụng)" {
  interface TableService <<Interface>>
  class TableServiceImpl {
    - tableInfoRepository
    + getFloorMap()
    + updateTable(...)
  }

  interface AssignmentService <<Interface>>
  class AssignmentServiceImpl

  interface AvailabilityApiService <<Interface>>
  class AvailabilityApiServiceImpl
}

package "Infrastructure & Notification (Hạ tầng)" {
  interface ReservationMappingService <<Interface>>
  class ReservationMappingServiceImpl
  
  interface EmailService <<Interface>>
  class EmailServiceImpl {
    - mailSender: JavaMailSender
    + sendCustomEmail(...)
    + sendReservationConfirmationEmail(...)
  }
}

' Thực thi (Realization)
TableAvailabilityService <|.. TableAvailabilityServiceImpl
TableAllocationStrategy <|.. SingleTableStrategy
TableAllocationStrategy <|.. OptimalCapacityMergeStrategy

TableService <|.. TableServiceImpl
AssignmentService <|.. AssignmentServiceImpl
AvailabilityApiService <|.. AvailabilityApiServiceImpl
ReservationMappingService <|.. ReservationMappingServiceImpl
EmailService <|.. EmailServiceImpl

' Phụ thuộc (Dependency)
AssignmentServiceImpl --> TableAvailabilityService
AssignmentServiceImpl --> TableAllocationStrategy
AssignmentServiceImpl --> ReservationMappingService

AvailabilityApiServiceImpl --> TableAvailabilityService
AvailabilityApiServiceImpl --> TableAllocationStrategy

@enduml
```

---

## II. CHI TIẾT CÁC THÀNH PHẦN

### 1. Nhóm Core Domain (Nền tảng logic)

#### **`TableAvailabilityService` (Interface & Impl)**
* **Mục đích:** Là nguồn sự thật duy nhất (Single Source of Truth) về trạng thái trống/bận thực tế của bàn.
* **Đặc điểm SOLID:** Tuân thủ **SRP**. Ngăn chặn lỗi N+1 Query bằng cách cung cấp các API lấy dữ liệu thô tối ưu (`getCurrentlyAvailableTables`, `getNextBookingTime`), hỗ trợ xử lý cả `bufferMinutes` (thời gian dọn dẹp).

#### **`TableAllocationStrategy` (Interface & Impls)**
* **Mục đích:** Hệ thống thuật toán lõi giúp tìm bàn phù hợp cho khách.
* **Cài đặt:**
  * **`SingleTableStrategy` (`@Order(1)`):** Quét nhanh bàn đơn.
  * **`OptimalCapacityMergeStrategy` (`@Order(2)`):** Áp dụng **Quy hoạch động (Dynamic Programming)** để tìm tổ hợp ghép bàn ít dư thừa chỗ nhất. Tuân thủ **OCP**, dễ dàng thay đổi thuật toán.

---

### 2. Nhóm Application Services (Luồng nghiệp vụ)
*Toàn bộ tầng này giao tiếp với Controller thông qua Interface, giấu kín chi tiết cài đặt ở class Impl.*

#### **`TableService` (Quản lý Bàn vật lý)**
* **Mục đích:** Cung cấp các API CRUD cho bàn (tạo, sửa, xóa, lấy sơ đồ Floor Map).
* **Điểm sáng Kiến trúc:**
  * **Thread-Safety (An toàn Đa luồng):** Đã khắc phục triệt để lỗi ghi đè dữ liệu cục bộ bằng cách đưa các biến trạng thái (như `resTime`) vào phạm vi Local Variable, đảm bảo an toàn khi nhiều nhân viên cùng truy cập Sơ đồ bàn (Floor Map) cùng lúc.
  * **Optimistic Locking:** Sử dụng cơ chế kiểm tra `version` để chống ghi đè dữ liệu đồng thời (Concurrency Control).
  * **Clean Code:** Tách các logic kiểm tra (Validation) phức tạp thành các hàm `private` phụ trợ để tuân thủ DRY và SRP.

#### **`AssignmentService` (Command Side - Gán bàn)**
* **Mục đích:** Điều phối việc lấy pool bàn trống, thử lần lượt các Strategy đa hình (Đơn -> Ghép) cho đến khi thành công và gọi DB lưu lại.

#### **`AvailabilityApiService` (Query Side - Tra cứu)**
* **Mục đích:** Phục vụ luồng Booking Online và màn hình POS.
* **Đặc điểm:** Áp dụng mẫu kiến trúc **CQRS**, hoàn toàn không gọi trực tiếp Repository mà chỉ lấy dữ liệu thông qua `TableAvailabilityService` để tính toán và cache lại kết quả.

---

### 3. Nhóm Infrastructure & Notification (Hạ tầng)

#### **`EmailService` (Dịch vụ Thông báo)**
* **Mục đích:** Quản lý toàn bộ việc định dạng template và gửi Email qua giao thức SMTP.
* **Điểm sáng Kiến trúc:**
  * **SRP Tuyệt đối:** Chuyển toàn bộ trách nhiệm "nặn" nội dung email (lời chào, format ngày giờ) từ `EmailNotificationListener` về cho `EmailServiceImpl`. Listener lúc này chỉ làm đúng 1 nhiệm vụ là "Nghe Event và Điều hướng", giúp hệ thống dễ bảo trì và mở rộng template.
  * **DIP:** Các module khác (Auth, Event) chỉ phụ thuộc vào `EmailService` (Interface). Việc thay đổi từ JavaMailSender sang API bên thứ 3 (VD: SendGrid, Amazon SES) sẽ không làm ảnh hưởng đến luồng nghiệp vụ.

#### **`ReservationMappingService` (Lưu trữ quan hệ DB)**
* **Mục đích:** Quản lý việc ghi xuống Database các mối quan hệ N-N. Sử dụng `@Transactional(propagation = Propagation.MANDATORY)` để đảm bảo tính toàn vẹn (Atomic).

---

## III. TỔNG KẾT TƯ DUY KIẾN TRÚC (DESIGN MINDSET)

Kiến trúc hiện tại đã giải quyết được các bài toán khó nhất của một hệ thống Enterprise:
1.  **Dependency Inversion (DIP):** Không có Service nào giao tiếp trực tiếp với Concrete Class của Service khác. Mọi thứ đều là Hợp đồng (Interface).
2.  **Đa luồng & Concurrency:** Từ việc xử lý biến cục bộ trong `TableService` đến áp dụng Optimistic Locking (`version`) đảm bảo dữ liệu không bao giờ bị corrupt khi tải cao.
3.  **Clean Architecture:** Tách bạch rõ ràng logic kết nối Database, thuật toán toán học, và logic tạo Template UI (Email).

---
*Tài liệu được cập nhật dựa trên quy trình Tái cấu trúc (Refactoring) theo tiêu chuẩn OOSE & SOLID mới nhất.*

---