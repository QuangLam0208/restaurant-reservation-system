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
        private ReservationResponse _currentReservation = null;

        public UcOnlineBooking()
        {
            InitializeComponent();
            _reservationService = new ReservationService();
            _tableService = new TableService();

            // Gắn sự kiện cho các nút
            btnSearch.Click += BtnSearch_Click;
            btnCheckIn.Click += BtnCheckIn_Click;

            // LƯU Ý: Ở FILE DESIGNER BẠN HÃY ĐỔI TÊN NÚT NÀY TỪ btnCancelBooking THÀNH btnChangeTable VÀ SỬA CHỮ THÀNH "ĐỔI BÀN"
            btnChangeTable.Click += BtnChangeTable_Click;

            // Đổi màu nút để hợp với nút Đổi bàn (Xanh dương)
            btnChangeTable.Text = "ĐỔI BÀN";
            btnChangeTable.ForeColor = Color.FromArgb(41, 128, 185);
            btnChangeTable.FlatAppearance.BorderColor = Color.FromArgb(41, 128, 185);

            // Ẩn panel chi tiết lúc mới vào màn hình
            pnlDetails.Visible = false;
            btnCheckIn.Visible = false;
            btnChangeTable.Visible = false;
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

            // Lấy TẤT CẢ các đơn khớp số điện thoại hoặc mã đơn, sắp xếp theo giờ đến tăng dần
            var matchedReservations = res.Data.Where(r =>
                r.Type == "ONLINE" &&
                (r.CustomerPhone == keyword || r.ReservationId.ToString() == keyword))
                .OrderBy(r => r.StartTime)
                .ToList();

            if (matchedReservations.Count == 0)
            {
                MessageBox.Show("Không tìm thấy đơn đặt trước (Online) chờ Check-in với thông tin này.\nKhách có thể đã bị hủy đơn (No-show) hoặc nhập sai số.",
                                "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ResetUI();
                return;
            }

            // XỬ LÝ LỰA CHỌN
            if (matchedReservations.Count == 1)
            {
                // Chỉ có 1 đơn -> Gán luôn
                _currentReservation = matchedReservations[0];
            }
            else
            {
                // Có nhiều đơn -> Bật Popup cho Lễ tân chọn
                _currentReservation = ShowReservationSelectionDialog(matchedReservations);

                // Nếu Lễ tân bấm [X] tắt Popup mà không chọn gì
                if (_currentReservation == null) return;
            }

            // Hiển thị thông tin đơn đã được chọn ra màn hình chính
            await ShowReservationInfo(_currentReservation);
        }

        // 2. HIỂN THỊ CHI TIẾT
        private async Task ShowReservationInfo(ReservationResponse r)
        {
            pnlDetails.Visible = true;
            btnCheckIn.Visible = true;
            btnChangeTable.Visible = true;

            lblValName.Text = r.CustomerName ?? "Khách hàng";
            lblValPhone.Text = r.CustomerPhone ?? "N/A";
            lblValGuests.Text = $"{r.GuestCount} người";
            lblValTime.Text = r.StartTime.ToString("HH:mm");
            lblValTable.Text = (r.TableIds != null && r.TableIds.Count > 0)
                ? string.Join(", ", r.TableIds)
                : "Chưa xếp bàn";

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

        // ==========================================================
        // 5. HIỂN THỊ POPUP NẾU KHÁCH CÓ NHIỀU ĐƠN
        // ==========================================================
        private ReservationResponse ShowReservationSelectionDialog(List<ReservationResponse> list)
        {
            ReservationResponse selectedRes = null;

            Form popup = new Form
            {
                Text = $"Tìm thấy {list.Count} đơn đặt bàn",
                Size = new Size(500, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(15),
                AutoScroll = true
            };
            popup.Controls.Add(flp);

            Label lblHint = new Label
            {
                Text = $"Khách hàng {list[0].CustomerName} có nhiều đơn đặt bàn.\nVui lòng chọn đơn cần Check-in:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Margin = new Padding(0, 0, 15, 15)
            };
            flp.Controls.Add(lblHint);

            foreach (var r in list)
            {
                string timeStr = r.StartTime.ToString("HH:mm");
                string tableStr = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "Chưa xếp";

                Button btnRes = new Button
                {
                    Text = $"Giờ đến: {timeStr}  |  Khách: {r.GuestCount} người\nBàn: {tableStr}  |  Đã cọc: {r.DepositAmount:N0}đ",
                    Width = 450,
                    Height = 60,
                    Margin = new Padding(0, 0, 0, 10),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10),
                    Cursor = Cursors.Hand
                };

                btnRes.FlatAppearance.BorderColor = Color.FromArgb(41, 128, 185);

                // Bấm vào nút nào thì lấy thông tin đơn đó
                btnRes.Click += (s, e) =>
                {
                    selectedRes = r;
                    popup.DialogResult = DialogResult.OK;
                    popup.Close();
                };

                flp.Controls.Add(btnRes);
            }

            popup.ShowDialog();
            return selectedRes;
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
                MessageBox.Show(result.Message, "Không thể Check-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSearch.PerformClick(); // Tải lại data mới nhất
            }

            btnCheckIn.Enabled = true;
            btnCheckIn.Text = "CHECK-IN";
        }

        // ==========================================================
        // 4. ĐỔI BÀN (Popup Thông minh - THAY THẾ NÚT HỦY CŨ)
        // ==========================================================
        private async void BtnChangeTable_Click(object sender, EventArgs e)
        {
            if (_currentReservation == null) return;

            btnChangeTable.Enabled = false;
            btnChangeTable.Text = "ĐANG TẢI...";

            // Lấy danh sách bàn để Lễ tân chọn
            var mapRes = await _tableService.GetFloorMapAsync();

            btnChangeTable.Enabled = true;
            btnChangeTable.Text = "ĐỔI BÀN";

            if (!mapRes.IsSuccess || mapRes.Data == null)
            {
                MessageBox.Show("Không thể tải sơ đồ bàn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lọc ra các bàn trống (Không có người ngồi, không bị Reserved)
            var availableTables = mapRes.Data.Where(t => t.IsActive && t.Status == "AVAILABLE" && t.CurrentReservationStatus != "RESERVED").ToList();

            if (availableTables.Count == 0)
            {
                MessageBox.Show("Nhà hàng hiện tại không còn bàn trống nào để đổi!", "Hết bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mở Popup để Lễ tân chọn bàn (Không cần nhập lý do)
            var changeReq = ShowChangeTableDialog(availableTables, _currentReservation.GuestCount);
            if (changeReq == null) return; // Lễ tân bấm Hủy / Tắt popup

            // GỌI API ĐỔI BÀN
            btnChangeTable.Enabled = false;
            btnChangeTable.Text = "ĐANG CHUYỂN...";

            var res = await _reservationService.ChangeTableAsync(_currentReservation.ReservationId, changeReq);

            if (res.IsSuccess)
            {
                MessageBox.Show("Đã chuyển bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _currentReservation = res.Data;
                await ShowReservationInfo(_currentReservation); // Cập nhật lại giao diện (Số bàn thay đổi)
            }
            else
            {
                MessageBox.Show(res.Message, "Không thể chuyển bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            btnChangeTable.Enabled = true;
            btnChangeTable.Text = "ĐỔI BÀN";
        }

        private ChangeTableRequest ShowChangeTableDialog(List<FloorMapTableResponse> availableTables, int guestCount)
        {
            ChangeTableRequest result = null;
            List<long> selectedIds = new List<long>();
            int totalCapacity = 0;

            Form popup = new Form
            {
                Text = $"Đổi bàn cho {guestCount} khách",
                Size = new Size(650, 480), // Nhỏ gọn hơn do bỏ textbox lý do
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            Label lblTop = new Label { Text = $"Chọn bàn mới (Cần chứa đủ {guestCount} khách):", AutoSize = true, Location = new Point(20, 20), Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            popup.Controls.Add(lblTop);

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Location = new Point(20, 60),
                Size = new Size(590, 300),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            popup.Controls.Add(flp);

            Label lblStatus = new Label { Text = "Đã chọn: 0 bàn (0 chỗ)", AutoSize = true, Location = new Point(20, 380), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.MediumSeaGreen };
            popup.Controls.Add(lblStatus);

            Button btnOk = new Button { Text = "XÁC NHẬN ĐỔI BÀN", Location = new Point(410, 375), Size = new Size(200, 45), BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnOk.FlatAppearance.BorderSize = 0;
            popup.Controls.Add(btnOk);

            foreach (var t in availableTables)
            {
                Button btnT = new Button
                {
                    Text = $"Bàn {t.TableId}\n({t.Capacity} chỗ)",
                    Size = new Size(100, 80),
                    Margin = new Padding(10),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.WhiteSmoke,
                    Cursor = Cursors.Hand,
                    Tag = t
                };
                btnT.Click += (s, e) =>
                {
                    if (selectedIds.Contains(t.TableId))
                    {
                        selectedIds.Remove(t.TableId); totalCapacity -= t.Capacity;
                        btnT.BackColor = Color.WhiteSmoke; btnT.ForeColor = Color.Black;
                    }
                    else
                    {
                        selectedIds.Add(t.TableId); totalCapacity += t.Capacity;
                        btnT.BackColor = Color.FromArgb(46, 204, 113); btnT.ForeColor = Color.White;
                    }
                    lblStatus.Text = $"Đã chọn: {selectedIds.Count} bàn ({totalCapacity} chỗ)";
                    lblStatus.ForeColor = totalCapacity >= guestCount ? Color.MediumSeaGreen : Color.IndianRed;
                };
                flp.Controls.Add(btnT);
            }

            btnOk.Click += (s, e) =>
            {
                if (selectedIds.Count == 0) { MessageBox.Show("Vui lòng chọn ít nhất 1 bàn!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (totalCapacity < guestCount) { MessageBox.Show($"Sức chứa ({totalCapacity} chỗ) không đủ cho {guestCount} khách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (totalCapacity > guestCount + 2)
                {
                    MessageBox.Show($"Bạn đang chọn bàn quá rộng! (Dư tối đa 2 chỗ). Vui lòng chọn tổ hợp bàn nhỏ hơn.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                result = new ChangeTableRequest { TableIds = selectedIds };
                popup.DialogResult = DialogResult.OK;
                popup.Close();
            };

            popup.ShowDialog();
            return result;
        }

        private void ResetUI()
        {
            pnlDetails.Visible = false;
            btnCheckIn.Visible = false;
            btnChangeTable.Visible = false;
            txtSearch.Text = "";
            _currentReservation = null;
        }
    }
}