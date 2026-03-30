using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reservation_winforms.DTO;
using reservation_winforms.DTO.table;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class TableService
    {
        // =========================================================
        // HÀM HELPER: Bóc tách JSON lấy đúng câu thông báo lỗi
        // =========================================================
        private string GetErrorMessage(string jsonContent)
        {
            try
            {
                // Phân tích chuỗi JSON
                var jObject = JObject.Parse(jsonContent);
                // Nếu có trường "message" thì chỉ lấy đúng trường đó
                if (jObject["message"] != null)
                {
                    return jObject["message"].ToString();
                }
            }
            catch
            {
                // Bỏ qua lỗi nếu chuỗi trả về không phải là định dạng JSON hợp lệ
            }

            // Trả về nguyên gốc nếu không móc được "message"
            return jsonContent;
        }

        // =========================================================
        // CÁC HÀM GỌI API
        // =========================================================

        // Hàm gọi API lấy danh sách sơ đồ bàn (Floor Map)
        public async Task<(bool IsSuccess, List<FloorMapTableResponse> Data, string Message)> GetFloorMapAsync()
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/tables/floor-map");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tables = JsonConvert.DeserializeObject<List<FloorMapTableResponse>>(content);
                    return (true, tables, "Thành công");
                }

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 1. Lấy TẤT CẢ các bàn (Quản lý)
        public async Task<(bool IsSuccess, List<TableResponse> Data, string Message)> GetAllTablesAsync()
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/tables");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tables = JsonConvert.DeserializeObject<List<TableResponse>>(content);
                    return (true, tables, "Thành công");
                }

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }

        // 2. Thêm bàn mới
        public async Task<(bool IsSuccess, string Message)> CreateTableAsync(TableRequest req)
        {
            try
            {
                ApiClient.AttachToken();
                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/tables", json);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return (true, "Thêm bàn thành công!");

                return (false, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, $"Lỗi: {ex.Message}"); }
        }

        // 3. Cập nhật bàn
        public async Task<(bool IsSuccess, string Message)> UpdateTableAsync(long id, TableRequest req)
        {
            try
            {
                ApiClient.AttachToken();
                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                var response = await ApiClient.Client.PutAsync($"{ApiClient.BaseUrl}/tables/{id}", json);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return (true, "Cập nhật thành công!");

                return (false, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, $"Lỗi: {ex.Message}"); }
        }

        // 4. Xóa/Vô hiệu hóa bàn
        public async Task<(bool IsSuccess, string Message)> DeleteTableAsync(long id)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.DeleteAsync($"{ApiClient.BaseUrl}/tables/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return (true, "Vô hiệu hóa bàn thành công!");

                return (false, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, $"Lỗi: {ex.Message}"); }
        }
    }
}