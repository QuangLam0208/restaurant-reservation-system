export function showLoginModal() {
    const loginModal = document.getElementById("login-modal");
    if (loginModal) loginModal.classList.remove("hidden");
}

export function showRegisterModal() {
    const registerModal = document.getElementById("register-modal");
    if (registerModal) registerModal.classList.remove("hidden");
}

export function hideModals() {
    const loginModal = document.getElementById("login-modal");
    const registerModal = document.getElementById("register-modal");
    if (loginModal) loginModal.classList.add("hidden");
    if (registerModal) registerModal.classList.add("hidden");
}

export function initModals() {
    const loginModal = document.getElementById("login-modal");
    const registerModal = document.getElementById("register-modal");
    const showRegisterBtn = document.getElementById("show-register-btn");
    const showLoginBtn = document.getElementById("show-login-btn");

    // Xử lý nút close
    document.querySelectorAll(".close-btn").forEach(btn => {
        btn.addEventListener("click", hideModals);
    });

    // Click ra ngoài modal để đóng
    window.addEventListener("click", (e) => {
        if (e.target === loginModal || e.target === registerModal) {
            hideModals();
        }
    });

    // Chuyển đổi qua lại giữa Login & Register
    if (showRegisterBtn) {
        showRegisterBtn.addEventListener("click", (e) => {
            e.preventDefault();
            if (loginModal) loginModal.classList.add("hidden");
            showRegisterModal();
        });
    }

    if (showLoginBtn) {
        showLoginBtn.addEventListener("click", (e) => {
            e.preventDefault();
            if (registerModal) registerModal.classList.add("hidden");
            showLoginModal();
        });
    }
}