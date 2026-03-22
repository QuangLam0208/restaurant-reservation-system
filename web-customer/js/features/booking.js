export function initBooking() {
    const bookingForm = document.getElementById('booking-form');
    if (bookingForm) {
        bookingForm.addEventListener('submit', function(e) {
            e.preventDefault();
            const btn = this.querySelector('button[type="submit"]');
            btn.innerText = "Processing...";
            btn.disabled = true;

            setTimeout(() => {
                alert("✨ Reservation Successful! Your booking ID is: #RES-9981");
                window.location.href = "index.html";
            }, 1500);
        });
    }
}