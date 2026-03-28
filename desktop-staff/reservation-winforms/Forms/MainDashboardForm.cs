using System;
using System.Windows.Forms;
using reservation_winforms.Services;

namespace reservation_winforms.Forms
{
    public partial class MainDashboardForm : Form
    {
        public MainDashboardForm()
        {
            InitializeComponent();

            // 1. Phân quyền hiển thị: Ẩn/Hiện tính năng dựa trên Role
            ApplyRolePermissions();

            // 2. Mặc định khi vừa mở lên, hiển thị Sơ đồ bàn (UcTableMap)
            LoadUserControl(new UcTableMap());

            // 3. Gắn sự kiện Click cho tất cả các nút trên thanh Menu
            btnTableMap.Click += BtnTableMap_Click;
            btnOnlineBooking.Click += BtnOnlineBooking_Click;
            btnWaitlist.Click += BtnWaitlist_Click;
            btnTableSetup.Click += BtnTableSetup_Click;
            btnReports.Click += BtnReports_Click;
            btnSystemLogs.Click += BtnSystemLogs_Click;
            btnRegisterStaff.Click += btnRegisterStaff_Click; // Nút tạo TK Lễ tân (Menu bên trái)

            // 4. Nút Đăng xuất
            btnLogout.Click += BtnLogout_Click;
        }

        // ==========================================
        // PHÂN QUYỀN HIỂN THỊ MENU
        // ==========================================
        private void ApplyRolePermissions()
        {
            // Nếu người đang đăng nhập KHÔNG PHẢI là MANAGER (tức là RECEPTIONIST)
            if (GlobalState.Role != "MANAGER")
            {
                // Ẩn hoàn toàn khu vực dành cho Quản lý
                lblAdminSection.Visible = false; // Nhãn "QUẢN TRỊ (MANAGER)"
                btnTableSetup.Visible = false;   // Quản lý sơ đồ bàn
                btnReports.Visible = false;      // Báo cáo thống kê
                btnSystemLogs.Visible = false;   // Nhật ký hệ thống
                btnRegisterStaff.Visible = false;// Tạo tài khoản Lễ tân
            }
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
        // MỞ FORM ĐĂNG KÝ TÀI KHOẢN (Chỉ Manager mới bấm được nút này)
        // =========================================================
        private void btnRegisterStaff_Click(object sender, EventArgs e)
        {
            // 1. Xóa tất cả giao diện đang hiển thị trên khu vực chính
            pnlMainContent.Controls.Clear();

            // 2. Khởi tạo UserControl Đăng ký
            UcRegister registerCtrl = new UcRegister();

            // 3. Căn giữa UserControl bên trong pnlMainContent
            registerCtrl.Location = new System.Drawing.Point(
                (pnlMainContent.Width - registerCtrl.Width) / 2,
                (pnlMainContent.Height - registerCtrl.Height) / 2
            );

            // 4. Thêm nó vào giao diện
            pnlMainContent.Controls.Add(registerCtrl);
        }

        // =========================================================
        // XỬ LÝ ĐĂNG XUẤT
        // =========================================================
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Xóa toàn bộ dữ liệu phiên đăng nhập
                GlobalState.StaffToken = "";
                GlobalState.CurrentUsername = "";
                GlobalState.Role = "";

                // Đóng Dashboard. Code bên LoginForm sẽ tự động hiện lại màn hình Login.
                this.Close();
            }
        }
    }
}