using reservation_winforms.DTO.overrides;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcSystemLogs : UserControl
    {
        private readonly OverrideService _overrideService;
        private List<OverrideLogResponse> _allLogs = new List<OverrideLogResponse>();

        public UcSystemLogs()
        {
            InitializeComponent();
            _overrideService = new OverrideService();

            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpTo.Value = DateTime.Now;

            dtpFrom.ValueChanged += DtpFrom_ValueChanged;
            DtpFrom_ValueChanged(null, null);

            btnFilter.Click += BtnFilter_Click;

            txtSearch.Text = "";
            txtSearch.ForeColor = Color.Black;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            dgvLogs.RowTemplate.Height = 50;

            btnExportExcel.Click += BtnExportExcel_Click;

            this.Load += async (s, e) => await LoadLogsAsync();
        }

        private void DtpFrom_ValueChanged(object sender, EventArgs e)
        {
            dtpTo.MinDate = dtpFrom.Value.Date;
        }

        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            btnFilter.Enabled = false;
            btnFilter.Text = "LOADING...";

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date.AddDays(1).AddTicks(-1);

            var res = await _overrideService.GetLogsAsync(fromDate, toDate);

            if (res.IsSuccess && res.Data != null)
            {
                _allLogs = res.Data;

                // Nếu ô tìm kiếm đang có chữ thì tự động lọc lại luôn
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    TxtSearch_TextChanged(null, null);
                }
                else
                {
                    RenderLogs(_allLogs);
                }
            }
            else
            {
                MessageBox.Show(res.Message, "Data Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnFilter.Enabled = true;
            btnFilter.Text = "FILTER DATA";
        }

        private void RenderLogs(List<OverrideLogResponse> list)
        {
            dgvLogs.Rows.Clear();

            foreach (var log in list)
            {
                dgvLogs.Rows.Add(
                    log.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                    log.AccountUsername,
                    $"#{log.ReservationId}",
                    log.Reason
                );
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                RenderLogs(_allLogs);
                return;
            }

            var filteredList = _allLogs.Where(log =>
                (log.AccountUsername != null && log.AccountUsername.ToLower().Contains(keyword)) ||
                log.ReservationId.ToString().Contains(keyword)
            ).ToList();

            RenderLogs(filteredList);
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvLogs.Rows.Count == 0 || (dgvLogs.Rows.Count == 1 && dgvLogs.Rows[0].IsNewRow))
            {
                MessageBox.Show("No data to export!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel CSV File|*.csv", FileName = $"SystemLogs_{DateTime.Now:ddMMyyyy}.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append('\uFEFF');

                        sb.AppendLine("Time,Staff Name,Reservation ID,Reason");

                        foreach (DataGridViewRow row in dgvLogs.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string time = row.Cells[0].Value?.ToString() ?? "";
                            string staff = row.Cells[1].Value?.ToString() ?? "";
                            string id = row.Cells[2].Value?.ToString() ?? "";
                            string reason = row.Cells[3].Value?.ToString() ?? "";

                            reason = reason.Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ");
                            sb.AppendLine($"\"{time}\",\"{staff}\",\"{id}\",\"{reason}\"");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Excel file exported successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
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