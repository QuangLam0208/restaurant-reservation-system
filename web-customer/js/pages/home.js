import { initAnimations, initPasswordToggle } from '../common/utils.js';
import { initHeaderScroll } from '../components/header.js';
import { initModals } from '../components/modal.js';
import { initAuth } from '../features/auth.js';

document.addEventListener("DOMContentLoaded", () => {
    initHeaderScroll();     // Cuộn header
    initAnimations();       // Hiệu ứng hiện dần
    initModals();           // Logic đóng/mở Modal
    initPasswordToggle();   // Logic con mắt password
    initAuth();             // Logic đăng nhập/đăng ký
});