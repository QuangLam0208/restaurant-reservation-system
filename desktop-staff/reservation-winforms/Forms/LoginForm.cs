using System;
using System.Windows.Forms;
using reservation_winforms.Services;

namespace reservation_winforms.Forms
{
    public partial class LoginForm : Form
    {
        // Khởi tạo AuthService một lần để dùng chung
        private readonly AuthService _authService;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService();

            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += btnExit_Click;
        }

        // ==========================================
        // 1. XỬ LÝ NÚT ĐĂNG NHẬP
        // ==========================================
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblMessage.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return;
            }

            // Vô hiệu hóa nút trong lúc chờ mạng
            btnLogin.Enabled = false;
            btnLogin.Text = "Đang kiểm tra...";
            lblMessage.Text = "";

            // Gọi API Đăng nhập
            var response = await _authService.LoginAsync(user, pass);

            // Bật lại nút
            btnLogin.Enabled = true;
            btnLogin.Text = "ĐĂNG NHẬP";

            if (response.IsSuccess)
            {
                // Xóa pass cũ
                txtPassword.Clear();
                lblMessage.Text = "";

                // Mở màn hình chính
                this.Hide();
                MainDashboardForm dashboard = new MainDashboardForm();
                dashboard.ShowDialog();

                // Khi đăng xuất, quay lại màn hình này
                this.Show();
            }
            else
            {
                lblMessage.Text = response.Message;
            }
        }

        // ==========================================
        // 2. XỬ LÝ NÚT THOÁT
        // ==========================================
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}