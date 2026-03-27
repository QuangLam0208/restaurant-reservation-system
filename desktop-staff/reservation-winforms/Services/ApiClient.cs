using System.Net.Http;
using System.Net.Http.Headers;

namespace reservation_winforms.Services
{
    // Cung cấp 1 HttpClient dùng chung cho toàn bộ App để tối ưu hiệu năng
    public static class ApiClient
    {
        public static readonly HttpClient Client = new HttpClient();
        public static readonly string BaseUrl = "http://localhost:8081/api";

        // Hàm này dùng để gắn Token vào mỗi khi gửi Request (Trừ lúc Login)
        public static void AttachToken()
        {
            if (!string.IsNullOrEmpty(GlobalState.StaffToken))
            {
                // Cách 1: Header chuẩn Bearer
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", GlobalState.StaffToken);

                // Cách 2: Header tùy chỉnh
                Client.DefaultRequestHeaders.Remove("X-Staff-Token");
                Client.DefaultRequestHeaders.Add("X-Staff-Token", GlobalState.StaffToken);
            }
        }
    }
}