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
            // --- BƯỚC 1: GỬI YÊU CẦU GỢI Ý (SUGGEST) ---
            btnSeatWalkIn.Enabled = false;
            btnSeatWalkIn.Text = "ĐANG TÌM BÀN...";

            var request = new WalkInRequest
            {
                GuestCount = (int)nudGuestCount.Value,
                // Nếu Lễ tân đã click chọn bàn trên sơ đồ thì gửi ID qua, nếu không để null để Backend tự tìm
                TableId = _selectedTables.Count > 0 ? _selectedTables.Select(t => t.TableId).ToList() : null,
                MergeTables = _selectedTables.Count > 1 || _selectedTables.Count == 0
            };

            var suggestRes = await _reservationService.SuggestWalkInAsync(request);

            if (!suggestRes.IsSuccess)
            {
                MessageBox.Show(suggestRes.Message, "Không tìm thấy bàn phù hợp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetButtonState();

                // Nếu Backend báo hết bàn hoặc lỗi, load lại sơ đồ để Lễ tân thấy trạng thái mới nhất
                await LoadTableData();
                return;
            }

            // --- BƯỚC 2: HIỆN THÔNG TIN GỢI Ý ĐỂ XÁC NHẬN ---
            var suggestion = suggestRes.Data;
            string tableList = string.Join(", ", suggestion.SuggestedTables.Select(t => t.TableId));

            // Dịch 4 loại trạng thái từ Backend sang tiếng Việt thân thiện cho Lễ tân
            string typeText = "";
            switch (suggestion.AvailabilityType)
            {
                case "FULL_AVAILABLE":
                    typeText = "✅ TRỐNG HOÀN TOÀN (Bàn đơn)";
                    break;
                case "MERGED_AVAILABLE":
                    typeText = "✅ TRỐNG HOÀN TOÀN (Ghép bàn)";
                    break;
                case "PARTIAL_AVAILABLE":
                    typeText = "⚠️ TRỐNG TẠM THỜI (Bàn đơn - Sắp có khách đặt trước)";
                    break;
                case "PARTIAL_MERGED_AVAILABLE":
                    typeText = "⚠️ TRỐNG TẠM THỜI (Ghép bàn - Có bàn vướng lịch đặt trước)";
                    break;
                default:
                    typeText = suggestion.AvailabilityType;
                    break;
            }

            string confirmMsg = $"[THÔNG TIN XẾP BÀN WALK-IN]\n\n" +
                                $"Bàn được chọn: Bàn {tableList}\n" +
                                $"Tổng sức chứa: {suggestion.SuggestedTables.Sum(t => t.Capacity)} chỗ\n" +
                                $"Loại bàn: {typeText}\n" +
                                $"Thời gian khách ngồi: Từ {suggestion.StartTime:HH:mm} đến {suggestion.EndTime:HH:mm}\n\n" +
                                $"Lưu ý: Hệ thống đang tạm khóa (Soft-lock) bàn này đến {suggestion.LockExpiresAt:HH:mm:ss} để chờ bạn xác nhận.\n\n" +
                                $"Khách hàng có đồng ý với sự sắp xếp này không?";

            DialogResult result = MessageBox.Show(confirmMsg, "Xác nhận với khách hàng", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                // --- BƯỚC 3: XÁC NHẬN CHÍNH THỨC (CONFIRM) ---
                btnSeatWalkIn.Text = "ĐANG CHỐT ĐƠN...";
                var confirmRes = await _reservationService.ConfirmWalkInAsync(suggestion.SuggestionId);

                if (confirmRes.IsSuccess)
                {
                    MessageBox.Show("Đã xếp khách vào bàn thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Không cần Clear() thủ công ở đây vì LoadTableData() sẽ tự làm việc đó
                    await LoadTableData();
                }
                else
                {
                    MessageBox.Show(confirmRes.Message, "Lỗi xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await LoadTableData(); // Tránh lỗi kẹt trạng thái
                }
            }
            else
            {
                // --- BƯỚC 4: HỦY GỢI Ý (CANCEL SUGGESTION) ---
                btnSeatWalkIn.Text = "ĐANG HỦY GIỮ BÀN...";
                await _reservationService.CancelWalkInSuggestionAsync(suggestion.SuggestionId);

                MessageBox.Show("Đã hủy gợi ý xếp bàn. Bàn đã được nhả lại cho hệ thống.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadTableData(); // Tải lại để xóa soft-lock trên Server và trả lại màu gốc
            }

            ResetButtonState();
        }

        // Hàm phụ để reset trạng thái nút
        private void ResetButtonState()
        {
            btnSeatWalkIn.Enabled = true;
            btnSeatWalkIn.Text = "XẾP BÀN (WALK-IN)";
        }
    }
}