using reservation_winforms.DTO.overrides;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            txtSearch.GotFocus += RemoveText;
            txtSearch.LostFocus += AddText;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            dgvLogs.RowTemplate.Height = 50;

            this.Load += async (s, e) => await LoadLogsAsync();
        }

        private void DtpFrom_ValueChanged(object sender, EventArgs e)
        {
            dtpTo.MinDate = dtpFrom.Value.Date;
        }

        private void RemoveText(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Nhập tên nhân viên hoặc mã đơn...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập tên nhân viên hoặc mã đơn...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            btnFilter.Enabled = false;
            btnFilter.Text = "ĐANG TẢI...";

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date.AddDays(1).AddTicks(-1);

            var res = await _overrideService.GetLogsAsync(fromDate, toDate);

            if (res.IsSuccess && res.Data != null)
            {
                _allLogs = res.Data;

                if (txtSearch.Text != "Nhập tên nhân viên hoặc mã đơn..." && !string.IsNullOrWhiteSpace(txtSearch.Text))
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
                MessageBox.Show(res.Message, "Lỗi lấy dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnFilter.Enabled = true;
            btnFilter.Text = "LỌC DỮ LIỆU";
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

            if (keyword == "nhập tên nhân viên hoặc mã đơn..." || string.IsNullOrEmpty(keyword))
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
    }
}