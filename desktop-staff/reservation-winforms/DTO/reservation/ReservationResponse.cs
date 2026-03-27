using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace reservation_winforms.DTO.reservation
{
    public class ReservationResponse
    {
        [JsonProperty("reservationId")]
        public long ReservationId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // Nhận Enum ReservationStatus (CREATED, RESERVED, SEATED...)

        [JsonProperty("type")]
        public string Type { get; set; } // Nhận Enum ReservationType (ONLINE, WALK_IN)

        [JsonProperty("guestCount")]
        public int GuestCount { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("customerId")]
        public long? CustomerId { get; set; } // Dùng long? vì có thể null (khách vãng lai không lưu DB)

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("tableIds")]
        public List<long> TableIds { get; set; }
    }
}