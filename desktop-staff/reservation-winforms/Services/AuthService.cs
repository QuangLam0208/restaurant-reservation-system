using Newtonsoft.Json;
using reservation_winforms.DTO.auth;
using reservation_winforms.DTO;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class AuthService
    {
        // 1. HÀM ĐĂNG NHẬP
        public async Task<(bool IsSuccess, string Message)> LoginAsync(string username, string password)
        {
            try
            {
                // Gói dữ liệu gửi đi
                var requestData = new StaffLoginRequest { Username = username, Password = password };

                // Chuyển thành JSON
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                // Gọi API Login (Không cần AttachToken vì lúc này chưa đăng nhập)
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/staff/auth/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Đọc dữ liệu JSON trả về
                    var responseString = await response.Content.ReadAsStringAsync();
                    var loginData = JsonConvert.DeserializeObject<StaffLoginResponse>(responseString);

                    // Lưu thông tin vào bộ nhớ dùng chung (GlobalState)
                    GlobalState.StaffToken = loginData.SessionToken;
                    GlobalState.CurrentUsername = loginData.Username;
                    GlobalState.Role = loginData.Role;

                    return (true, "Đăng nhập thành công!");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return (false, "Tài khoản hoặc mật khẩu không chính xác.");
                }
                else
                {
                    return (false, $"Hệ thống đang lỗi (Mã lỗi: {(int)response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối tới Server: {ex.Message}");
            }
        }

        // 2. HÀM ĐĂNG KÝ TÀI KHOẢN MỚI (Dành cho Quản lý)
        public async Task<(bool IsSuccess, string Message)> RegisterStaffAsync(string username, string password, string role = "RECEPTIONIST")
        {
            try
            {
                var requestData = new StaffRegisterRequest
                {
                    Username = username,
                    Password = password,
                    Role = role
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                // Bắt buộc gọi dòng này để đính kèm Token của Manager vào Header thì Spring Boot mới cho phép đi qua
                ApiClient.AttachToken();

                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/staff/auth/register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Tạo tài khoản Lễ tân thành công!");
                }
                else
                {
                    // Nếu Backend trả về lỗi (ví dụ: Username đã tồn tại, pass quá ngắn...)
                    var errorString = await response.Content.ReadAsStringAsync();
                    return (false, $"Đăng ký thất bại: {errorString}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối: {ex.Message}");
            }
        }
    }
}