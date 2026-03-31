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
        public async Task<(bool IsSuccess, List<DailyReservationReport> Data, string Message)> GetReservationsByDateAsync(string from, string to)
        {
            return await GetChartDataAsync($"reservations-by-date?from={from}&to={to}");
        }

        public async Task<(bool IsSuccess, List<DailyReservationReport> Data, string Message)> GetReservationsByMonthAsync(string from, string to)
        {
            return await GetChartDataAsync($"reservations-by-month?from={from}&to={to}");
        }

        public async Task<(bool IsSuccess, List<DailyReservationReport> Data, string Message)> GetReservationsByYearAsync(string from, string to)
        {
            return await GetChartDataAsync($"reservations-by-year?from={from}&to={to}");
        }
        private async Task<(bool, List<DailyReservationReport>, string)> GetChartDataAsync(string endpoint)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reports/{endpoint}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<List<DailyReservationReport>>(content), "Thành công");

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }


        // 2. Lấy dữ liệu Tỷ lệ Khách bùng bàn (No-show)
        public async Task<(bool IsSuccess, NoShowRateResponse Data, string Message)> GetNoShowRateAsync(string from, string to)
        {
            return await GetRateDataAsync($"no-show-rate?from={from}&to={to}");
        }

        public async Task<(bool IsSuccess, NoShowRateResponse Data, string Message)> GetNoShowRateByMonthAsync(string from, string to)
        {
            return await GetRateDataAsync($"no-show-rate-by-month?from={from}&to={to}");
        }

        public async Task<(bool IsSuccess, NoShowRateResponse Data, string Message)> GetNoShowRateByYearAsync(string from, string to)
        {
            return await GetRateDataAsync($"no-show-rate-by-year?from={from}&to={to}");
        }
        private async Task<(bool, NoShowRateResponse, string)> GetRateDataAsync(string endpoint)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reports/{endpoint}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<NoShowRateResponse>(content), "Thành công");

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }
    }
}