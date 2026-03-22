import { initAnimations } from '../common/utils.js';
import { initHeaderScroll } from '../components/header.js';
import { initAuth, checkIsLoggedIn } from '../features/auth.js';
import { initBooking } from '../features/booking.js';

document.addEventListener("DOMContentLoaded", () => {
    // Route Guard: Cốt lõi của SPA (Dù đã chặn ở HTML nhưng nên có ở JS)
    if (!checkIsLoggedIn()) {
        window.location.href = "index.html";
        return;
    }

    initHeaderScroll();
    initAnimations();
    initAuth();             // Cần khởi tạo để hiện nút Logout
    initBooking();          // Logic xử lý form booking
});