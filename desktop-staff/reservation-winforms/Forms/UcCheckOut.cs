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
        private readonly OverrideService _overrideService;
        private ReservationResponse _currentSelectedRes = null;

        public UcCheckOut()
        {
            InitializeComponent();
            _reservationService = new ReservationService();
            _overrideService = new OverrideService();

            btnRefresh.Click += BtnRefresh_Click;
            btnCheckOut.Click += BtnCheckOut_Click;

            this.Load += async (s, e) => await LoadActiveTablesAsync();

            ResetDetailsPanel();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadActiveTablesAsync();
        }

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

                btnCard.FlatAppearance.BorderColor = durationMins > 120 ? Color.FromArgb(231, 76, 60) : Color.FromArgb(41, 128, 185);
                btnCard.FlatAppearance.BorderSize = 2;

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

        private void ShowReservationDetails(ReservationResponse r)
        {
            pnlDetails.Visible = true;
            btnCheckOut.Visible = true;
            chkOverride.Visible = true;
            chkOverride.Checked = false;

            lblValTable.Text = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "N/A";
            lblValName.Text = r.CustomerName ?? "Khách vãng lai";
            lblValTime.Text = r.StartTime.ToString("HH:mm");

            TimeSpan duration = DateTime.Now - r.StartTime;
            int mins = (int)duration.TotalMinutes;
            lblValDuration.Text = $"{mins} phút";
            lblValDuration.ForeColor = mins > 120 ? Color.Red : Color.FromArgb(243, 156, 18);

            lblValDeposit.Text = (r.DepositAmount != null && r.DepositAmount > 0) ? $"{r.DepositAmount:N0}đ" : "0đ";
            chkOverride.Checked = false;
            if (mins > 120)
            {
                chkOverride.Visible = true;
            }
            else
            {
                chkOverride.Visible = false;
            }
        }

        private async void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (_currentSelectedRes == null) return;

            if (chkOverride.Checked)
            {
                using (var frm = new OverrideDialog())
                {
                    frm.SetWarningMessage($"Bàn của khách {_currentSelectedRes.CustomerName} đang bị quá giờ (Overstay).\nBạn phải nhập lý do xử lý để ghi đè hệ thống.");

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            btnCheckOut.Enabled = false;
                            btnCheckOut.Text = "ĐANG GHI ĐÈ...";

                            var res = await _overrideService.OverrideReservationAsync(_currentSelectedRes.ReservationId, frm.Reason);

                            if (res.IsSuccess)
                            {
                                MessageBox.Show("Đã thanh toán cưỡng chế, lưu log và giải phóng bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadActiveTablesAsync(); // Tải lại danh sách
                            }
                            else
                            {
                                MessageBox.Show(res.Message, "Lỗi ghi đè", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        finally
                        {
                            btnCheckOut.Enabled = true;
                            btnCheckOut.Text = "XÁC NHẬN TRẢ BÀN";
                        }
                    }
                }

                return;
            }

            string confirmMsg = $"Bạn xác nhận thanh toán và trả bàn cho Khách hàng: {_currentSelectedRes.CustomerName}?\n";
            if (_currentSelectedRes.DepositAmount > 0)
            {
                confirmMsg += $"\n💰 LƯU Ý: KHÁCH NÀY ĐÃ CỌC TRƯỚC, NHỚ TRỪ TIỀN CỌC: {_currentSelectedRes.DepositAmount:N0}đ VÀO HÓA ĐƠN!";
            }

            var confirm = MessageBox.Show(confirmMsg, "Xác nhận Trả Bàn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                btnCheckOut.Enabled = false;
                btnCheckOut.Text = "ĐANG XỬ LÝ...";

                var result = await _reservationService.CheckOutAsync(_currentSelectedRes.ReservationId);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Trả bàn thành công! Hệ thống đã giải phóng các bàn này để đón khách tiếp theo.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadActiveTablesAsync();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi Trả bàn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnCheckOut.Enabled = true;
                btnCheckOut.Text = "XÁC NHẬN TRẢ BÀN";
            }
        }

        private void ResetDetailsPanel()
        {
            pnlDetails.Visible = false;
            btnCheckOut.Visible = false;
            _currentSelectedRes = null;
            chkOverride.Visible = false;
            chkOverride.Checked = false;
        }
    }
}