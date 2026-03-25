using System;
using System.Windows.Forms;
using reservation_winforms.Services; // Chứa ApiService

namespace reservation_winforms.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Gắn sự kiện Click cho cả 3 nút
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += btnRegister_Click;// Gọi hàm Đăng ký
            btnExit.Click += btnExit_Click;         // Gọi hàm Thoát
        }


        // ==========================================
        // 1. XỬ LÝ NÚT ĐĂNG NHẬP (GỌI API)
        // ==========================================
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblMessage.Text = "Vui lòng nhập tên đăng nhập và mật khẩu!";
                return;
            }

            // Hiệu ứng đang xử lý
            btnLogin.Enabled = false;
            btnLogin.Text = "Đang kiểm tra...";
            lblMessage.Text = "";

            var apiService = new ApiService();
            var response = await apiService.LoginAsync(user, pass);

            btnLogin.Enabled = true;
            btnLogin.Text = "ĐĂNG NHẬP";

            if (response != null)
            {
                // Đăng nhập thành công, mở Dashboard
                MainDashboardForm dashboard = new MainDashboardForm();
                this.Hide();
                dashboard.ShowDialog();

                // Sau khi đăng xuất (tắt Dashboard), quay lại đây
                txtPassword.Clear();
                this.Show();
            }
            else
            {
                lblMessage.Text = "Sai tên đăng nhập hoặc mật khẩu!";
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            this.Hide();

            // 2. Mở form Đăng ký lên và chờ người dùng thao tác
            RegisterForm register = new RegisterForm();
            register.ShowDialog();

            // 3. Sau khi form Đăng ký bị đóng (bấm Hủy hoặc Đăng ký thành công), 
            // code sẽ chạy tiếp xuống đây và hiện lại form Đăng nhập
            this.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}