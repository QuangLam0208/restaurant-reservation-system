import { callApi, getStoredUser, clearUser } from '../common/api.js';

const S = { party:2, date:null, dateStr:null, time:null, week:0, MAX_W:4, reservationId: null };
const DAYS   = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'];
const MONTHS = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
const today  = new Date(); today.setHours(0,0,0,0);
const dow = today.getDay(), toMon = dow===0?6:dow-1;
const thisMonday = new Date(today); thisMonday.setDate(today.getDate()-toMon);

let myBookingsList = [];
let countdownInterval = null;
let timeLeft = 300; // 5 phút = 300 giây

// ─── HÀM EXPORT CHO HTML GỌI ───
window.setParty = setParty;
window.confirmParty = confirmParty;
window.confirmDate = confirmDate;
window.confirmTime = confirmTime;
window.editStep = editStep;
window.goToPayment = goToPayment;
window.backToBooking = backToBooking;
window.confirmReservation = confirmReservation;
window.openBookingsModal = openBookingsModal;
window.closeBookingsModal = closeBookingsModal;
window.cancelMyBooking = cancelMyBooking;

// ─── LÔ-GIC TIMER ───
function startTimer() {
    clearInterval(countdownInterval);
    timeLeft = 300;
    updateTimerDisplay();

    countdownInterval = setInterval(() => {
        timeLeft--;
        updateTimerDisplay();
        if (timeLeft <= 0) {
            clearInterval(countdownInterval);
            alert("Thời gian giữ bàn 5 phút đã hết. Vui lòng thao tác lại để giữ bàn mới.");
            backToBooking();
        }
    }, 1000);
}

function stopTimer() { clearInterval(countdownInterval); }

function updateTimerDisplay() {
    const m = Math.floor(timeLeft / 60).toString().padStart(2, '0');
    const s = (timeLeft % 60).toString().padStart(2, '0');
    document.getElementById('timer-text').textContent = `${m}:${s}`;

    const percentage = (timeLeft / 300) * 100;
    document.querySelector('.timer-prog').setAttribute('stroke-dasharray', `${percentage}, 100`);

    const pill = document.getElementById('timer-pill');
    const prog = document.querySelector('.timer-prog');
    const text = document.getElementById('timer-text');

    if (timeLeft <= 60) {
        pill.style.borderColor = '#E76B6B';
        pill.style.background = '#FFF5F5';
        prog.style.stroke = '#E76B6B';
        text.style.color = '#E76B6B';
    } else {
        pill.style.borderColor = 'rgba(163, 131, 84, 0.2)';
        pill.style.background = 'var(--gold-pale)';
        prog.style.stroke = 'var(--gold)';
        text.style.color = 'var(--gold)';
    }
}

// ─── LÔ-GIC MY BOOKINGS ───
async function openBookingsModal() {
    const listHtml = document.getElementById('bookings-list');
    listHtml.innerHTML = '<div class="loading-bookings">Loading your reservations...</div>';
    document.getElementById('bookings-modal').classList.add('open');

    const { ok, status, data: reservations } = await callApi('/reservations/my');

    if (!ok) {
        listHtml.innerHTML = `<p class="error-state">Failed to load reservations. (Error: ${status})</p>`;
        return;
    }

    if (!reservations || reservations.length === 0) {
        listHtml.innerHTML = '<p class="empty-state">You have no reservations yet.</p>';
        return;
    }

    listHtml.innerHTML = '';
    const now = new Date();
    
    reservations.forEach(r => {
        const start = new Date(r.startTime);
        const dateStr = start.toLocaleDateString('en-US', { weekday: 'short', day: 'numeric', month: 'short' });
        const timeStr = r.startTime.split('T')[1].substring(0, 5);
        
        // Điều kiện hủy: Phải là RESERVED và cách hiện tại ít nhất 3 tiếng
        const canCancel = r.status === 'RESERVED' && (start.getTime() - now.getTime() > 3 * 60 * 60 * 1000);

        let statusClass = 'st-default';
        if (r.status === 'RESERVED') statusClass = 'st-reserved';
        if (r.status === 'CANCELLED' || r.status === 'NO_SHOW') statusClass = 'st-cancelled';
        if (r.status === 'COMPLETED' || r.status === 'SEATED') statusClass = 'st-completed';

        listHtml.innerHTML += `
        <div class="booking-item">
          <div class="b-header">
            <p class="b-ref">#${r.reservationId}</p>
            <span class="status-badge ${statusClass}">${r.status}</span>
          </div>
          <div class="b-body">
            <p class="b-detail"><strong>Date:</strong> ${dateStr} at ${timeStr}</p>
            <p class="b-detail"><strong>Guests:</strong> ${r.guestCount} people</p>
            ${r.note ? `<p class="b-note">"${r.note}"</p>` : ''}
          </div>
          <div class="b-footer">
            ${canCancel ? `<button class="btn-cancel-small" onclick="cancelMyBooking(${r.reservationId})">Cancel Reservation</button>` : ''}
            ${r.status === 'RESERVED' && !canCancel ? '<span class="cancel-deadline">Cancellation closed (within 3h)</span>' : ''}
          </div>
        </div>
      `;
    });
}

function closeBookingsModal() {
    document.getElementById('bookings-modal').classList.remove('open');
}

async function cancelMyBooking(id) {
    if (!confirm(`Are you sure you want to cancel reservation #${id}?`)) return;

    const { ok, data } = await callApi(`/reservations/${id}`, 'DELETE');
    if (ok) {
        alert("Reservation cancelled successfully.");
        openBookingsModal(); // Refresh list
    } else {
        alert(data?.message || "Failed to cancel reservation.");
    }
}

// ─── CÁC BƯỚC ĐẶT BÀN ───
function setParty(n){
    n=Math.max(1,Math.min(30,parseInt(n)||1)); S.party=n;
    const pInput = document.getElementById('party-val');
    if(pInput) pInput.value=n;
    document.querySelectorAll('.party-chip').forEach(c=>{
        const v=parseInt(c.dataset.v);
        c.classList.toggle('sel', v===n||(c.dataset.v==='8'&&n>=8));
    });
    updateSummary();
}

function confirmParty(){
    const sv=document.getElementById('sv-1');
    sv.textContent=`${S.party} ${S.party===1?'Guest':'Guests'}`; sv.classList.remove('ph');
    doneStep(1); openStep(2);
}

function getDs(d){ return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`; }

function renderDates(){
    const grid=document.getElementById('date-grid'), lbl=document.getElementById('date-nav-lbl'), o=S.week;
    if(!grid) return;
    grid.innerHTML='';
    const ws=new Date(thisMonday); ws.setDate(thisMonday.getDate()+o*7);
    const we=new Date(ws); we.setDate(ws.getDate()+6);
    lbl.textContent=o===0?'This Week':o===1?'Next Week':`${MONTHS[ws.getMonth()]} ${ws.getDate()} – ${MONTHS[we.getMonth()]} ${we.getDate()}`;
    document.getElementById('prev-week').disabled=o<=0;
    document.getElementById('next-week').disabled=o>=S.MAX_W;

    for(let i=0;i<7;i++){
        const d=new Date(ws); d.setDate(ws.getDate()+i);
        const ds=getDs(d), past=d<today, isTod=d.getTime()===today.getTime();
        const btn=document.createElement('button');
        btn.className='d-chip'+(isTod?' today':'')+(S.dateStr===ds?' sel':'');
        btn.disabled=past;
        btn.innerHTML=`<span class="dn">${DAYS[d.getDay()]}</span><span class="dd">${d.getDate()}</span>`;
        if(!past) btn.addEventListener('click',()=>{
            S.date=d; S.dateStr=ds;
            document.querySelectorAll('.d-chip').forEach(c=>c.classList.remove('sel'));
            btn.classList.add('sel');
            const cta=document.getElementById('date-cta');
            cta.disabled=false; cta.innerHTML='Continue <span class="arr">→</span>';
            updateSummary(); renderTimes();
        });
        grid.appendChild(btn);
    }
}

const LUNCH  = ['12:00','12:30','13:00','13:30','14:00','14:30','15:00'];
const DINNER = ['18:00','18:30','19:00','19:30','20:00','20:30','21:00','21:30','22:00','22:30'];

function renderTimes(){ 
    fillSlots('tg-lunch',LUNCH); 
    fillSlots('tg-dinner',DINNER); 
}

async function fillSlots(id, slots) {
    const grid = document.getElementById(id);
    if (!grid || !S.dateStr) return;
    grid.innerHTML = '<div class="loading-slots">Checking availability...</div>';

    try {
        // Gọi API kiểm tra giờ (có bàn trống đáp ứng slot)
        const { ok, data } = await callApi(`/reservations/availability/slots?date=${S.dateStr}&guests=${S.party}&slots=${slots.join(',')}`);
        
        grid.innerHTML = '';
        slots.forEach((slot) => {
            const isAvailable = ok && data && data[slot] === true;
            const btn = document.createElement('button');
            btn.className = 't-chip' + (!isAvailable ? ' bkd' : '') + (S.time === slot ? ' sel' : '');
            btn.disabled = !isAvailable;
            btn.innerHTML = `<span>${slot}</span>`;
            
            if (isAvailable) {
                btn.addEventListener('click', () => {
                    S.time = slot;
                    document.querySelectorAll('.t-chip').forEach(c => c.classList.remove('sel'));
                    btn.classList.add('sel');
                    const cta = document.getElementById('time-cta');
                    cta.disabled = false;
                    cta.innerHTML = 'Continue <span class="arr">→</span>';
                    updateSummary();
                });
            }
            grid.appendChild(btn);
        });
    } catch (err) {
        console.error("Failed to fetch slots:", err);
        grid.innerHTML = '<div class="error-slots">Error loading slots</div>';
    }
}

function confirmDate(){
    if(!S.dateStr)return;
    const sv=document.getElementById('sv-2');
    sv.textContent=`${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}`; sv.classList.remove('ph');
    doneStep(2); openStep(3); renderTimes();
}

function confirmTime(){
    if(!S.time)return;
    const sv=document.getElementById('sv-3'); sv.textContent=S.time; sv.classList.remove('ph');
    doneStep(3); openStep(4);
    loadStep4AutoFill();
}

function loadStep4AutoFill() {
    const user = getStoredUser();
    const nameInput = document.getElementById('guest-name');
    const emailInput = document.getElementById('guest-email');
    const phoneInput = document.getElementById('guest-phone');

    if (nameInput) nameInput.value = user.name || "";
    if (emailInput) emailInput.value = user.email || "";
    if (phoneInput) phoneInput.value = user.phone || "";
}

function doneStep(n){ const el=document.getElementById(`step-${n}`); el.classList.remove('active'); el.classList.add('completed'); setProg(n,'done'); }

function openStep(n){
    const el=document.getElementById(`step-${n}`); el.classList.remove('completed'); el.classList.add('active'); setProg(n,'active');
    setTimeout(()=>el.scrollIntoView({behavior:'smooth',block:'nearest'}),80);
    if(n===2)renderDates(); if(n===3&&S.dateStr)renderTimes();
}

function editStep(n){
    for(let i=n;i<=4;i++){ const el=document.getElementById(`step-${i}`); el.classList.remove('active','completed'); setProg(i,''); }
    if(n<=2){S.date=null;S.dateStr=null;} if(n<=3){S.time=null;}
    document.getElementById('proceed-btn').classList.remove('ready');
    openStep(n); updateSummary();
}

function setProg(n,st){ const el=document.querySelector(`.prog-item[data-s="${n}"]`); if(!el)return; el.classList.remove('active','done'); if(st)el.classList.add(st); }

function updateSummary(){
    document.getElementById('sum-guests').innerHTML = S.party ? `${S.party} ${S.party===1?'Guest':'Guests'}` : '<span class="empty">Not set</span>';
    document.getElementById('sum-date').innerHTML   = S.date  ? `${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}` : '<span class="empty">Not set</span>';
    document.getElementById('sum-time').innerHTML   = S.time  ? S.time : '<span class="empty">Not set</span>';
    document.getElementById('sum-dep').innerHTML    = S.party ? `${(S.party*50000).toLocaleString()} VND` : '<span class="empty">—</span>';
    if(S.party&&S.dateStr&&S.time) document.getElementById('proceed-btn').classList.add('ready');
}

async function goToPayment(){
    // Gom Occasion và Special Request thành Note
    const occasions = Array.from(document.querySelectorAll('.occ-chip.sel')).map(c => c.dataset.v);
    const specialReq = document.getElementById('guest-note').value.trim();
    
    let fullNote = "";
    if (occasions.length > 0) {
        fullNote = "Occasions: " + occasions.join(', ');
    }
    if (specialReq) {
        fullNote = fullNote ? fullNote + " | Notes: " + specialReq : specialReq;
    }

    const bookingData = {
        startTime: `${S.dateStr}T${S.time}:00`,
        guestCount: S.party,
        note: fullNote
    };

    // Gọi API Đặt bàn Online (Backend sẽ soft-lock bàn)
    const { ok, data } = await callApi('/reservations/online', 'POST', bookingData);
    if (!ok) {
        alert(data?.message || "Failed to create reservation. The slot might have been taken.");
        return;
    }

    S.reservationId = data.reservationId;

    document.getElementById('booking-page').style.display='none';
    document.getElementById('payment-page').style.display='block';
    window.scrollTo(0,0);

    startTimer();

    const dep = data.depositAmount || (S.party * 50000); // Backend dùng VND
    document.getElementById('p-guests').textContent=`Deposit (${S.party} guest${S.party>1?'s':''} × ${dep/S.party} VND)`;
    document.getElementById('p-deposit').textContent=`${dep.toLocaleString()} VND`;
    document.getElementById('p-total').textContent=`${dep.toLocaleString()} VND`;
    if(S.date) document.getElementById('p-date').textContent=`${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}`;
    if(S.time) document.getElementById('p-time').textContent=S.time;
}

function backToBooking(){
    stopTimer();
    document.getElementById('booking-page').style.display='block';
    document.getElementById('payment-page').style.display='none';
    window.scrollTo(0,0);
}

async function confirmReservation(){
    if (!S.reservationId) return;

    // Gọi API xác nhận thanh toán (Backend sẽ chốt bàn vào Mapping)
    const { ok, data } = await callApi(`/reservations/${S.reservationId}/payment/confirm`, 'POST');
    
    if (!ok) {
        alert(data?.message || "Payment confirmation failed.");
        return;
    }

    stopTimer();
    document.getElementById('payment-page').style.display='none';
    const sp=document.getElementById('success-page'); sp.style.display='flex';
    window.scrollTo(0,0);

    const refId = 'RS-' + S.reservationId;
    document.getElementById('success-ref').textContent='Reservation · ' + refId;

    myBookingsList.push({
        ref: refId,
        date: `${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}`,
        time: S.time,
        party: S.party
    });
}

// ─── KHỞI TẠO DOM ───
document.addEventListener("DOMContentLoaded", () => {
    // Gắn sự kiện các nút
    const pInput = document.getElementById('party-val');
    if(pInput) {
        pInput.addEventListener('input',()=>setParty(pInput.value));
    }

    const minusBtn = document.getElementById('minus-btn');
    const plusBtn = document.getElementById('plus-btn');
    if(minusBtn) minusBtn.addEventListener('click',()=>setParty(S.party-1));
    if(plusBtn) plusBtn.addEventListener('click',()=>setParty(S.party+1));

    document.querySelectorAll('.party-chip').forEach(c=>c.addEventListener('click',()=>setParty(c.dataset.v)));

    const prevWeekBtn = document.getElementById('prev-week');
    const nextWeekBtn = document.getElementById('next-week');
    if(prevWeekBtn) prevWeekBtn.addEventListener('click',()=>{S.week--;renderDates();});
    if(nextWeekBtn) nextWeekBtn.addEventListener('click',()=>{S.week++;renderDates();});

    const cardNum = document.getElementById('card-num');
    if(cardNum) cardNum.addEventListener('input',function(){ this.value=this.value.replace(/\D/g,'').replace(/(\d{4})(?=\d)/g,'$1 ').substring(0,19); });

    const logoutBtn = document.getElementById('logout-btn');
    if(logoutBtn) logoutBtn.addEventListener('click',e=>{ e.preventDefault(); clearUser(); window.location.href='index.html'; });

    // Click listener cho Occasion chips
    document.querySelectorAll('.occ-chip').forEach(c => {
        c.addEventListener('click', () => {
            c.classList.toggle('sel');
        });
    });

    // Cài đặt trạng thái ban đầu
    setParty(2);
    updateSummary();
});