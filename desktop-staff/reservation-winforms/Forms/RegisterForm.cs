using System;
using System.Windows.Forms;
using reservation_winforms.Services; // Chứa ApiService

namespace reservation_winforms.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();

            // 1. Xóa các role cũ bị sai và nạp lại đúng 2 role của Backend
            cboRole.Items.Clear();
            cboRole.Items.Add("RECEPTIONIST");
            cboRole.Items.Add("MANAGER");

            // Chọn sẵn RECEPTIONIST làm mặc định
            cboRole.SelectedIndex = 0;

            // Gắn sự kiện cho các nút bấm (nếu chưa gắn ở Designer)
            btnCancel.Click += (s, e) => this.Close();
            btnRegister.Click += BtnRegister_Click;
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();
            string role = cboRole.SelectedItem?.ToString();

            // Kiểm tra dữ liệu rỗng
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra độ dài mật khẩu khớp với Backend (@Size(min = 6))
            if (pass.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Khóa nút trong lúc chờ Server xử lý
            btnRegister.Enabled = false;
            btnRegister.Text = "Đang xử lý...";

            // Gọi API
            var apiService = new ApiService();
            try
            {
                string resultMessage = await apiService.RegisterAsync(user, pass, role);
                MessageBox.Show(resultMessage, "Kết quả");

                // Đăng ký thành công thì tự động đóng form
                if (resultMessage.ToLower().Contains("thành công"))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối tới Server: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Mở khóa nút lại nếu có lỗi
                btnRegister.Enabled = true;
                btnRegister.Text = "TẠO TÀI KHOẢN";
            }
        }
    }
}