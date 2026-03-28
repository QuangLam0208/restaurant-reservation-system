using Newtonsoft.Json;

namespace reservation_winforms.DTO.auth
{
    public class StaffRegisterRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; } // Dùng string để hứng Enum UserRole (ví dụ: "RECEPTIONIST" hoặc "MANAGER")
    }
}