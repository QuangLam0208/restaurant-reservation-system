using System;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class MainDashboardForm : Form
    {
        public MainDashboardForm()
        {
            InitializeComponent();

            // 1. Mặc định khi vừa mở lên, hiển thị Sơ đồ bàn (UcTableMap)
            LoadUserControl(new UcTableMap());

            // 2. Gắn sự kiện Click cho tất cả các nút trên thanh Menu
            btnTableMap.Click += BtnTableMap_Click;
            btnOnlineBooking.Click += BtnOnlineBooking_Click;
            btnWaitlist.Click += BtnWaitlist_Click;
            btnTableSetup.Click += BtnTableSetup_Click;
            btnReports.Click += BtnReports_Click;
            btnSystemLogs.Click += BtnSystemLogs_Click;

            // 3. Nút Đăng xuất
            btnLogout.Click += BtnLogout_Click;
        }

        // =========================================================
        // HÀM XỬ LÝ LÕI: Nhúng UserControl vào Panel hiển thị
        // =========================================================
        private void LoadUserControl(UserControl uc)
        {
            // Xóa sạch nội dung cũ trong Panel
            pnlMainContent.Controls.Clear();

            // Căn chỉnh giao diện mới lấp đầy Panel
            uc.Dock = DockStyle.Fill;

            // Thêm giao diện mới vào Panel
            pnlMainContent.Controls.Add(uc);
            uc.BringToFront();
        }

        // =========================================================
        // XỬ LÝ CHUYỂN MENU (Sử dụng hàm LoadUserControl ở trên)
        // =========================================================

        private void BtnTableMap_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcTableMap());
        }

        private void BtnOnlineBooking_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcOnlineBooking());
        }

        private void BtnWaitlist_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcWaitlist());
        }

        private void BtnTableSetup_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcTableSetup());
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcReports());
        }

        private void BtnSystemLogs_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcSystemLogs());
        }

        // =========================================================
        // XỬ LÝ ĐĂNG XUẤT
        // =========================================================
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close(); // Đóng Dashboard. Code bên LoginForm sẽ tự động hiện lại màn hình Login.
            }
        }
    }
}