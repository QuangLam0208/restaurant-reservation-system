const S = { party:2, date:null, dateStr:null, time:null, week:0, MAX_W:4 };
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
function openBookingsModal() {
    const listHtml = document.getElementById('bookings-list');
    listHtml.innerHTML = '';

    if (myBookingsList.length === 0) {
        listHtml.innerHTML = '<p class="empty-state">You have no upcoming reservations.</p>';
    } else {
        myBookingsList.forEach(b => {
            listHtml.innerHTML += `
        <div class="booking-item">
          <p class="b-ref">${b.ref}</p>
          <p class="b-detail"><strong>Date:</strong> ${b.date} at ${b.time}</p>
          <p class="b-detail"><strong>Guests:</strong> ${b.party} people</p>
        </div>
      `;
        });
    }
    document.getElementById('bookings-modal').classList.add('open');
}

function closeBookingsModal() {
    document.getElementById('bookings-modal').classList.remove('open');
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

function renderTimes(){ fillSlots('tg-lunch',LUNCH); fillSlots('tg-dinner',DINNER); }

function fillSlots(id,slots){
    const grid=document.getElementById(id);
    if(!grid) return;
    grid.innerHTML='';
    const seed=S.dateStr?S.dateStr.charCodeAt(S.dateStr.length-1):0;
    slots.forEach((slot,i)=>{
        const booked=(seed+i)%3===0;
        const btn=document.createElement('button');
        btn.className='t-chip'+(booked?' bkd':'')+(S.time===slot?' sel':'');
        btn.disabled=booked;
        btn.innerHTML=`<span>${slot}</span>`;
        if(!booked) btn.addEventListener('click',()=>{
            S.time=slot;
            document.querySelectorAll('.t-chip').forEach(c=>c.classList.remove('sel'));
            btn.classList.add('sel');
            const cta=document.getElementById('time-cta');
            cta.disabled=false; cta.innerHTML='Continue <span class="arr">→</span>';
            updateSummary();
        });
        grid.appendChild(btn);
    });
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
    document.getElementById('sum-dep').innerHTML    = S.party ? `€${S.party*15}` : '<span class="empty">—</span>';
    if(S.party&&S.dateStr&&S.time) document.getElementById('proceed-btn').classList.add('ready');
}

function goToPayment(){
    document.getElementById('booking-page').style.display='none';
    document.getElementById('payment-page').style.display='block';
    window.scrollTo(0,0);

    startTimer();

    const dep=S.party*15;
    document.getElementById('p-guests').textContent=`Deposit (${S.party} guest${S.party>1?'s':''} × €15)`;
    document.getElementById('p-deposit').textContent=`€${dep}`;
    document.getElementById('p-total').textContent=`€${dep}`;
    if(S.date) document.getElementById('p-date').textContent=`${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}`;
    if(S.time) document.getElementById('p-time').textContent=S.time;
}

function backToBooking(){
    stopTimer();
    document.getElementById('booking-page').style.display='block';
    document.getElementById('payment-page').style.display='none';
    window.scrollTo(0,0);
}

function confirmReservation(){
    stopTimer();
    document.getElementById('payment-page').style.display='none';
    const sp=document.getElementById('success-page'); sp.style.display='flex';
    window.scrollTo(0,0);

    const refId = 'SL-'+Math.random().toString(36).substring(2,8).toUpperCase();
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
    if(logoutBtn) logoutBtn.addEventListener('click',e=>{ e.preventDefault(); localStorage.removeItem('isLoggedIn'); window.location.href='index.html'; });

    // Cài đặt trạng thái ban đầu
    setParty(2);
    updateSummary();
});