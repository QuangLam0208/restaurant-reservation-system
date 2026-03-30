using reservation_winforms.DTO.reservation;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcCheckOut : UserControl
    {
        private readonly ReservationService _reservationService;
        private ReservationResponse _currentSelectedRes = null;

        public UcCheckOut()
        {
            InitializeComponent();
            _reservationService = new ReservationService();

            btnRefresh.Click += BtnRefresh_Click;
            btnCheckOut.Click += BtnCheckOut_Click;

            // Tự động load dữ liệu khi mở Tab này
            this.Load += async (s, e) => await LoadActiveTablesAsync();

            ResetDetailsPanel();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadActiveTablesAsync();
        }

        // ==========================================================
        // 1. GỌI API LẤY DANH SÁCH BÀN ĐANG CÓ KHÁCH (SEATED)
        // ==========================================================
        private async Task LoadActiveTablesAsync()
        {
            btnRefresh.Enabled = false;
            btnRefresh.Text = "ĐANG TẢI...";

            var res = await _reservationService.GetActiveReservationsAsync();

            btnRefresh.Enabled = true;
            btnRefresh.Text = "LÀM MỚI";

            if (!res.IsSuccess || res.Data == null)
            {
                MessageBox.Show("Không thể tải danh sách bàn đang hoạt động.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RenderActiveTables(res.Data);
        }

        // ==========================================================
        // 2. VẼ DANH SÁCH CÁC THẺ (CARD) LÊN PANEL TRÁI
        // ==========================================================
        private void RenderActiveTables(List<ReservationResponse> activeReservations)
        {
            flpActiveTables.Controls.Clear();
            ResetDetailsPanel();

            if (activeReservations.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Hiện tại không có bàn nào đang hoạt động.",
                    Font = new Font("Segoe UI", 14, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                flpActiveTables.Controls.Add(lblEmpty);
                return;
            }

            foreach (var r in activeReservations)
            {
                string tableStr = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "N/A";
                TimeSpan duration = DateTime.Now - r.StartTime; // Tính thời gian khách đã ngồi
                int durationMins = (int)duration.TotalMinutes;

                // Nếu khách ngồi lố 2 tiếng (120 phút), hiện cảnh báo đỏ
                string durationText = durationMins > 120 ? $"⚠️ Quá giờ ({durationMins}p)" : $"{durationMins} phút";

                Button btnCard = new Button
                {
                    Text = $"🪑 BÀN {tableStr}\n\n👥 Khách: {r.CustomerName ?? "Vãng lai"}\n🕒 Ngồi: {durationText}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Width = 260,
                    Height = 150,
                    Margin = new Padding(10),
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    BackColor = Color.White,
                    Tag = r
                };

                // Thẻ nào ngồi lố giờ thì viền đỏ nhấp nháy, bình thường viền xanh dương
                btnCard.FlatAppearance.BorderColor = durationMins > 120 ? Color.FromArgb(231, 76, 60) : Color.FromArgb(41, 128, 185);
                btnCard.FlatAppearance.BorderSize = 2;

                // Sự kiện: Click vào thẻ nào thì thẻ đó chuyển màu xanh đậm và bật thông tin sang Panel phải
                btnCard.Click += (s, e) =>
                {
                    foreach (Control c in flpActiveTables.Controls)
                        if (c is Button b) { b.BackColor = Color.White; b.ForeColor = Color.Black; }

                    btnCard.BackColor = Color.FromArgb(41, 128, 185);
                    btnCard.ForeColor = Color.White;

                    _currentSelectedRes = (ReservationResponse)btnCard.Tag;
                    ShowReservationDetails(_currentSelectedRes);
                };

                flpActiveTables.Controls.Add(btnCard);
            }
        }

        // ==========================================================
        // 3. ĐẨY THÔNG TIN ĐƠN LÊN BẢNG BÊN PHẢI
        // ==========================================================
        private void ShowReservationDetails(ReservationResponse r)
        {
            pnlDetails.Visible = true;
            btnCheckOut.Visible = true;

            lblValTable.Text = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "N/A";
            lblValName.Text = r.CustomerName ?? "Khách vãng lai";
            lblValTime.Text = r.StartTime.ToString("HH:mm");

            TimeSpan duration = DateTime.Now - r.StartTime;
            int mins = (int)duration.TotalMinutes;
            lblValDuration.Text = $"{mins} phút";
            lblValDuration.ForeColor = mins > 120 ? Color.Red : Color.FromArgb(243, 156, 18);

            lblValDeposit.Text = (r.DepositAmount != null && r.DepositAmount > 0) ? $"{r.DepositAmount:N0}đ" : "0đ";
        }

        // ==========================================================
        // 4. THỰC HIỆN CHECK-OUT VÀ GIẢI PHÓNG BÀN
        // ==========================================================
        private async void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (_currentSelectedRes == null) return;

            string confirmMsg = $"Bạn xác nhận thanh toán và trả bàn cho Khách hàng: {_currentSelectedRes.CustomerName}?\n";
            if (_currentSelectedRes.DepositAmount > 0)
            {
                // Cảnh báo cực mạnh cho Thu ngân để không thu dư tiền của khách
                confirmMsg += $"\n💰 LƯU Ý: KHÁCH NÀY ĐÃ CỌC TRƯỚC, NHỚ TRỪ TIỀN CỌC: {_currentSelectedRes.DepositAmount:N0}đ VÀO HÓA ĐƠN!";
            }

            var confirm = MessageBox.Show(confirmMsg, "Xác nhận Trả Bàn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            btnCheckOut.Enabled = false;
            btnCheckOut.Text = "ĐANG XỬ LÝ...";

            var result = await _reservationService.CheckOutAsync(_currentSelectedRes.ReservationId);

            if (result.IsSuccess)
            {
                MessageBox.Show("Trả bàn thành công! Hệ thống đã giải phóng các bàn này để đón khách tiếp theo.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadActiveTablesAsync(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show(result.Message, "Lỗi Trả bàn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnCheckOut.Enabled = true;
                btnCheckOut.Text = "XÁC NHẬN TRẢ BÀN";
            }
        }

        private void ResetDetailsPanel()
        {
            pnlDetails.Visible = false;
            btnCheckOut.Visible = false;
            _currentSelectedRes = null;
        }
    }
}