using reservation_winforms.DTO.reservation;
using reservation_winforms.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcOnlineBooking : UserControl
    {
        private readonly ReservationService _reservationService;
        private readonly TableService _tableService;
        private ReservationResponse _currentReservation = null;

        public UcOnlineBooking()
        {
            InitializeComponent();
            _reservationService = new ReservationService();
            _tableService = new TableService();

            // Gắn sự kiện cho các nút
            btnSearch.Click += BtnSearch_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnCancelBooking.Click += BtnCancelBooking_Click;

            // Ẩn panel chi tiết lúc mới vào màn hình
            pnlDetails.Visible = false;
            btnCheckIn.Visible = false;
            btnCancelBooking.Visible = false;
        }

        // 1. TÌM KIẾM ĐƠN ĐẶT BÀN
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập Số điện thoại hoặc Mã đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSearch.Text = "ĐANG TÌM...";
            btnSearch.Enabled = false;

            var res = await _reservationService.GetUpcomingReservationsAsync(1440);

            btnSearch.Text = "TÌM KIẾM";
            btnSearch.Enabled = true;

            if (!res.IsSuccess || res.Data == null)
            {
                MessageBox.Show("Lỗi lấy dữ liệu từ máy chủ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tìm đơn khớp (Chỉ lấy đơn ONLINE, vì Walk-in không cần check-in lại)
            _currentReservation = res.Data.FirstOrDefault(r =>
                r.Type == "ONLINE" &&
                (r.CustomerPhone == keyword || r.ReservationId.ToString() == keyword));

            if (_currentReservation == null)
            {
                MessageBox.Show("Không tìm thấy đơn đặt trước (Online) chờ Check-in với thông tin này.\nKhách có thể đã bị hủy đơn (No-show) hoặc nhập sai số.",
                                "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pnlDetails.Visible = false;
                btnCheckIn.Visible = false;
                btnCancelBooking.Visible = false;
                return;
            }

            await ShowReservationInfo(_currentReservation);
        }

        // 2. HIỂN THỊ CHI TIẾT
        private async Task ShowReservationInfo(ReservationResponse r)
        {
            pnlDetails.Visible = true;
            btnCheckIn.Visible = true;
            btnCancelBooking.Visible = true;

            lblValName.Text = r.CustomerName ?? "Khách hàng";
            lblValPhone.Text = r.CustomerPhone ?? "N/A";
            lblValGuests.Text = $"{r.GuestCount} người";
            lblValTime.Text = r.StartTime.ToString("HH:mm");
            lblValTable.Text = r.TableIds != null && r.TableIds.Count > 0 ? string.Join(", ", r.TableIds) : "Chưa xếp bàn";

            // Đánh giá: Khách đến sớm hay trễ?
            TimeSpan diff = DateTime.Now - r.StartTime;
            if (diff.TotalMinutes < 0)
            {
                lblValTimeStatus.Text = $"(Đến sớm {Math.Abs((int)diff.TotalMinutes)} phút)";
                lblValTimeStatus.ForeColor = Color.MediumSeaGreen;
            }
            else
            {
                lblValTimeStatus.Text = $"(Đến trễ {(int)diff.TotalMinutes} phút)";
                // Nếu trễ quá 15 phút thì cảnh báo đỏ
                lblValTimeStatus.ForeColor = diff.TotalMinutes > 15 ? Color.Red : Color.Orange;
            }

            // Đánh giá: Bàn xếp cho khách có đang trống không?
            var mapRes = await _tableService.GetFloorMapAsync();
            if (mapRes.IsSuccess && r.TableIds != null)
            {
                var assignedTables = mapRes.Data.Where(t => r.TableIds.Contains(t.TableId)).ToList();
                bool isBusy = assignedTables.Any(t => t.Status == "OCCUPIED" || t.Status == "OVERSTAY");

                if (isBusy)
                {
                    lblValTableStatus.Text = "🔴 ĐANG CÓ KHÁCH (Sẽ tự tìm bàn thay thế khi Check-in)";
                    lblValTableStatus.ForeColor = Color.IndianRed;
                }
                else
                {
                    lblValTableStatus.Text = "🟢 TRỐNG - Sẵn sàng đón khách";
                    lblValTableStatus.ForeColor = Color.MediumSeaGreen;
                }
            }
        }

        // 3. CHECK-IN (NHẬN BÀN)
        private async void BtnCheckIn_Click(object sender, EventArgs e)
        {
            if (_currentReservation == null) return;

            btnCheckIn.Enabled = false;
            btnCheckIn.Text = "ĐANG XỬ LÝ...";

            var result = await _reservationService.CheckInAsync(_currentReservation.ReservationId);

            if (result.IsSuccess)
            {
                MessageBox.Show($"Check-in thành công!\nĐã xác nhận khách đến bàn.",
                                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetUI();
            }
            else
            {
                // Bắt gọn lỗi ném ra từ Backend (VD: Khách No-show do đến trễ quá 15p, hoặc Hết bàn thay thế)
                MessageBox.Show(result.Message, "Không thể Check-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSearch.PerformClick(); // Tải lại data mới nhất
            }

            btnCheckIn.Enabled = true;
            btnCheckIn.Text = "CHECK-IN";
        }

        // 4. HỦY ĐƠN (LỄ TÂN THAO TÁC)
        private async void BtnCancelBooking_Click(object sender, EventArgs e)
        {
            if (_currentReservation == null) return;

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn hủy đơn của khách '{_currentReservation.CustomerName}' không?\nTiền cọc ({_currentReservation.DepositAmount:N0}đ) có thể không được hoàn lại tùy theo chính sách.",
                                          "Xác nhận Hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var result = await _reservationService.CancelReservationAsync(_currentReservation.ReservationId);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Đã hủy đơn thành công. Bàn đã được giải phóng.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetUI();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi Hủy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetUI()
        {
            pnlDetails.Visible = false;
            btnCheckIn.Visible = false;
            btnCancelBooking.Visible = false;
            txtSearch.Text = "";
            _currentReservation = null;
        }
    }
}