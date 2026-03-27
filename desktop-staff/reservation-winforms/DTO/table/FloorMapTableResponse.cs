using Newtonsoft.Json;

namespace reservation_winforms.DTO.table
{
    public class FloorMapTableResponse
    {
        [JsonProperty("tableId")]
        public long TableId { get; set; }

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // Enum TableStatus

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("currentReservationId")]
        public long? CurrentReservationId { get; set; } // Có thể null nếu bàn đang trống

        [JsonProperty("currentCustomerName")]
        public string CurrentCustomerName { get; set; }

        [JsonProperty("currentReservationStatus")]
        public string CurrentReservationStatus { get; set; } // Enum ReservationStatus
    }
}