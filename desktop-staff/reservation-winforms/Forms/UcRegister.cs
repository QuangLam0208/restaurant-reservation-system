using System;
using System.Windows.Forms;
using reservation_winforms.Services; // Chứa ApiService

namespace reservation_winforms.Forms
{
    public partial class UcRegister : UserControl
    {
        private readonly AuthService _authService;
        public UcRegister()
        {
            InitializeComponent();
            _authService = new AuthService();
            cboRole.Items.Clear();
            cboRole.Items.Add("RECEPTIONIST");
            cboRole.SelectedIndex = 0;
            cboRole.Enabled = false;

            btnRegister.Click += BtnRegister_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();
            string role = cboRole.SelectedItem.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblMessage.Text = "Vui lòng nhập đủ thông tin!";
                return;
            }

            btnRegister.Enabled = false;
            btnRegister.Text = "Đang xử lý...";
            lblMessage.Text = "";

            // Gọi hàm đăng ký mà chúng ta đã viết ban nãy
            var response = await _authService.RegisterStaffAsync(user, pass, role);

            btnRegister.Enabled = true;
            btnRegister.Text = "TẠO TÀI KHOẢN";

            if (response.IsSuccess)
            {
                MessageBox.Show("Tạo tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUsername.Clear();
                txtPassword.Clear();
            }
            else
            {
                lblMessage.Text = response.Message;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Xóa rỗng các ô nhập liệu
            txtUsername.Clear();
            txtPassword.Clear();
            lblMessage.Text = "";
        }
    }
}