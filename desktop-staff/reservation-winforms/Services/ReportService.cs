using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reservation_winforms.DTO.report;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class ReportService
    {
        private string GetErrorMessage(string jsonContent)
        {
            try
            {
                var jObject = JObject.Parse(jsonContent);
                if (jObject["message"] != null) return jObject["message"].ToString();
            }
            catch { }
            return jsonContent;
        }

        // 1. Lấy dữ liệu vẽ Biểu đồ
        public async Task<(bool IsSuccess, List<DailyReservationReport> Data, string Message)> GetReservationsByDateAsync(DateTime from, DateTime to)
        {
            try
            {
                ApiClient.AttachToken();
                string fromStr = from.ToString("yyyy-MM-dd");
                string toStr = to.ToString("yyyy-MM-dd");

                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reports/reservations-by-date?from={fromStr}&to={toStr}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<List<DailyReservationReport>>(content), "Thành công");

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }

        // 2. Lấy dữ liệu Tỷ lệ Khách bùng bàn (No-show)
        public async Task<(bool IsSuccess, NoShowRateResponse Data, string Message)> GetNoShowRateAsync(DateTime from, DateTime to)
        {
            try
            {
                ApiClient.AttachToken();
                string fromStr = from.ToString("yyyy-MM-dd");
                string toStr = to.ToString("yyyy-MM-dd");

                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reports/no-show-rate?from={fromStr}&to={toStr}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<NoShowRateResponse>(content), "Thành công");

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }
    }
}