using System;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Gắn sự kiện click cho nút Đăng nhập
            btnLogin.Click += BtnLogin_Click;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // 1. Tạo tài khoản mẫu cứng (Hardcode)
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (user == "admin" && pass == "123")
            {
                // 2. Khởi tạo màn hình Dashboard
                MainDashboardForm dashboard = new MainDashboardForm();

                // 3. Ẩn form Login
                this.Hide();

                // 4. Mở Dashboard và chờ người dùng thao tác
                dashboard.ShowDialog();

                // 5. Khi Dashboard bị đóng (do ấn Đăng xuất), Code sẽ chạy tiếp xuống đây
                txtPassword.Clear(); // Xóa pass cũ
                this.Show();         // Hiện lại màn hình Login
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu! (Dùng admin/123)", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}