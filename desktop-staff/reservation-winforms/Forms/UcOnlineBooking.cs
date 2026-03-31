using reservation_winforms.DTO.reservation;
using reservation_winforms.DTO.table;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
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

        private List<ReservationResponse> _allTodayReservations = new List<ReservationResponse>();

        private ReservationResponse _currentReservation = null;

        public UcOnlineBooking()
        {
            InitializeComponent();
            _reservationService = new ReservationService();
            _tableService = new TableService();

            btnSearch.Click += BtnSearch_Click;
            btnReload.Click += BtnReload_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnChangeTable.Click += BtnChangeTable_Click;

            ResetUI();

            this.Load += async (s, e) => await LoadTodayReservationsAsync();
        }

        private async Task LoadTodayReservationsAsync()
        {
            btnReload.Text = "ĐANG TẢI...";
            btnReload.Enabled = false;
            ResetUI();

            var res = await _reservationService.GetUpcomingReservationsAsync(1440); // 1440 phút = 24h

            if (res.IsSuccess && res.Data != null)
            {
                _allTodayReservations = res.Data
                    .Where(r => r.Type == "ONLINE" && r.StartTime.Date == DateTime.Today)
                    .OrderBy(r => r.StartTime)
                    .ToList();

                RenderReservationCards(_allTodayReservations);
            }
            else
            {
                MessageBox.Show("Không thể tải danh sách đơn đặt bàn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnReload.Text = "🔄 TẢI LẠI";
            btnReload.Enabled = true;
        }

        private void RenderReservationCards(List<ReservationResponse> listToRender)
        {
            flpReservations.Controls.Clear();

            if (listToRender.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Không có đơn đặt trước nào phù hợp.",
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                flpReservations.Controls.Add(lblEmpty);
                return;
            }

            foreach (var r in listToRender)
            {
                string timeStr = r.StartTime.ToString("HH:mm");
                string tableStr = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "Chưa xếp";

                Button btnCard = new Button
                {
                    Text = $"Giờ đến: {timeStr}\nKhách: {r.CustomerName} ({r.CustomerPhone})\nBàn: {tableStr} - ({r.GuestCount} người)",
                    Width = flpReservations.Width - 25,
                    Height = 100,
                    Margin = new Padding(5, 5, 5, 10),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 12),
                    BackColor = Color.WhiteSmoke,
                    Cursor = Cursors.Hand,
                    Tag = r
                };

                btnCard.FlatAppearance.BorderColor = Color.LightGray;

                btnCard.Click += async (s, e) =>
                {
                    foreach (Control c in flpReservations.Controls)
                    {
                        if (c is Button b)
                        {
                            b.BackColor = Color.WhiteSmoke;
                            b.ForeColor = Color.Black;
                        }
                    }

                    btnCard.BackColor = Color.FromArgb(41, 128, 185);
                    btnCard.ForeColor = Color.White;

                    _currentReservation = r;
                    await ShowReservationInfo(r);
                };

                flpReservations.Controls.Add(btnCard);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập Số điện thoại hoặc Mã đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filteredList = _allTodayReservations.Where(r =>
                r.CustomerPhone.Contains(keyword) ||
                r.ReservationId.ToString() == keyword).ToList();

            ResetUI();
            RenderReservationCards(filteredList);
        }

        private async void BtnReload_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            await LoadTodayReservationsAsync();
        }

        private async Task ShowReservationInfo(ReservationResponse r)
        {
            pnlDetails.Visible = true;
            btnCheckIn.Visible = true;
            btnChangeTable.Visible = true;

            lblValName.Text = r.CustomerName ?? "Khách hàng";
            lblValPhone.Text = r.CustomerPhone ?? "N/A";
            lblValGuests.Text = $"{r.GuestCount} người";
            lblValTime.Text = r.StartTime.ToString("HH:mm");
            lblValTable.Text = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "Chưa xếp bàn";
            lblValNote.Text = string.IsNullOrEmpty(r.Note) ? "Không có ghi chú nào." : r.Note;

            TimeSpan diff = DateTime.Now - r.StartTime;
            if (diff.TotalMinutes < 0)
            {
                lblValTimeStatus.Text = $"(Đến sớm {Math.Abs((int)diff.TotalMinutes)} phút)";
                lblValTimeStatus.ForeColor = Color.MediumSeaGreen;
            }
            else
            {
                lblValTimeStatus.Text = $"(Đến trễ {(int)diff.TotalMinutes} phút)";
                lblValTimeStatus.ForeColor = diff.TotalMinutes > 15 ? Color.Red : Color.Orange;
            }

            var mapRes = await _tableService.GetFloorMapAsync();
            if (mapRes.IsSuccess && r.TableIds != null)
            {
                var assignedTables = mapRes.Data.Where(t => r.TableIds.Contains(t.TableId)).ToList();
                bool isBusy = assignedTables.Any(t => t.Status == "OCCUPIED" || t.Status == "OVERSTAY");

                if (isBusy)
                {
                    lblValTableStatus.Text = "🔴 ĐANG CÓ KHÁCH (Tìm bàn khác)";
                    lblValTableStatus.ForeColor = Color.IndianRed;
                }
                else
                {
                    lblValTableStatus.Text = "🟢 TRỐNG - Sẵn sàng";
                    lblValTableStatus.ForeColor = Color.MediumSeaGreen;
                }
            }
        }

        private async void BtnCheckIn_Click(object sender, EventArgs e)
        {
            if (_currentReservation == null) return;

            try
            {
                btnCheckIn.Enabled = false;
                btnCheckIn.Text = "ĐANG XỬ LÝ...";

                var result = await _reservationService.CheckInAsync(_currentReservation.ReservationId);

                if (result.IsSuccess)
                {
                    MessageBox.Show($"Check-in thành công!\nĐã xác nhận khách đến bàn.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadTodayReservationsAsync();
                }
                else MessageBox.Show(result.Message, "Không thể Check-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnCheckIn.Enabled = true;
                btnCheckIn.Text = "CHECK-IN";
            }
        }

        //private async void BtnChangeTable_Click(object sender, EventArgs e)
        //{
        //    if (_currentReservation == null) return;

        //    try
        //    {
        //        btnChangeTable.Enabled = false;
        //        btnChangeTable.Text = "ĐANG TẢI...";

        //        var mapRes = await _tableService.GetFloorMapAsync();

        //        if (!mapRes.IsSuccess || mapRes.Data == null)
        //        {
        //            MessageBox.Show("Không thể tải sơ đồ bàn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        var availableTables = mapRes.Data.Where(t => t.IsActive && t.Status == "AVAILABLE" && t.CurrentReservationStatus != "RESERVED").ToList();

        //        if (availableTables.Count == 0)
        //        {
        //            MessageBox.Show("Nhà hàng hiện tại không còn bàn trống nào để đổi!", "Hết bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        var changeReq = ShowChangeTableDialog(availableTables, _currentReservation.GuestCount);
        //        if (changeReq == null) return;

        //        btnChangeTable.Text = "ĐANG CHUYỂN...";
        //        var res = await _reservationService.ChangeTableAsync(_currentReservation.ReservationId, changeReq);
        //        if (res.IsSuccess)
        //        {
        //            MessageBox.Show("Đã chuyển bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //            long savedResId = _currentReservation.ReservationId;

        //            await LoadTodayReservationsAsync();

        //            foreach (Control c in flpReservations.Controls)
        //            {
        //                if (c is Button btnCard && btnCard.Tag is ReservationResponse rObj && rObj.ReservationId == savedResId)
        //                {
        //                    btnCard.PerformClick();
        //                    flpReservations.ScrollControlIntoView(btnCard);
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show(res.Message, "Không thể chuyển bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //    finally
        //    {
        //        btnChangeTable.Enabled = true;
        //        btnChangeTable.Text = "ĐỔI BÀN";
        //    }
        //}

        //private ChangeTableRequest ShowChangeTableDialog(List<FloorMapTableResponse> availableTables, int guestCount)
        //{
        //    ChangeTableRequest result = null;
        //    List<long> selectedIds = new List<long>();
        //    int totalCapacity = 0;

        //    Form popup = new Form { Text = $"Đổi bàn cho {guestCount} khách", Size = new Size(650, 480), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = Color.White };
        //    popup.Controls.Add(new Label { Text = $"Chọn bàn mới (Cần chứa đủ {guestCount} khách):", AutoSize = true, Location = new Point(20, 20), Font = new Font("Segoe UI", 12, FontStyle.Bold) });

        //    FlowLayoutPanel flp = new FlowLayoutPanel { Location = new Point(20, 60), Size = new Size(590, 300), AutoScroll = true, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10) };
        //    popup.Controls.Add(flp);

        //    Label lblStatus = new Label { Text = "Đã chọn: 0 bàn (0 chỗ)", AutoSize = true, Location = new Point(20, 380), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.MediumSeaGreen };
        //    popup.Controls.Add(lblStatus);

        //    Button btnOk = new Button { Text = "XÁC NHẬN ĐỔI BÀN", Location = new Point(410, 375), Size = new Size(200, 45), BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        //    btnOk.FlatAppearance.BorderSize = 0;
        //    popup.Controls.Add(btnOk);

        //    foreach (var t in availableTables)
        //    {
        //        Button btnT = new Button { Text = $"Bàn {t.TableId}\n({t.Capacity} chỗ)", Size = new Size(100, 80), Margin = new Padding(10), FlatStyle = FlatStyle.Flat, BackColor = Color.WhiteSmoke, Cursor = Cursors.Hand, Tag = t };
        //        btnT.Click += (s, e) =>
        //        {
        //            if (selectedIds.Contains(t.TableId)) { selectedIds.Remove(t.TableId); totalCapacity -= t.Capacity; btnT.BackColor = Color.WhiteSmoke; btnT.ForeColor = Color.Black; }
        //            else { selectedIds.Add(t.TableId); totalCapacity += t.Capacity; btnT.BackColor = Color.FromArgb(46, 204, 113); btnT.ForeColor = Color.White; }

        //            lblStatus.Text = $"Đã chọn: {selectedIds.Count} bàn ({totalCapacity} chỗ)";
        //            lblStatus.ForeColor = totalCapacity >= guestCount ? Color.MediumSeaGreen : Color.IndianRed;
        //        };
        //        flp.Controls.Add(btnT);
        //    }

        //    btnOk.Click += (s, e) =>
        //    {
        //        if (selectedIds.Count == 0) { MessageBox.Show("Vui lòng chọn ít nhất 1 bàn!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        //        if (totalCapacity < guestCount) { MessageBox.Show($"Sức chứa không đủ cho {guestCount} khách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        //        if (totalCapacity > guestCount + 2) { MessageBox.Show($"Bạn đang chọn bàn quá rộng! (Dư tối đa 2 chỗ). Vui lòng chọn tổ hợp bàn nhỏ hơn.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        //        result = new ChangeTableRequest { TableIds = selectedIds };
        //        popup.DialogResult = DialogResult.OK;
        //        popup.Close();
        //    };

        //    popup.ShowDialog();
        //    return result;
        //}
        private async void BtnChangeTable_Click(object sender, EventArgs e)
        {
            if (_currentReservation == null) return;

            try
            {
                btnChangeTable.Enabled = false;
                btnChangeTable.Text = "ĐANG TẢI...";

                var optionsRes = await _reservationService.GetWalkInOptionsAsync(_currentReservation.GuestCount);

                if (!optionsRes.IsSuccess || optionsRes.Data == null || optionsRes.Data.Groups == null || optionsRes.Data.Groups.Count == 0)
                {
                    MessageBox.Show("Nhà hàng hiện không có tổ hợp bàn nào trống phù hợp cho số lượng khách này!", "Hết bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedIds = ShowOptionsDialog(optionsRes.Data, _currentReservation.GuestCount);
                if (selectedIds == null || selectedIds.Count == 0) return;

                var changeReq = new ChangeTableRequest { TableIds = selectedIds };

                btnChangeTable.Text = "ĐANG CHUYỂN...";
                var res = await _reservationService.ChangeTableAsync(_currentReservation.ReservationId, changeReq);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Đã chuyển bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    long savedResId = _currentReservation.ReservationId;
                    await LoadTodayReservationsAsync();

                    foreach (Control c in flpReservations.Controls)
                    {
                        if (c is Button btnCard && btnCard.Tag is ReservationResponse rObj && rObj.ReservationId == savedResId)
                        {
                            btnCard.PerformClick();
                            flpReservations.ScrollControlIntoView(btnCard);
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
                Text = $"Gợi ý đổi bàn cho đơn {guestCount} khách",
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
                        Margin = new Padding(5, 5, 5, 5),
                        TextAlign = ContentAlignment.MiddleLeft,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };

                    btnOpt.FlatAppearance.BorderColor = opt.AvailableUntil.HasValue ? Color.Orange : Color.LightGray;

                    btnOpt.Click += (s, e) =>
                    {
                        selectedIds = opt.TableIds;
                        popup.DialogResult = DialogResult.OK;
                        popup.Close();
                    };

                    flp.Controls.Add(btnOpt);
                }
            }

            popup.ShowDialog();
            return selectedIds;
        }

        private void ResetUI()
        {
            pnlDetails.Visible = false;
            btnCheckIn.Visible = false;
            btnChangeTable.Visible = false;
            _currentReservation = null;
        }
    }
}