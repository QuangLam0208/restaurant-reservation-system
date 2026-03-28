using Newtonsoft.Json;

namespace reservation_winforms.DTO.report
{
    public class DailyReservationReport
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}