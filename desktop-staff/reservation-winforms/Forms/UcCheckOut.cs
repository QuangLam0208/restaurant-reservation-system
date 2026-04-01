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
            btnChangeTable.Click += async (s, e) => await HandleChangeTableAsync();

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
                TimeSpan duration = DateTime.Now - r.StartTime;
                int durationMins = (int)duration.TotalMinutes;

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
            btnChangeTable.Visible = true;

            lblValTable.Text = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "N/A";
            lblValName.Text = r.CustomerName ?? "Khách vãng lai";
            lblValTime.Text = r.StartTime.ToString("HH:mm");

            TimeSpan duration = DateTime.Now - r.StartTime;
            int mins = (int)duration.TotalMinutes;
            lblValDuration.Text = $"{mins} phút";
            lblValDuration.ForeColor = mins > 120 ? Color.Red : Color.FromArgb(243, 156, 18);

            lblValDeposit.Text = (r.DepositAmount != null && r.DepositAmount > 0) ? $"{r.DepositAmount:N0}đ" : "0đ";

            // Hiện checkbox override nếu quá giờ
            chkOverride.Checked = false;
            chkOverride.Visible = (mins > 120);
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
                                await LoadActiveTablesAsync();
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

        private async Task HandleChangeTableAsync()
        {
            if (_currentSelectedRes == null)
            {
                MessageBox.Show("Vui lòng chọn một bàn đang có khách để thực hiện đổi bàn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnChangeTable.Enabled = false;
                btnChangeTable.Text = "ĐANG TẢI...";

                int guestCount = _currentSelectedRes.GuestCount;

                var optionsRes = await _reservationService.GetWalkInOptionsAsync(guestCount);

                if (!optionsRes.IsSuccess || optionsRes.Data == null || optionsRes.Data.Groups == null || optionsRes.Data.Groups.Count == 0)
                {
                    MessageBox.Show("Nhà hàng hiện không có tổ hợp bàn nào trống phù hợp cho số lượng khách này!", "Hết bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedIds = ShowOptionsDialog(optionsRes.Data, guestCount);
                if (selectedIds == null || selectedIds.Count == 0) return;

                var changeReq = new ChangeTableRequest
                {
                    TableIds = selectedIds
                };

                btnChangeTable.Text = "ĐANG CHUYỂN...";
                var res = await _reservationService.ChangeTableAsync(_currentSelectedRes.ReservationId, changeReq);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Đã chuyển bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    long savedResId = _currentSelectedRes.ReservationId;
                    await LoadActiveTablesAsync();

                    foreach (Control c in flpActiveTables.Controls)
                    {
                        if (c is Button btnCard && btnCard.Tag is ReservationResponse rObj && rObj.ReservationId == savedResId)
                        {
                            btnCard.PerformClick();
                            flpActiveTables.ScrollControlIntoView(btnCard);
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(res.Message, "Không thể chuyển bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                btnChangeTable.Enabled = true;
                btnChangeTable.Text = "ĐỔI BÀN";
            }
        }

        private List<long> ShowOptionsDialog(WalkInOptionResponse data, int guestCount)
        {
            List<long> selectedIds = null;

            Form popup = new Form
            {
                Text = $"Gợi ý đổi bàn cho nhóm {guestCount} khách",
                Size = new Size(550, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };
            popup.Controls.Add(flp);

            foreach (var group in data.Groups)
            {
                Label lblGroup = new Label
                {
                    Text = group.GroupName,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    AutoSize = true,
                    Margin = new Padding(0, 10, 0, 5),
                    ForeColor = group.GroupName.Contains("Ưu tiên") ? Color.MediumSeaGreen : Color.Orange
                };
                flp.Controls.Add(lblGroup);

                foreach (var opt in group.Options)
                {
                    string tableStr = string.Join(", ", opt.TableIds);
                    string typeStr = opt.TableIds.Count > 1 ? "Ghép bàn" : "Bàn đơn";
                    string limitStr = opt.AvailableUntil.HasValue ? $" | Phải trả bàn lúc: {opt.AvailableUntil.Value:HH:mm}" : "";

                    string btnText = $"Bàn {tableStr} ({opt.TotalCapacity} chỗ) - {typeStr}{limitStr}";

                    Button btnOpt = new Button
                    {
                        Text = btnText,
                        Width = 490,
                        Height = 45,
                        Margin = new Padding(5),
                        TextAlign = ContentAlignment.MiddleLeft,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };

                    btnOpt.FlatAppearance.BorderColor = opt.AvailableUntil.HasValue ? Color.Orange : Color.LightGray;

                    btnOpt.Click += (s, e) =>
                    {
                        var confirmResult = MessageBox.Show(
                            $"Bạn có chắc chắn muốn đổi sang Bàn {tableStr} không?",
                            "Xác nhận đổi bàn",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (confirmResult == DialogResult.Yes)
                        {
                            selectedIds = opt.TableIds;
                            popup.DialogResult = DialogResult.OK;
                            popup.Close();
                        }
                    };

                    flp.Controls.Add(btnOpt);
                }
            }

            popup.ShowDialog();
            return selectedIds;
        }

        private void ResetDetailsPanel()
        {
            pnlDetails.Visible = false;
            btnCheckOut.Visible = false;
            btnChangeTable.Visible = false;
            _currentSelectedRes = null;
            chkOverride.Visible = false;
            chkOverride.Checked = false;
        }
    }
}