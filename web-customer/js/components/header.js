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
    const headerAuthBtn = document.getElementById("header-auth-btn");
    const logoutBtn = document.getElementById("logout-btn");

    if (isLoggedIn) {
        const userName = localStorage.getItem("userName");
        if (headerAuthBtn) headerAuthBtn.innerText = userName ? `Hi, ${userName}` : "Book a Table";
        if (logoutBtn) logoutBtn.classList.remove("hidden");
    } else {
        if (headerAuthBtn) headerAuthBtn.innerText = "Login";
        if (logoutBtn) logoutBtn.classList.add("hidden");
    }
}