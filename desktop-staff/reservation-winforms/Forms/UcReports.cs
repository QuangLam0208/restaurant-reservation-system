using reservation_winforms.Services;
using System;
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

            // Cài đặt ngày mặc định: Từ đầu tháng đến hôm nay
            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpTo.Value = DateTime.Now;

            // Xóa rác biểu đồ lúc thiết kế
            chartReservations.Series[0].Points.Clear();
            chartReservations.ChartAreas[0].AxisX.Title = "Thời gian (Ngày)";
            chartReservations.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);

            chartReservations.ChartAreas[0].AxisY.Title = "Số lượng đơn đặt bàn (Đơn)";
            chartReservations.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            btnFilter.Click += BtnFilter_Click;

            // Tự động load dữ liệu khi vừa mở tab
            this.Load += async (s, e) => await LoadReportDataAsync();
        }

        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            await LoadReportDataAsync();
        }

        private async System.Threading.Tasks.Task LoadReportDataAsync()
        {
            if (dtpFrom.Value.Date > dtpTo.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Lỗi bộ lọc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Khóa nút lọc
                btnFilter.Enabled = false;
                btnFilter.Text = "ĐANG TẢI...";

                DateTime from = dtpFrom.Value.Date;
                DateTime to = dtpTo.Value.Date;

                // GỌI API 1: LẤY TỶ LỆ NO-SHOW (Cho 3 Thẻ phía trên)
                var noShowRes = await _reportService.GetNoShowRateAsync(from, to);
                if (noShowRes.IsSuccess && noShowRes.Data != null)
                {
                    lblTotalAll.Text = noShowRes.Data.TotalAll.ToString();
                    lblTotalOnline.Text = noShowRes.Data.TotalOnline.ToString();
                    lblTotalWalkIn.Text = noShowRes.Data.TotalWalkIn.ToString();
                    lblNoShow.Text = noShowRes.Data.NoShowCount.ToString();
                    lblRate.Text = $"{noShowRes.Data.NoShowRate}%";

                    // Đổi màu tỷ lệ: > 15% thì báo động Đỏ, an toàn thì Xanh
                    lblRate.ForeColor = noShowRes.Data.NoShowRate > 15 ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);
                }
                else
                {
                    MessageBox.Show(noShowRes.Message, "Lỗi lấy Tỷ lệ No-Show", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // GỌI API 2: LẤY DỮ LIỆU VẼ BIỂU ĐỒ
                var chartRes = await _reportService.GetReservationsByDateAsync(from, to);
                if (chartRes.IsSuccess && chartRes.Data != null)
                {
                    // Vẽ biểu đồ
                    chartReservations.Series[0].Points.Clear();
                    foreach (var item in chartRes.Data)
                    {
                        // Chuyển "2026-03-31" thành "31/03" cho dễ nhìn
                        DateTime dt = DateTime.Parse(item.Date);
                        string displayDate = dt.ToString("dd/MM");

                        DataPoint point = new DataPoint();
                        point.SetValueXY(displayDate, item.Count);

                        // Đổi màu cột nếu ngày đó không có đơn nào
                        if (item.Count == 0) point.Color = Color.LightGray;

                        chartReservations.Series[0].Points.Add(point);
                    }
                }
                else
                {
                    MessageBox.Show(chartRes.Message, "Lỗi lấy dữ liệu Biểu đồ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Xả khóa nút lọc
                btnFilter.Enabled = true;
                btnFilter.Text = "🔍 LỌC DỮ LIỆU";
            }
        }
    }
}