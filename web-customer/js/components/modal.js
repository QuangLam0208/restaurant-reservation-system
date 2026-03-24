export function showLoginModal() {
    const loginModal = document.getElementById("login-modal");
    if (loginModal) {
        loginModal.classList.remove("hidden");
        loginModal.classList.add("open");
        loginModal.style.opacity = "1";
        loginModal.style.pointerEvents = "auto";
        loginModal.style.visibility = "visible";
    }
}

export function showRegisterModal() {
    const registerModal = document.getElementById("register-modal");
    if (registerModal) {
        registerModal.classList.remove("hidden");
        registerModal.classList.add("open");
        registerModal.style.opacity = "1";
        registerModal.style.pointerEvents = "auto";
        registerModal.style.visibility = "visible";
    }
}

export function hideModals() {
    const loginModal = document.getElementById("login-modal");
    const registerModal = document.getElementById("register-modal");

    if (loginModal) {
        loginModal.classList.remove("open");
        loginModal.style.opacity = "0";
        loginModal.style.pointerEvents = "none";
        setTimeout(() => loginModal.classList.add("hidden"), 300);
    }
    if (registerModal) {
        registerModal.classList.remove("open");
        registerModal.style.opacity = "0";
        registerModal.style.pointerEvents = "none";
        setTimeout(() => registerModal.classList.add("hidden"), 300);
    }
}

export function initModals() {
    const loginModal = document.getElementById("login-modal");
    const registerModal = document.getElementById("register-modal");
    const showRegisterBtn = document.getElementById("show-register-btn");
    const showLoginBtn = document.getElementById("show-login-btn");

    document.querySelectorAll(".close-btn").forEach(btn => {
        btn.addEventListener("click", hideModals);
    });

    window.addEventListener("click", (e) => {
        if (e.target === loginModal || e.target === registerModal) {
            hideModals();
        }
    });

    if (showRegisterBtn) {
        showRegisterBtn.addEventListener("click", (e) => {
            e.preventDefault();
            hideModals();
            setTimeout(showRegisterModal, 300);
        });
    }

    if (showLoginBtn) {
        showLoginBtn.addEventListener("click", (e) => {
            e.preventDefault();
            hideModals();
            setTimeout(showLoginModal, 300);
        });
    }
}