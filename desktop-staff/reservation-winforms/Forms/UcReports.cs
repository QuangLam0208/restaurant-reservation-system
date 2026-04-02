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

            Series sWalkIn = new Series("Walk-in Reservations");
            sWalkIn.ChartType = SeriesChartType.StackedColumn;
            sWalkIn.Color = Color.FromArgb(39, 174, 96);
            sWalkIn.IsValueShownAsLabel = true;

            Series sOnline = new Series("Online Reservations (Success)");
            sOnline.ChartType = SeriesChartType.StackedColumn;
            sOnline.Color = Color.FromArgb(41, 128, 185);
            sOnline.IsValueShownAsLabel = true;

            Series sNoShow = new Series("No-Show Guests");
            sNoShow.ChartType = SeriesChartType.StackedColumn;
            sNoShow.Color = Color.FromArgb(243, 156, 18);
            sNoShow.IsValueShownAsLabel = true;

            chartReservations.Series.Add(sWalkIn);
            chartReservations.Series.Add(sOnline);
            chartReservations.Series.Add(sNoShow);

            chartReservations.Series[0].Points.Clear();
            chartReservations.ChartAreas[0].AxisX.Title = "Time";
            chartReservations.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            chartReservations.ChartAreas[0].AxisY.Title = "Number of Reservations";
            chartReservations.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);

            btnFilter.Click += BtnFilter_Click;
            tabFilter.SizeMode = TabSizeMode.Fixed;
            tabFilter.ItemSize = new Size((tabFilter.Width / tabFilter.TabCount) - 2, 35);

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
                btnFilter.Text = "LOADING...";

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
                else MessageBox.Show(rateRes.Message, "Error fetching No-Show Rate", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (chartRes.IsSuccess && chartRes.Data != null)
                {
                    _currentReportData = chartRes.Data;
                    chartReservations.Series["Walk-in Reservations"].Points.Clear();
                    chartReservations.Series["Online Reservations (Success)"].Points.Clear();
                    chartReservations.Series["No-Show Guests"].Points.Clear();

                    foreach (var item in chartRes.Data)
                    {
                        string displayLabel = item.Date;
                        if (item.Date.Length == 10) displayLabel = DateTime.Parse(item.Date).ToString(xLabelFormat);
                        else if (item.Date.Length == 7) displayLabel = DateTime.Parse(item.Date + "-01").ToString(xLabelFormat);

                        long walkIn = item.TotalWalkIn;
                        long noShow = item.NoShowCount;
                        long onlineSuccess = item.TotalOnline - noShow;

                        chartReservations.Series["Walk-in Reservations"].Points.AddXY(displayLabel, walkIn);
                        chartReservations.Series["Online Reservations (Success)"].Points.AddXY(displayLabel, onlineSuccess);
                        chartReservations.Series["No-Show Guests"].Points.AddXY(displayLabel, noShow);

                        int lastIdx = chartReservations.Series["Walk-in Reservations"].Points.Count - 1;
                        chartReservations.Series["Walk-in Reservations"].Points[lastIdx].IsValueShownAsLabel = (walkIn > 0);
                        chartReservations.Series["Online Reservations (Success)"].Points[lastIdx].IsValueShownAsLabel = (onlineSuccess > 0);
                        chartReservations.Series["No-Show Guests"].Points[lastIdx].IsValueShownAsLabel = (noShow > 0);
                    }
                }
                else MessageBox.Show(chartRes.Message, "Error rendering chart", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFilter.Enabled = true;
                btnFilter.Text = "FILTER DATA";
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (_currentReportData == null || _currentReportData.Count == 0 || _currentRateData == null)
            {
                MessageBox.Show("There is no report data to export!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel CSV File|*.csv", FileName = $"Statistics_Report_{DateTime.Now:ddMMyyyy}.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append('\uFEFF');

                        sb.AppendLine("1. REPORT OVERVIEW");
                        sb.AppendLine($"Total Reservations:,\"{_currentRateData.TotalAll}\"");
                        sb.AppendLine($"Online Reservations:,\"{_currentRateData.TotalOnline}\"");
                        sb.AppendLine($"Walk-in Reservations:,\"{_currentRateData.TotalWalkIn}\"");
                        sb.AppendLine($"No-Show Guests:,\"{_currentRateData.NoShowCount}\"");
                        sb.AppendLine($"No-Show Rate:,\"{_currentRateData.NoShowRate}%\"");
                        sb.AppendLine();
                        sb.AppendLine();

                        sb.AppendLine("2. DETAILS BY TIME");
                        sb.AppendLine("Time,Total Reservations,Walk-in Reservations,Online Reservations,No-Show Guests");

                        foreach (var item in _currentReportData)
                        {
                            long total = item.TotalWalkIn + item.TotalOnline;
                            sb.AppendLine($"\"{item.Date}\",\"{total}\",\"{item.TotalWalkIn}\",\"{item.TotalOnline}\",\"{item.NoShowCount}\"");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Report exported to Excel successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}