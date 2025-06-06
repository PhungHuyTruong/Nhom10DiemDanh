using Nhom10ModuleDiemDanh.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Services
{
    public class KeHoachService : IKeHoachService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7296/";

        public KeHoachService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<KeHoachViewModel>> GetAllKeHoachs(string tuKhoa = "", string trangThai = "", string idBoMon = "", string idCapDoDuAn = "", string idHocKy = "", string namHoc = "")
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(tuKhoa)) queryParams.Add($"tuKhoa={Uri.EscapeDataString(tuKhoa)}");
            if (!string.IsNullOrEmpty(trangThai)) queryParams.Add($"trangThai={Uri.EscapeDataString(trangThai)}");
            if (!string.IsNullOrEmpty(idBoMon)) queryParams.Add($"idBoMon={Uri.EscapeDataString(idBoMon)}");
            if (!string.IsNullOrEmpty(idCapDoDuAn)) queryParams.Add($"idCapDoDuAn={Uri.EscapeDataString(idCapDoDuAn)}");
            if (!string.IsNullOrEmpty(idHocKy)) queryParams.Add($"idHocKy={Uri.EscapeDataString(idHocKy)}");
            if (!string.IsNullOrEmpty(namHoc)) queryParams.Add($"namHoc={Uri.EscapeDataString(namHoc)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<KeHoachViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return new List<KeHoachViewModel>();
        }

        public async Task<KeHoachViewModel> GetKeHoachById(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<KeHoachViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return null;
        }

        public async Task CreateKeHoach(KeHoachViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiBaseUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Không thể tạo kế hoạch.");
            }
        }

        public async Task UpdateKeHoach(KeHoachViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/{model.IdKeHoach}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Không thể cập nhật kế hoạch.");
            }
        }

        public async Task DeleteKeHoach(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Không thể xóa kế hoạch.");
            }
        }

        public async Task ToggleStatus(Guid id)
        {
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/ToggleStatus/{id}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Không thể cập nhật trạng thái.");
            }
        }
    }
}