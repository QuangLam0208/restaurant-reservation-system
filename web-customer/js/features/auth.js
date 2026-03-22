import { updateHeaderAuthUI } from '../components/header.js';
import { showLoginModal, hideModals } from '../components/modal.js';

export function checkIsLoggedIn() {
    return localStorage.getItem("isLoggedIn") === "true";
}

export function initAuth() {
    // 1. Khởi tạo trạng thái ban đầu của Header
    updateHeaderAuthUI(checkIsLoggedIn());

    // 2. Xử lý nút bật Modal Login (trên header hoặc body)
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

    // 3. Xử lý form Login
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

    // 4. Xử lý form Register
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

    // 5. Xử lý nút Logout
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