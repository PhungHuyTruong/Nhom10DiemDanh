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
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5017/";
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }

        public async Task<List<BoMonCoSoViewModel>> GetAllAsync(string tenBoMon = null, Guid? idCoSo = null)
        {
            var queryString = new List<string>();
            if (!string.IsNullOrEmpty(tenBoMon))
                queryString.Add($"tenBoMon={Uri.EscapeDataString(tenBoMon)}");
            if (idCoSo.HasValue)
                queryString.Add($"idCoSo={idCoSo.Value}");

            var url = "BoMonCoSo" + (queryString.Any() ? "?" + string.Join("&", queryString) : "");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<BoMonCoSoViewModel>();

            return await response.Content.ReadFromJsonAsync<List<BoMonCoSoViewModel>>() ?? new List<BoMonCoSoViewModel>();
        }

        public async Task<BoMonCoSoViewModel> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"BoMonCoSo/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<BoMonCoSoViewModel>();
        }

        public async Task<BoMonCoSoViewModel> CreateAsync(BoMonCoSoViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("BoMonCoSo", model);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<BoMonCoSoViewModel>();
        }

        public async Task UpdateAsync(Guid id, BoMonCoSoViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"BoMonCoSo/{id}", model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Cập nhật thất bại");
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"BoMonCoSo/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Xóa thất bại");
        }

        public async Task<List<CoSo>> GetCoSosAsync()
        {
            var response = await _httpClient.GetAsync("BoMonCoSo/GetCoSos");
            if (!response.IsSuccessStatusCode)
                return new List<CoSo>();

            return await response.Content.ReadFromJsonAsync<List<CoSo>>() ?? new List<CoSo>();
        }

        public async Task<List<QuanLyBoMon>> GetBoMonsAsync()
        {
            var response = await _httpClient.GetAsync("BoMonCoSo/GetBoMons");
            if (!response.IsSuccessStatusCode)
                return new List<QuanLyBoMon>();

            return await response.Content.ReadFromJsonAsync<List<QuanLyBoMon>>() ?? new List<QuanLyBoMon>();
        }
    }
}
