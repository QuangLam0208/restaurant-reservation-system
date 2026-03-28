using Newtonsoft.Json;

namespace reservation_winforms.DTO.table
{
    public class TableRequest
    {
        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true; // Mặc định là true khi tạo mới

        [JsonProperty("status")]
        public string Status { get; set; } // Hứng Enum TableStatus (VD: "AVAILABLE", "MAINTENANCE")

        [JsonProperty("version")]
        public long? Version { get; set; } // Dùng cho Optimistic Locking khi Manager cập nhật bàn
    }
}