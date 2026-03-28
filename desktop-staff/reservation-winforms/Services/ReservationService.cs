using Newtonsoft.Json;
using reservation_winforms.DTO;
using reservation_winforms.DTO.reservation;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class ReservationService
    {
        // 1. Gửi yêu cầu gợi ý (Soft-lock bàn)
        public async Task<(bool IsSuccess, WalkInSuggestionResponse Data, string Message)> SuggestWalkInAsync(WalkInRequest request)
        {
            try
            {
                ApiClient.AttachToken();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/walk-in/suggest", jsonContent);

                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<WalkInSuggestionResponse>(contentString);
                    return (true, data, "Gợi ý thành công");
                }

                return (false, null, contentString); // Trả về thông báo lỗi từ server nếu có
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 2. Lễ tân bấm XÁC NHẬN (Confirm)
        public async Task<(bool IsSuccess, string Message)> ConfirmWalkInAsync(long suggestionId)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/walk-in/confirm/{suggestionId}", null);
                var contentString = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, contentString);
            }
            catch (Exception ex) { return (false, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 3. Lễ tân bấm HỦY (Cancel)
        public async Task<bool> CancelWalkInSuggestionAsync(long suggestionId)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/walk-in/cancel-suggestion/{suggestionId}", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}