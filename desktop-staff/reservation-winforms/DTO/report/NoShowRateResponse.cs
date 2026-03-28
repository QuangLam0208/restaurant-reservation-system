using Newtonsoft.Json;

namespace reservation_winforms.DTO.report
{
    public class NoShowRateResponse
    {
        [JsonProperty("totalReservations")]
        public long TotalReservations { get; set; }

        [JsonProperty("noShowCount")]
        public long NoShowCount { get; set; }

        [JsonProperty("noShowRate")]
        public double NoShowRate { get; set; }
    }
}