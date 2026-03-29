using Newtonsoft.Json;
using reservation_winforms.DTO.table;
using reservation_winforms.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class TableService
    {
        // Hàm gọi API lấy danh sách sơ đồ bàn
        public async Task<(bool IsSuccess, List<FloorMapTableResponse> Data, string Message)> GetFloorMapAsync()
        {
            try
            {
                // Đính kèm Token trước khi gọi API bảo mật
                ApiClient.AttachToken();

                // Gọi GET /api/tables/floor-map
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/tables/floor-map");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    // Ép kiểu JSON thành 1 List các bàn
                    var tables = JsonConvert.DeserializeObject<List<FloorMapTableResponse>>(responseString);
                    return (true, tables, "Thành công");
                }
                else
                {
                    return (false, null, $"Lỗi từ server: {(int)response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return (false, null, $"Lỗi kết nối: {ex.Message}");
            }
        }
    }
}