using System.Text.Json.Serialization;

namespace reservation_winforms.Models
{
    public class StaffLoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class StaffLoginResponse
    {
        [JsonPropertyName("sessionToken")]
        public string SessionToken { get; set; }

        [JsonPropertyName("accountId")]
        public long AccountId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class StaffRegisterRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
[JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } // Map với UserRole enum (thường truyền dạng chuỗi như "STAFF", "ADMIN")
    }
}