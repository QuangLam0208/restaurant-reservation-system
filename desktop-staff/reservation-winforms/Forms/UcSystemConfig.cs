using reservation_winforms.DTO.config;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcSystemConfig : UserControl
    {
        private readonly SystemConfigService _configService;
        private List<SystemConfigDTO> _configList;

        public UcSystemConfig()
        {
            InitializeComponent();
            _configService = new SystemConfigService();
            _configList = new List<SystemConfigDTO>();

            this.Load += async (s, e) => await LoadConfigsAsync();
            dgvConfigs.CellContentClick += DgvConfigs_CellContentClick;
        }

        private async Task LoadConfigsAsync()
        {
            lblStatus.Text = "Đang tải cấu hình hệ thống...";
            lblStatus.ForeColor = Color.Orange;

            var response = await _configService.GetAllConfigsAsync();

            if (response.IsSuccess && response.Data != null)
            {
                _configList = response.Data;
                RenderConfigGrid();
                lblStatus.Text = "Tải dữ liệu thành công!";
                lblStatus.ForeColor = Color.MediumSeaGreen;
            }
            else
            {
                MessageBox.Show(response.Message, "Lỗi tải cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Tải thất bại!";
                lblStatus.ForeColor = Color.IndianRed;
            }
        }

        private void RenderConfigGrid()
        {
            dgvConfigs.Rows.Clear();

            foreach (var config in _configList)
            {
                dgvConfigs.Rows.Add(
                    config.ConfigKey,       // Cột ẩn: Key
                    config.Description,     // Cột hiển thị: Mô tả
                    config.ConfigValue,     // Cột hiển thị: Giá trị (cho phép edit)
                    "LƯU CẬP NHẬT"          // Nút bấm
                );
            }
        }

        private async void DgvConfigs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cột nút "Lưu" (Giả sử cột Lưu là cột thứ 3 - index 3)
            if (e.RowIndex >= 0 && e.ColumnIndex == 3)
            {
                string key = dgvConfigs.Rows[e.RowIndex].Cells[0].Value.ToString();
                string description = dgvConfigs.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? key;
                string newValue = dgvConfigs.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn cập nhật:\n[{description}] thành giá trị [{newValue}] không?",
                    "Xác nhận thay đổi cấu hình",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await UpdateConfigValue(key, newValue, e.RowIndex);
                }
            }
        }

        private async Task UpdateConfigValue(string key, string newValue, int rowIndex)
        {
            dgvConfigs.Rows[rowIndex].Cells[3].Value = "ĐANG LƯU...";
            dgvConfigs.Enabled = false;

            var response = await _configService.UpdateConfigAsync(key, newValue);

            if (response.IsSuccess)
            {
                MessageBox.Show("Cập nhật cấu hình thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvConfigs.Rows[rowIndex].Cells[3].Value = "LƯU CẬP NHẬT";
                // Có thể gọi lại LoadConfigsAsync() ở đây để refresh, hoặc tin tưởng dữ liệu đã gõ
            }
            else
            {
                MessageBox.Show(response.Message, "Lỗi cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await LoadConfigsAsync(); // Reset lại data cũ nếu lỗi
            }

            dgvConfigs.Enabled = true;
        }
    }
}