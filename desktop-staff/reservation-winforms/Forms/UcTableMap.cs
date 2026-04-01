using reservation_winforms.DTO;
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
    public partial class UcTableMap : UserControl
    {
        private readonly TableService _tableService;
        private readonly ReservationService _reservationService;

        private List<FloorMapTableResponse> _allTables = new List<FloorMapTableResponse>();
        private List<FloorMapTableResponse> _selectedTables = new List<FloorMapTableResponse>();

        private Timer _blinkTimer;
        private bool _blinkToggle = false;
        private List<Button> _blinkingButtons = new List<Button>();

        public UcTableMap()
        {
            InitializeComponent();
            _tableService = new TableService();
            _reservationService = new ReservationService();

            InitializeBlinkTimer();

            this.Load += UcTableMap_Load;
            
            btnFilterAll.Click += (s, e) => ApplyFilter("ALL");
            btnFilterAvailable.Click += (s, e) => ApplyFilter("AVAILABLE");
            btnFilterOccupied.Click += (s, e) => ApplyFilter("OCCUPIED");
            btnFilterReserved.Click += (s, e) => ApplyFilter("RESERVED");
            btnFilterOverstay.Click += (s, e) => ApplyFilter("OVERSTAY");
            
            btnSeatWalkIn.Click += BtnSeatWalkIn_Click;
        }

        private void InitializeBlinkTimer()
        {
            _blinkTimer = new Timer();
            _blinkTimer.Interval = 800; // Nhấp nháy mỗi 0.8 giây
            _blinkTimer.Tick += BlinkTimer_Tick;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            _blinkToggle = !_blinkToggle; // Đảo trạng thái true/false

            foreach (var btn in _blinkingButtons)
            {
                var table = btn.Tag as FloorMapTableResponse;
                if (table == null) continue;

                if (_blinkToggle)
                {
                    // Đổi sang màu Cam để cảnh báo
                    btn.BackColor = Color.Orange;
                    btn.ForeColor = Color.White;
                }
                else
                {
                    // Trả về màu gốc (Occupied thì Xám, Overstay thì Đỏ)
                    btn.BackColor = table.Status == "OCCUPIED" ? Color.Gray : Color.IndianRed;
                    btn.ForeColor = Color.White;
                }
            }
        }

        private async void UcTableMap_Load(object sender, EventArgs e)
        {
            await LoadTableData();
        }

        private async Task LoadTableData()
        {
            Label lblLoading = new Label { Text = "Đang tải sơ đồ bàn từ Server...", AutoSize = true, Font = new Font("Segoe UI", 12) };

            flpTableMap.Controls.Add(lblLoading);

            var response = await _tableService.GetFloorMapAsync();

            flpTableMap.Controls.Clear();

            if (response.IsSuccess && response.Data != null)
            {
                _allTables = response.Data;
                _selectedTables.Clear();
                UpdateSelectedTableLabel();

                if (response.Data.Count == 0)
                {
                    Label lblEmpty = new Label
                    {
                        Text = "Chưa có bàn nào trong hệ thống!\nVui lòng nhờ Quản lý (Manager) vào tab 'Quản lý Sơ đồ bàn' để thêm bàn mới.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Margin = new Padding(20)
                    };
                    flpTableMap.Controls.Add(lblEmpty);
                }
                else
                {
                    _allTables = response.Data;
                    DrawTables(_allTables);
                }
            }
            else
            {
                MessageBox.Show(response.Message, "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilter(string filterType)
        {
            flpTableMap.Controls.Clear();

            if (_allTables == null || _allTables.Count == 0) return;

            List<FloorMapTableResponse> filteredList = new List<FloorMapTableResponse>();

            foreach (var table in _allTables)
            {
                if (!table.IsActive) continue;

                bool isReserved = table.CurrentReservationStatus == "RESERVED";

                switch (filterType)
                {
                    case "ALL":
                        filteredList.Add(table);
                        break;
                    case "AVAILABLE":
                        if (table.Status == "AVAILABLE" && !isReserved) filteredList.Add(table);
                        break;
                    case "RESERVED":
                        if (table.Status == "AVAILABLE" && isReserved) filteredList.Add(table);
                        break;
                    case "OCCUPIED":
                        if (table.Status == "OCCUPIED") filteredList.Add(table);
                        break;
                    case "OVERSTAY":
                        if (table.Status == "OVERSTAY") filteredList.Add(table);
                        break;
                }
            }

            DrawTables(filteredList);
        }

        private void DrawTables(List<FloorMapTableResponse> tables)
        {
            _blinkTimer.Stop();
            _blinkingButtons.Clear();
            bool hasBlinkingTables = false;

            foreach (var table in tables)
            {
                if (!table.IsActive) continue;

                Button btnTable = new Button
                {
                    Width = 120,
                    Height = 120,
                    Margin = new Padding(15),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Tag = table
                };

                btnTable.FlatAppearance.BorderSize = 0;
                string tableText = $"Bàn {table.TableId}\n({table.Capacity} chỗ)";

                bool shouldBlink = false;

                if ((table.Status == "OCCUPIED" || table.Status == "OVERSTAY") && table.NextReservationTime.HasValue)
                {
                    TimeSpan timeUntilNext = table.NextReservationTime.Value - DateTime.Now;

                    if (timeUntilNext.TotalMinutes > 0 && timeUntilNext.TotalMinutes <= 15)
                    {
                        shouldBlink = true;
                        hasBlinkingTables = true;
                        tableText += $"\n\n⏰ Khách mới: {table.NextReservationTime.Value:HH:mm}";
                    }
                }

                switch (table.Status)
                {
                    case "AVAILABLE":
                        if (table.CurrentReservationStatus == "RESERVED")
                        {
                            btnTable.BackColor = Color.Orange;
                            btnTable.ForeColor = Color.White;

                            if (table.CurrentReservationTime.HasValue)
                            {
                                tableText += $"\n\nĐã đặt: {table.CurrentReservationTime.Value.ToString("HH:mm")}";
                            }
                            else
                            {
                                tableText += $"\n\nĐã đặt trước";
                            }
                        }
                        else
                        {
                            btnTable.BackColor = Color.MediumSeaGreen;
                            btnTable.ForeColor = Color.White;
                        }
                        break;

                    case "OCCUPIED":
                        btnTable.BackColor = Color.Gray;
                        btnTable.ForeColor = Color.White;
                        if (!shouldBlink) tableText += $"\n\nKhách: {table.CurrentCustomerName}";
                        break;

                    case "OVERSTAY":
                        btnTable.BackColor = Color.IndianRed;
                        btnTable.ForeColor = Color.White;
                        if (!shouldBlink) tableText += $"\n\n⚠️ QUÁ GIỜ";
                        break;

                    default:
                        btnTable.BackColor = Color.Gray;
                        btnTable.ForeColor = Color.White;
                        break;
                }

                btnTable.Text = tableText;
                btnTable.Click += BtnTable_Click;

                if (shouldBlink)
                {
                    _blinkingButtons.Add(btnTable);
                }

                flpTableMap.Controls.Add(btnTable);
            }

            if (hasBlinkingTables)
            {
                _blinkTimer.Start();
            }
        }

        private void BtnTable_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            FloorMapTableResponse tableData = clickedButton.Tag as FloorMapTableResponse;

            bool isAvailable = tableData.Status == "AVAILABLE";

            if (isAvailable)
            {
                var existingTable = _selectedTables.FirstOrDefault(t => t.TableId == tableData.TableId);

                if (existingTable != null)
                {
                    _selectedTables.Remove(existingTable);
                    clickedButton.BackColor = tableData.CurrentReservationStatus == "RESERVED" ? Color.Orange : Color.MediumSeaGreen;
                }
                else
                {
                    if (tableData.CurrentReservationStatus == "RESERVED")
                    {
                        string timeStr = tableData.CurrentReservationTime?.ToString("HH:mm") ?? "N/A";
                        DialogResult confirmResult = MessageBox.Show(
                            $"Lưu ý: Bàn {tableData.TableId} đã có khách đặt lúc {timeStr}.\n" +
                            "Hệ thống sẽ tự động giới hạn thời gian ăn của khách Walk-in để trả bàn kịp giờ.\n\n" +
                            "Bạn có chắc chắn muốn chọn bàn này không?",
                            "Bàn trống tạm thời",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Information);

                        if (confirmResult != DialogResult.OK) return;
                    }

                    _selectedTables.Add(tableData);
                    clickedButton.BackColor = Color.DodgerBlue;
                }

                UpdateSelectedTableLabel();
            }
            else
            {
                MessageBox.Show($"Bàn {tableData.TableId} hiện đang có khách ngồi (Occupied/Overstay), không thể chọn!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateSelectedTableLabel()
        {
            if (_selectedTables.Count == 0)
            {
                lblSelectedTable.Text = "Chưa chọn bàn";
                lblSelectedTable.ForeColor = Color.FromArgb(41, 128, 185);
            }
            else
            {
                var selectedIds = _selectedTables.Select(t => t.TableId).OrderBy(id => id);
                lblSelectedTable.Text = $"Bàn: {string.Join(", ", selectedIds)}";
                lblSelectedTable.ForeColor = Color.MediumSeaGreen;
            }
        }

        private async void BtnSeatWalkIn_Click(object sender, EventArgs e)
        {
            try
            {
                btnSeatWalkIn.Enabled = false;
                btnSeatWalkIn.Text = "ĐANG XỬ LÝ...";

                int guests = (int)nudGuestCount.Value;
                List<long> finalTableIdsToSuggest = null;

                if (_selectedTables.Count > 0)
                {
                    finalTableIdsToSuggest = _selectedTables.Select(t => t.TableId).ToList();
                }
                else
                {
                    var optionsRes = await _reservationService.GetWalkInOptionsAsync(guests);

                    if (!optionsRes.IsSuccess || optionsRes.Data.Groups == null || optionsRes.Data.Groups.Count == 0)
                    {
                        MessageBox.Show("Nhà hàng hiện đã hết bàn trống hoặc không có tổ hợp bàn ghép nào phù hợp cho số lượng khách này.", "Hết bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    finalTableIdsToSuggest = ShowOptionsDialog(optionsRes.Data, guests);

                    if (finalTableIdsToSuggest == null) return;
                }

                var request = new WalkInRequest
                {
                    GuestCount = guests,
                    TableId = finalTableIdsToSuggest,
                    MergeTables = false
                };

                var suggestRes = await _reservationService.SuggestWalkInAsync(request);

                if (!suggestRes.IsSuccess)
                {
                    MessageBox.Show(suggestRes.Message, "Không thể giữ bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await LoadTableData();
                    return;
                }

                var suggestion = suggestRes.Data;
                string tableList = string.Join(", ", suggestion.SuggestedTables.Select(t => t.TableId));
                string typeText = suggestion.AvailabilityType.Contains("PARTIAL") ? "⚠️ TRỐNG TẠM THỜI (Vướng lịch đặt sau)" : "✅ TRỐNG HOÀN TOÀN";

                string confirmMsg = $"[THÔNG TIN CHỐT XẾP BÀN]\n\n" +
                                    $"Bàn: {tableList} (Sức chứa {suggestion.SuggestedTables.Sum(t => t.Capacity)} chỗ)\n" +
                                    $"Loại bàn: {typeText}\n" +
                                    $"Thời gian khách ngồi: Từ {suggestion.StartTime:HH:mm} đến {suggestion.EndTime:HH:mm}\n\n" +
                                    $"*Hệ thống đang khóa tạm bàn này đến {suggestion.LockExpiresAt:HH:mm:ss}.\n" +
                                    $"Khách hàng có đồng ý không?";

                DialogResult result = MessageBox.Show(confirmMsg, "Xác nhận chốt đơn", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    var confirmRes = await _reservationService.ConfirmWalkInAsync(suggestion.SuggestionId);

                    if (confirmRes.IsSuccess)
                    {
                        MessageBox.Show("Xếp bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(confirmRes.Message, "Lỗi chốt đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    await _reservationService.CancelWalkInSuggestionAsync(suggestion.SuggestionId);
                    MessageBox.Show("Đã hủy gợi ý xếp bàn.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadTableData();
            }
            finally
            {
                btnSeatWalkIn.Enabled = true;
                btnSeatWalkIn.Text = "XẾP BÀN (WALK-IN)";
            }
        }

        private List<long> ShowOptionsDialog(WalkInOptionResponse data, int guestCount)
        {
            List<long> selectedIds = null;

            Form popup = new Form
            {
                Text = $"Danh sách phương án xếp bàn cho {guestCount} khách",
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

    }
}