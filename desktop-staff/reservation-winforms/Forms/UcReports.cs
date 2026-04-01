using reservation_winforms.DTO.report;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace reservation_winforms.Forms
{
    public partial class UcReports : UserControl
    {
        private readonly ReportService _reportService;

        private List<DailyReservationReport> _currentReportData;
        private NoShowRateResponse _currentRateData;

        public UcReports()
        {
            InitializeComponent();
            _reportService = new ReportService();

            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            dtpFromMonth.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpToMonth.Value = DateTime.Now;

            numFromYear.Value = DateTime.Now.Year - 3;
            numToYear.Value = DateTime.Now.Year;

            chartReservations.Series.Clear();
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpFromMonth.ValueChanged += DtpFromMonth_ValueChanged;
            numFromYear.ValueChanged += NumFromYear_ValueChanged;

            DtpFromDate_ValueChanged(null, null);
            DtpFromMonth_ValueChanged(null, null);
            NumFromYear_ValueChanged(null, null);

            Series sWalkIn = new Series("Đơn Walk-in");
            sWalkIn.ChartType = SeriesChartType.StackedColumn;
            sWalkIn.Color = Color.FromArgb(39, 174, 96);
            sWalkIn.IsValueShownAsLabel = true;

            Series sOnline = new Series("Đơn Online (Thành công)");
            sOnline.ChartType = SeriesChartType.StackedColumn;
            sOnline.Color = Color.FromArgb(41, 128, 185);
            sOnline.IsValueShownAsLabel = true;

            Series sNoShow = new Series("Khách No-Show");
            sNoShow.ChartType = SeriesChartType.StackedColumn;
            sNoShow.Color = Color.FromArgb(243, 156, 18);
            sNoShow.IsValueShownAsLabel = true;

            chartReservations.Series.Add(sWalkIn);
            chartReservations.Series.Add(sOnline);
            chartReservations.Series.Add(sNoShow);

            chartReservations.Series[0].Points.Clear();
            chartReservations.ChartAreas[0].AxisX.Title = "Thời gian";
            chartReservations.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            chartReservations.ChartAreas[0].AxisY.Title = "Số lượng đơn đặt bàn (Đơn)";
            chartReservations.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);

            btnFilter.Click += BtnFilter_Click;
            tabFilter.SizeMode = TabSizeMode.Fixed;
            tabFilter.ItemSize = new Size((tabFilter.Width / tabFilter.TabCount) - 2, 35);

            // BỔ SUNG: Sự kiện click cho nút xuất Excel
            if (btnExportExcel != null)
            {
                btnExportExcel.Click += BtnExportExcel_Click;
            }

            this.Load += async (s, e) => await ExecuteFilterAsync();
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime from = dtpFromDate.Value;
            dtpToDate.MinDate = new DateTime(1753, 1, 1);
            dtpToDate.MaxDate = new DateTime(9998, 12, 31);
            dtpToDate.MinDate = from;
            dtpToDate.MaxDate = new DateTime(from.Year, from.Month, DateTime.DaysInMonth(from.Year, from.Month));
        }

        private void DtpFromMonth_ValueChanged(object sender, EventArgs e)
        {
            DateTime from = dtpFromMonth.Value;
            dtpToMonth.MinDate = new DateTime(1753, 1, 1);
            dtpToMonth.MaxDate = new DateTime(9998, 12, 31);
            dtpToMonth.MinDate = from;
            dtpToMonth.MaxDate = new DateTime(from.Year, 12, 31);
        }

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

                _currentReportData = null;
                _currentRateData = null;

                (bool IsSuccess, List<DailyReservationReport> Data, string Message) chartRes = (false, null, "");
                (bool IsSuccess, NoShowRateResponse Data, string Message) rateRes = (false, null, "");

                string xLabelFormat = "dd/MM";

                if (tabFilter.SelectedTab == tabPageDate)
                {
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

                if (rateRes.IsSuccess && rateRes.Data != null)
                {
                    _currentRateData = rateRes.Data;
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
                    _currentReportData = chartRes.Data;
                    chartReservations.Series["Đơn Walk-in"].Points.Clear();
                    chartReservations.Series["Đơn Online (Thành công)"].Points.Clear();
                    chartReservations.Series["Khách No-Show"].Points.Clear();

                    foreach (var item in chartRes.Data)
                    {
                        string displayLabel = item.Date;
                        if (item.Date.Length == 10) displayLabel = DateTime.Parse(item.Date).ToString(xLabelFormat);
                        else if (item.Date.Length == 7) displayLabel = DateTime.Parse(item.Date + "-01").ToString(xLabelFormat);

                        long walkIn = item.TotalWalkIn;
                        long noShow = item.NoShowCount;
                        long onlineSuccess = item.TotalOnline - noShow;

                        chartReservations.Series["Đơn Walk-in"].Points.AddXY(displayLabel, walkIn);
                        chartReservations.Series["Đơn Online (Thành công)"].Points.AddXY(displayLabel, onlineSuccess);
                        chartReservations.Series["Khách No-Show"].Points.AddXY(displayLabel, noShow);

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

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (_currentReportData == null || _currentReportData.Count == 0 || _currentRateData == null)
            {
                MessageBox.Show("Không có dữ liệu báo cáo để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel CSV File|*.csv", FileName = $"BaoCao_ThongKe_{DateTime.Now:ddMMyyyy}.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append('\uFEFF');

                        sb.AppendLine("1. TỔNG QUAN BÁO CÁO");
                        sb.AppendLine($"Tổng số đơn đã đặt:,\"{_currentRateData.TotalAll}\"");
                        sb.AppendLine($"Đơn Online:,\"{_currentRateData.TotalOnline}\"");
                        sb.AppendLine($"Đơn Walk-in:,\"{_currentRateData.TotalWalkIn}\"");
                        sb.AppendLine($"Số khách bỏ chỗ (No-Show):,\"{_currentRateData.NoShowCount}\"");
                        sb.AppendLine($"Tỷ lệ No-Show:,\"{_currentRateData.NoShowRate}%\"");
                        sb.AppendLine();
                        sb.AppendLine();

                        sb.AppendLine("2. CHI TIẾT THEO THỜI GIAN");
                        sb.AppendLine("Thời gian,Tổng đơn đặt,Đơn Walk-in,Đơn Online,Khách No-Show");

                        foreach (var item in _currentReportData)
                        {
                            long total = item.TotalWalkIn + item.TotalOnline;
                            sb.AppendLine($"\"{item.Date}\",\"{total}\",\"{item.TotalWalkIn}\",\"{item.TotalOnline}\",\"{item.NoShowCount}\"");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất báo cáo ra Excel thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}