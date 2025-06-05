using Nhom10ModuleDiemDanh.Models;
using System.Text.Json;

namespace Nhom10ModuleDiemDanh.Services
{
    public class CoSoService : ICoSoService
    {
        private readonly HttpClient _httpClient;

        public CoSoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7296/"); // Thay đổi port nếu cần
        }

        public async Task<List<CoSoViewModel>> GetCoSosAsync(string tenCoSo, string trangThai)
        {
            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(tenCoSo))
                parameters.Add($"tenCoSo={Uri.EscapeDataString(tenCoSo)}");
            if (!string.IsNullOrWhiteSpace(trangThai))
                parameters.Add($"trangThai={Uri.EscapeDataString(trangThai)}");

            var query = parameters.Count > 0 ? "?" + string.Join("&", parameters) : "";
            var response = await _httpClient.GetAsync($"api/CoSo{query}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API error: " + error); // Log lỗi chi tiết
                throw new Exception(error);
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CoSoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<CoSoViewModel> GetCoSoAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/CoSo/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CoSoViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CreateCoSoAsync(CoSoViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/CoSo", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateCoSoAsync(Guid id, CoSoViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/CoSo/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCoSoAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/CoSo/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleStatusAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/CoSo/ToggleStatus/{id}", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
