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
        // Lưu trữ cái bàn mà Lễ tân vừa click vào
        private List<FloorMapTableResponse> _selectedTables = new List<FloorMapTableResponse>();

        public UcTableMap()
        {
            InitializeComponent();
            _tableService = new TableService();
            _reservationService = new ReservationService();

            this.Load += UcTableMap_Load;
            
            // Sự kiện các nút lọc
            btnFilterAll.Click += (s, e) => ApplyFilter("ALL");
            btnFilterAvailable.Click += (s, e) => ApplyFilter("AVAILABLE");
            btnFilterOccupied.Click += (s, e) => ApplyFilter("OCCUPIED");
            btnFilterReserved.Click += (s, e) => ApplyFilter("RESERVED");
            btnFilterOverstay.Click += (s, e) => ApplyFilter("OVERSTAY");

            // Sự kiện nút xếp bàn
            btnSeatWalkIn.Click += BtnSeatWalkIn_Click;
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
                        tableText += $"\n\nKhách: {table.CurrentCustomerName}";
                        break;

                    case "OVERSTAY":
                        btnTable.BackColor = Color.IndianRed;
                        btnTable.ForeColor = Color.White;
                        tableText += $"\n\n⚠️ QUÁ GIỜ";
                        break;

                    default:
                        btnTable.BackColor = Color.Gray;
                        btnTable.ForeColor = Color.White;
                        break;
                }

                btnTable.Text = tableText;
                btnTable.Click += BtnTable_Click;

                flpTableMap.Controls.Add(btnTable);
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
                    // FIX: Chỉ khi bấm OK mới thực hiện chọn bàn màu Cam
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

                        if (confirmResult != DialogResult.OK) return; // Thoát nếu không chọn OK
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
                // KHÓA NÚT KHI VỪA BẤM
                btnSeatWalkIn.Enabled = false;
                btnSeatWalkIn.Text = "ĐANG XỬ LÝ...";

                int guests = (int)nudGuestCount.Value;
                List<long> finalTableIdsToSuggest = null;

                // KỊCH BẢN 1: LỄ TÂN ĐÃ TỰ CLICK CHỌN BÀN TRÊN SƠ ĐỒ
                if (_selectedTables.Count > 0)
                {
                    finalTableIdsToSuggest = _selectedTables.Select(t => t.TableId).ToList();
                }
                // KỊCH BẢN 2: LỄ TÂN CHƯA CHỌN BÀN -> GỌI API LẤY GỢI Ý
                else
                {
                    var optionsRes = await _reservationService.GetWalkInOptionsAsync(guests);

                    if (!optionsRes.IsSuccess || optionsRes.Data.Groups == null || optionsRes.Data.Groups.Count == 0)
                    {
                        MessageBox.Show("Nhà hàng hiện đã hết bàn trống hoặc không có tổ hợp bàn ghép nào phù hợp cho số lượng khách này.", "Hết bàn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Thoát hàm -> Sẽ nhảy thẳng xuống khối finally để mở khóa nút
                    }

                    // Hiển thị Popup danh sách phương án cho Lễ tân chọn
                    finalTableIdsToSuggest = ShowOptionsDialog(optionsRes.Data, guests);

                    // Lễ tân tắt Popup hoặc bấm Cancel
                    if (finalTableIdsToSuggest == null) return;
                }

                // --- BƯỚC TIẾP THEO: GỌI API SUGGEST ĐỂ SOFT-LOCK BÀN LẠI ---
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

                // --- BƯỚC CUỐI: XÁC NHẬN CHÍNH THỨC (CONFIRM) VỚI KHÁCH ---
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
                // BÙA HỘ MỆNH: LUÔN TRẢ LẠI TRẠNG THÁI NÚT VỀ BAN ĐẦU
                btnSeatWalkIn.Enabled = true;
                btnSeatWalkIn.Text = "XẾP BÀN (WALK-IN)";
            }
        }

        // =========================================================================
        // HÀM VẼ GIAO DIỆN POPUP CHỌN PHƯƠNG ÁN (KHÔNG CẦN DÙNG TỚI DESIGNER)
        // =========================================================================
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

            // Vẽ từng nhóm (Ưu tiên, Dự phòng)
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

                // Vẽ từng Option trong nhóm đó dưới dạng Nút bấm
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

                    // Trang trí viền nút theo loại
                    btnOpt.FlatAppearance.BorderColor = opt.AvailableUntil.HasValue ? Color.Orange : Color.LightGray;

                    // Gắn sự kiện: Bấm vào option nào thì bắt ID của option đó và tắt Form
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
            return selectedIds; // Trả về List ID bàn Lễ tân vừa bấm chọn
        }

    }
}