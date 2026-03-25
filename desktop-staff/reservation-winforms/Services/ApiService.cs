using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
// QUAN TRỌNG: Đã xóa System.Text.Json và thay bằng Newtonsoft.Json
using Newtonsoft.Json;

namespace reservation_winforms.Services
{
    // ========================================================
    // 1. ĐẶT CÁC CLASS MODEL VÀO ĐÂY ĐỂ TRÁNH LỖI "NOT FOUND"
    // ========================================================
    public class StaffLoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class StaffLoginResponse
    {
        [JsonProperty("sessionToken")]
        public string SessionToken { get; set; }

        [JsonProperty("accountId")]
        public long AccountId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class StaffRegisterRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }


    // ========================================================
    // 2. CLASS XỬ LÝ API CHÍNH
    // ========================================================
    public class ApiService
    {
        private static readonly string BaseUrl = "http://localhost:8081/api/staff/auth";
        private static readonly HttpClient client = new HttpClient();
        public static string CurrentToken { get; private set; }
        public static string CurrentRole { get; private set; }

        public async Task<StaffLoginResponse> LoginAsync(string username, string password)
        {
            var loginData = new StaffLoginRequest { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{BaseUrl}/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<StaffLoginResponse>(responseString);

                    // LƯU LẠI TOKEN VÀ CHỨC VỤ SAU KHI ĐĂNG NHẬP THÀNH CÔNG
                    CurrentToken = result.SessionToken;
                    CurrentRole = result.Role;

                    // Gắn token vào Header để dùng cho mọi API gửi đi sau này (như API tạo tài khoản)
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("X-Staff-Token", CurrentToken);

                    return result;
                }
            }
            catch (Exception) { }
            return null;
        }

        public async Task<string> RegisterAsync(string username, string password, string role)
        {
            var regData = new StaffRegisterRequest { Username = username, Password = password, Role = role };
            var json = JsonConvert.SerializeObject(regData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{BaseUrl}/register", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return "Tạo tài khoản thành công!";
                return "Lỗi server: " + responseString;
            }
            catch (Exception ex) { return "Lỗi kết nối: " + ex.Message; }
        }
    }
}