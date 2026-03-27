import { updateHeaderAuthUI } from '../components/header.js';
import { showLoginModal, showResetModal, hideModals } from '../components/modal.js';

export function checkIsLoggedIn() {
    return localStorage.getItem("isLoggedIn") === "true";
}

export function initAuth() {
    updateHeaderAuthUI(checkIsLoggedIn());

    const authActionBtns = document.querySelectorAll(".auth-action-btn");
    authActionBtns.forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            if (checkIsLoggedIn()) {
                window.location.href = "reservation.html";
            } else {
                showLoginModal();
            }
        });
    });

    // 1. FORM LOGIN
    const loginForm = document.getElementById("login-form");
    if (loginForm) {
        loginForm.addEventListener("submit", (e) => {
            e.preventDefault();
            const submitBtn = loginForm.querySelector("button[type='submit']");
            submitBtn.innerText = "Authenticating...";
            submitBtn.disabled = true;

            setTimeout(() => {
                localStorage.setItem("isLoggedIn", "true");
                hideModals();
                submitBtn.innerText = "Login";
                submitBtn.disabled = false;
                loginForm.reset();
                updateHeaderAuthUI(true);
                window.location.href = "reservation.html";
            }, 1000);
        });
    }

    // 2. FORM REGISTER
    const registerForm = document.getElementById("register-form");
    if (registerForm) {
        registerForm.addEventListener("submit", (e) => {
            e.preventDefault();
            const password = document.getElementById("reg-password").value;
            const confirmPassword = document.getElementById("reg-confirm-password").value;

            if (password !== confirmPassword) {
                alert("Passwords do not match! Please try again.");
                return;
            }

            const submitBtn = registerForm.querySelector("button[type='submit']");
            submitBtn.innerText = "Registering...";
            submitBtn.disabled = true;

            setTimeout(() => {
                localStorage.setItem("isLoggedIn", "true");
                hideModals();
                submitBtn.innerText = "Register";
                submitBtn.disabled = false;
                registerForm.reset();
                updateHeaderAuthUI(true);
                alert("Account created successfully!");
                window.location.href = "reservation.html";
            }, 1000);
        });
    }

    // 3. FORM QUÊN MẬT KHẨU (BƯỚC 1 - NHẬP EMAIL)
    const forgotForm = document.getElementById("forgot-form");
    if (forgotForm) {
        forgotForm.addEventListener("submit", (e) => {
            e.preventDefault(); // Chặn hành động reload trang
            const submitBtn = forgotForm.querySelector("button[type='submit']");
            const originalText = submitBtn.innerText;
            submitBtn.innerText = "Checking email...";
            submitBtn.disabled = true;

            // Giả lập gửi email thành công và chuyển sang bước nhập mật khẩu mới
            setTimeout(() => {
                hideModals();
                setTimeout(() => {
                    submitBtn.innerText = originalText;
                    submitBtn.disabled = false;
                    forgotForm.reset();
                    showResetModal(); // Gọi Modal Bước 2 lên
                }, 350);
            }, 1000);
        });
    }

    // 4. FORM ĐẶT LẠI MẬT KHẨU (BƯỚC 2 - NHẬP PASS MỚI)
    const resetForm = document.getElementById("reset-form");
    if (resetForm) {
        resetForm.addEventListener("submit", (e) => {
            e.preventDefault();
            const pw = document.getElementById("reset-password").value;
            const repw = document.getElementById("reset-confirm").value;

            if (pw !== repw) {
                alert("Passwords do not match! Please try again.");
                return;
            }

            const submitBtn = resetForm.querySelector("button[type='submit']");
            const originalText = submitBtn.innerText;
            submitBtn.innerText = "Updating...";
            submitBtn.disabled = true;

            // Giả lập đổi mật khẩu thành công và đẩy về lại Form Login
            setTimeout(() => {
                hideModals();
                setTimeout(() => {
                    submitBtn.innerText = originalText;
                    submitBtn.disabled = false;
                    resetForm.reset();
                    alert("Password updated successfully! Please login with your new password.");
                    showLoginModal();
                }, 350);
            }, 1000);
        });
    }

    // 5. LOGOUT
    const logoutBtn = document.getElementById("logout-btn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", (e) => {
            e.preventDefault();
            localStorage.removeItem("isLoggedIn");
            updateHeaderAuthUI(false);
            if (window.location.pathname.includes("reservation.html")) {
                window.location.href = "index.html";
            }
        });
    }
}