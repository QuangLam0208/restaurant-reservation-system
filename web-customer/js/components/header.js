export function initHeaderScroll() {
    const header = document.getElementById("main-header");
    if (!header) return;

    const isAlwaysScrolled = header.classList.contains("scrolled");

    if (!isAlwaysScrolled) {
        window.addEventListener("scroll", () => {
            if (window.scrollY > 50) {
                header.classList.add("scrolled");
            } else {
                header.classList.remove("scrolled");
            }
        });
        if (window.scrollY > 50) header.classList.add("scrolled");
    }
}

// Hàm cập nhật chữ ở Header khi Login/Logout
export function updateHeaderAuthUI(isLoggedIn) {
    const authBtn = document.getElementById("header-auth-btn");
    const authGroup = document.getElementById("nav-auth-group");
    const greeting = document.getElementById("user-greeting");

    if (isLoggedIn) {
        const userName = localStorage.getItem("userName");
        if (authBtn) authBtn.style.display = "none";
        if (authGroup) authGroup.style.display = "flex";
        if (greeting) greeting.textContent = userName ? `Hi, ${userName}` : "Hi, Guest";
    } else {
        if (authBtn) authBtn.style.display = "flex";
        if (authGroup) authGroup.style.display = "none";
    }
}