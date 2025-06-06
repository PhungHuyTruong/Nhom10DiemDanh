using API.Data;
using Nhom10ModuleDiemDanh.Models;

namespace Nhom10ModuleDiemDanh.Services
{
    public class BoMonCoSoService : IBoMonCoSoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BoMonCoSoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7296/";
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }

        public async Task<List<BoMonCoSoViewModel>> GetAllAsync(string tenBoMon = null, Guid? idCoSo = null)
        {
            var queryString = new List<string>();
            if (!string.IsNullOrEmpty(tenBoMon))
                queryString.Add($"tenBoMon={Uri.EscapeDataString(tenBoMon)}");
            if (idCoSo.HasValue && idCoSo != Guid.Empty)
                queryString.Add($"idCoSo={idCoSo.Value}");

            var url = "api/BoMonCoSo" + (queryString.Any() ? "?" + string.Join("&", queryString) : "");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi lấy danh sách bộ môn cơ sở: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<BoMonCoSoViewModel>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BoMonCoSoViewModel>();

            return result;
        }

        public async Task<BoMonCoSoViewModel> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/BoMonCoSo/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi lấy bộ môn cơ sở: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<BoMonCoSoViewModel>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<BoMonCoSoViewModel> CreateAsync(BoMonCoSoViewModel model)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new
            {
                model.IdBoMonCoSo,
                IdBoMon = model.IdBoMon,
                IdCoSo = model.IdCoSo.HasValue && model.IdCoSo != Guid.Empty ? model.IdCoSo : null,
                model.TrangThai,
                model.NgayTao,
                model.NgayCapNhat
            }), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/BoMonCoSo", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi tạo bộ môn cơ sở: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            // API trả về danh sách nếu chọn "Tất cả cơ sở", hoặc một bản ghi nếu chọn một cơ sở cụ thể
            if (model.IdCoSo == null || model.IdCoSo == Guid.Empty)
            {
                var result = System.Text.Json.JsonSerializer.Deserialize<List<BoMonCoSoViewModel>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result.FirstOrDefault(); // Trả về bản ghi đầu tiên (hoặc xử lý thêm nếu cần)
            }
            else
            {
                var result = System.Text.Json.JsonSerializer.Deserialize<BoMonCoSoViewModel>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }
        }

        public async Task UpdateAsync(Guid id, BoMonCoSoViewModel model)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new
            {
                model.IdBoMonCoSo,
                model.IdBoMon,
                model.IdCoSo,
                model.TrangThai,
                model.NgayTao,
                model.NgayCapNhat
            }), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/BoMonCoSo/{id}", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi cập nhật bộ môn cơ sở: {error}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/BoMonCoSo/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi xóa bộ môn cơ sở: {error}");
            }
        }

        public async Task<List<CoSo>> GetCoSosAsync()
        {
            var response = await _httpClient.GetAsync("api/BoMonCoSo/GetCoSos");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi lấy danh sách cơ sở: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<CoSo>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new List<CoSo>();
        }

        public async Task<List<QuanLyBoMon>> GetBoMonsAsync()
        {
            var response = await _httpClient.GetAsync("api/BoMonCoSo/GetBoMons");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi lấy danh sách bộ môn: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<QuanLyBoMon>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new List<QuanLyBoMon>();
        }

        public async Task ToggleStatusAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/BoMonCoSo/ToggleStatus/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi khi cập nhật trạng thái: {error}");
            }
        }
    }
}