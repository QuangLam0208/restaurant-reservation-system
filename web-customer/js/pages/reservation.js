import { initAnimations } from '../common/utils.js';
import { initHeaderScroll } from '../components/header.js';
import { initAuth, checkIsLoggedIn } from '../features/auth.js';
import { initBooking } from '../features/booking.js';

document.addEventListener("DOMContentLoaded", () => {
    if (!checkIsLoggedIn()) {
        window.location.href = "index.html";
        return;
    }

    initHeaderScroll();
    initAnimations();
    initAuth();
    initBooking();
});