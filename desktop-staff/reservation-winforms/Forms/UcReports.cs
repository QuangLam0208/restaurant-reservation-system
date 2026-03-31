using reservation_winforms.DTO.report;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace reservation_winforms.Forms
{
    public partial class UcReports : UserControl
    {
        private readonly ReportService _reportService;

        public UcReports()
        {
            InitializeComponent();
            _reportService = new ReportService();

            // Set mặc định Tab Ngày: Đầu tháng tới hiện tại
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            // Set mặc định Tab Tháng: Đầu năm tới hiện tại
            dtpFromMonth.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpToMonth.Value = DateTime.Now;

            // Set mặc định Tab Năm: 3 năm gần nhất
            numFromYear.Value = DateTime.Now.Year - 3;
            numToYear.Value = DateTime.Now.Year;

            // ========================================================
            // GẮN SỰ KIỆN TỰ ĐỘNG KHÓA NGÀY THÁNG LẠI (TRÁNH LỖI)
            // ========================================================
            chartReservations.Series.Clear();
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpFromMonth.ValueChanged += DtpFromMonth_ValueChanged;
            numFromYear.ValueChanged += NumFromYear_ValueChanged;

            // Kích hoạt hàm khóa lần đầu tiên
            DtpFromDate_ValueChanged(null, null);
            DtpFromMonth_ValueChanged(null, null);
            NumFromYear_ValueChanged(null, null);

            Series sWalkIn = new Series("Đơn Walk-in");
            sWalkIn.ChartType = SeriesChartType.StackedColumn;
            sWalkIn.Color = Color.FromArgb(39, 174, 96);
            sWalkIn.IsValueShownAsLabel = true;

            // 2. Khúc giữa: Khách Online thành công (Màu Xanh dương)
            Series sOnline = new Series("Đơn Online (Thành công)");
            sOnline.ChartType = SeriesChartType.StackedColumn;
            sOnline.Color = Color.FromArgb(41, 128, 185);
            sOnline.IsValueShownAsLabel = true;

            // 3. Khúc đỉnh: Khách No-Show (Màu Cam)
            Series sNoShow = new Series("Khách No-Show");
            sNoShow.ChartType = SeriesChartType.StackedColumn;
            sNoShow.Color = Color.FromArgb(243, 156, 18);
            sNoShow.IsValueShownAsLabel = true;

            chartReservations.Series.Add(sWalkIn);
            chartReservations.Series.Add(sOnline);
            chartReservations.Series.Add(sNoShow);

            // Cấu hình Biểu đồ bằng Code (Tránh lỗi Designer)
            chartReservations.Series[0].Points.Clear();
            chartReservations.ChartAreas[0].AxisX.Title = "Thời gian";
            chartReservations.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            chartReservations.ChartAreas[0].AxisY.Title = "Số lượng đơn đặt bàn (Đơn)";
            chartReservations.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);

            btnFilter.Click += BtnFilter_Click;
            tabFilter.SizeMode = TabSizeMode.Fixed;
            tabFilter.ItemSize = new Size((tabFilter.Width / tabFilter.TabCount) - 2, 35);

            // Tự động load dữ liệu khi vừa mở tab
            this.Load += async (s, e) => await ExecuteFilterAsync();
        }

        // --- KHÓA TAB NGÀY: "Đến ngày" không được vượt quá ngày cuối cùng của tháng ---
        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime from = dtpFromDate.Value;

            // Xóa mốc cũ để tránh lỗi xung đột Min > Max
            dtpToDate.MinDate = new DateTime(1753, 1, 1);
            dtpToDate.MaxDate = new DateTime(9998, 12, 31);

            // Gắn mốc mới: Phải lớn hơn hoặc bằng Từ ngày, và nhỏ hơn ngày cuối tháng
            dtpToDate.MinDate = from;
            dtpToDate.MaxDate = new DateTime(from.Year, from.Month, DateTime.DaysInMonth(from.Year, from.Month));
        }

        // --- KHÓA TAB THÁNG: "Đến tháng" không được nhảy qua năm sau ---
        private void DtpFromMonth_ValueChanged(object sender, EventArgs e)
        {
            DateTime from = dtpFromMonth.Value;

            dtpToMonth.MinDate = new DateTime(1753, 1, 1);
            dtpToMonth.MaxDate = new DateTime(9998, 12, 31);

            dtpToMonth.MinDate = from;
            dtpToMonth.MaxDate = new DateTime(from.Year, 12, 31);
        }

        // --- KHÓA TAB NĂM: "Đến năm" không được nhỏ hơn "Từ năm" ---
        private void NumFromYear_ValueChanged(object sender, EventArgs e)
        {
            numToYear.Minimum = numFromYear.Value;
        }

        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            await ExecuteFilterAsync();
        }

        private async System.Threading.Tasks.Task ExecuteFilterAsync()
        {
            try
            {
                btnFilter.Enabled = false;
                btnFilter.Text = "ĐANG TẢI...";

                (bool IsSuccess, List<DailyReservationReport> Data, string Message) chartRes = (false, null, "");
                (bool IsSuccess, NoShowRateResponse Data, string Message) rateRes = (false, null, "");

                string xLabelFormat = "dd/MM";

                // ==========================================
                // LỌC THEO TAB ĐANG CHỌN
                // ==========================================
                if (tabFilter.SelectedTab == tabPageDate)
                {
                    // KHÔNG CẦN VALIDATE IF/ELSE BẰNG MESSAGE BOX NỮA! (VÌ GIAO DIỆN ĐÃ KHÓA CỨNG)
                    string fromStr = dtpFromDate.Value.ToString("yyyy-MM-dd");
                    string toStr = dtpToDate.Value.ToString("yyyy-MM-dd");

                    chartRes = await _reportService.GetReservationsByDateAsync(fromStr, toStr);
                    rateRes = await _reportService.GetNoShowRateAsync(fromStr, toStr);
                    xLabelFormat = "dd/MM";
                }
                else if (tabFilter.SelectedTab == tabPageMonth)
                {
                    string fromStr = dtpFromMonth.Value.ToString("yyyy-MM");
                    string toStr = dtpToMonth.Value.ToString("yyyy-MM");

                    chartRes = await _reportService.GetReservationsByMonthAsync(fromStr, toStr);
                    rateRes = await _reportService.GetNoShowRateByMonthAsync(fromStr, toStr);
                    xLabelFormat = "MM/yyyy";
                }
                else if (tabFilter.SelectedTab == tabPageYear)
                {
                    string fromStr = numFromYear.Value.ToString();
                    string toStr = numToYear.Value.ToString();

                    chartRes = await _reportService.GetReservationsByYearAsync(fromStr, toStr);
                    rateRes = await _reportService.GetNoShowRateByYearAsync(fromStr, toStr);
                    xLabelFormat = "yyyy";
                }

                // ==========================================
                // HIỂN THỊ LÊN GIAO DIỆN
                // ==========================================
                if (rateRes.IsSuccess && rateRes.Data != null)
                {
                    lblTotalAll.Text = rateRes.Data.TotalAll.ToString();
                    lblTotalOnline.Text = rateRes.Data.TotalOnline.ToString();
                    lblTotalWalkIn.Text = rateRes.Data.TotalWalkIn.ToString();
                    lblNoShow.Text = rateRes.Data.NoShowCount.ToString();
                    lblRate.Text = $"{rateRes.Data.NoShowRate}%";

                    lblRate.ForeColor = rateRes.Data.NoShowRate > 15 ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);
                }
                else MessageBox.Show(rateRes.Message, "Lỗi lấy Tỷ lệ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (chartRes.IsSuccess && chartRes.Data != null)
                {
                    chartReservations.Series["Đơn Walk-in"].Points.Clear();
                    chartReservations.Series["Đơn Online (Thành công)"].Points.Clear();
                    chartReservations.Series["Khách No-Show"].Points.Clear();

                    foreach (var item in chartRes.Data)
                    {
                        string displayLabel = item.Date;
                        if (item.Date.Length == 10) displayLabel = DateTime.Parse(item.Date).ToString(xLabelFormat);
                        else if (item.Date.Length == 7) displayLabel = DateTime.Parse(item.Date + "-01").ToString(xLabelFormat);

                        // Tính toán các mảng dữ liệu
                        long walkIn = item.TotalWalkIn;
                        long noShow = item.NoShowCount;
                        long onlineSuccess = item.TotalOnline - noShow;

                        // Thêm các khúc vào cột
                        chartReservations.Series["Đơn Walk-in"].Points.AddXY(displayLabel, walkIn);
                        chartReservations.Series["Đơn Online (Thành công)"].Points.AddXY(displayLabel, onlineSuccess);
                        chartReservations.Series["Khách No-Show"].Points.AddXY(displayLabel, noShow);

                        // Ẩn số 0 cho đỡ rối mắt
                        int lastIdx = chartReservations.Series["Đơn Walk-in"].Points.Count - 1;
                        chartReservations.Series["Đơn Walk-in"].Points[lastIdx].IsValueShownAsLabel = (walkIn > 0);
                        chartReservations.Series["Đơn Online (Thành công)"].Points[lastIdx].IsValueShownAsLabel = (onlineSuccess > 0);
                        chartReservations.Series["Khách No-Show"].Points[lastIdx].IsValueShownAsLabel = (noShow > 0);
                    }
                }
                else MessageBox.Show(chartRes.Message, "Lỗi vẽ biểu đồ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFilter.Enabled = true;
                btnFilter.Text = "🔍 LỌC DỮ LIỆU";
            }
        }
    }
}