document.addEventListener("DOMContentLoaded", () => {
    // 1. Logic cho Header Scroll
    const header = document.getElementById("main-header");

    window.addEventListener("scroll", () => {
        if (window.scrollY > 50) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled");
        }
    });

    // Kích hoạt ngay lúc tải trang phòng trường hợp F5 giữa trang
    if (window.scrollY > 50) header.classList.add("scrolled");

    // 2. Logic cho Scroll Animations (Thay thế Framer Motion useInView)
    // Sử dụng IntersectionObserver (Tối ưu hiệu suất, pattern tiêu chuẩn hiện nay)
    const fadeElements = document.querySelectorAll(".fade-in");

    const appearOptions = {
        threshold: 0.2, // Khi hiển thị 20% element thì trigger
        rootMargin: "0px 0px -50px 0px"
    };

    const appearOnScroll = new IntersectionObserver(function(entries, observer) {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;

            // Thêm class 'appear' khi scroll tới
            entry.target.classList.add("appear");

            // Dừng observe sau khi đã hiện (triggerOnce: true trong React code)
            observer.unobserve(entry.target);
        });
    }, appearOptions);

    fadeElements.forEach(el => {
        appearOnScroll.observe(el);
    });
});