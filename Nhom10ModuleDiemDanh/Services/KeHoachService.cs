using API.Data;
using Microsoft.EntityFrameworkCore;
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
        private readonly ModuleDiemDanhDbContext _context;
        private readonly string _apiBaseUrl = "https://localhost:7296/";

        public KeHoachService(HttpClient httpClient, ModuleDiemDanhDbContext context)
        {
            _httpClient = httpClient;
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/KeHoach{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<KeHoachViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return new List<KeHoachViewModel>();
        }

        public async Task<KeHoachViewModel> GetKeHoachById(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/KeHoach/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<KeHoachViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task CreateKeHoach(KeHoachViewModel model)
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent(model.TenKeHoach ?? ""), "TenKeHoach" },
                { new StringContent(model.NoiDung ?? ""), "NoiDung" },
                { new StringContent(model.IdDuAn.ToString()), "IdDuAn" },
                { new StringContent(model.ThoiGianBatDau.ToString("o")), "ThoiGianBatDau" },
                { new StringContent(model.ThoiGianKetThuc.ToString("o")), "ThoiGianKetThuc" },
                { new StringContent(model.TrangThai.ToString()), "TrangThai" }
            };

            var response = await _httpClient.PostAsync(_apiBaseUrl + "api/KeHoach", form);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("Không thể tạo kế hoạch: " + error);
            }
        }


        public async Task UpdateKeHoach(KeHoachViewModel model)
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent(model.IdKeHoach.ToString()), "IdKeHoach" },
                { new StringContent(model.TenKeHoach ?? ""), "TenKeHoach" },
                { new StringContent(model.NoiDung ?? ""), "NoiDung" },
                { new StringContent(model.IdDuAn.ToString()), "IdDuAn" },
                { new StringContent(model.ThoiGianBatDau.ToString("o")), "ThoiGianBatDau" },
                { new StringContent(model.ThoiGianKetThuc.ToString("o")), "ThoiGianKetThuc" },
                { new StringContent(model.TrangThai.ToString()), "TrangThai" }
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"{_apiBaseUrl}api/KeHoach/{model.IdKeHoach}")
            {
                Content = form
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("Không thể cập nhật kế hoạch: " + error);
            }
        }


        public async Task DeleteKeHoach(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/KeHoach/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("Không thể xóa kế hoạch: " + error);
            }
        }

        public async Task ToggleStatus(Guid id)
        {
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}api/KeHoach/ToggleStatus/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("Không thể cập nhật trạng thái: " + error);
            }
        }

        public async Task<IEnumerable<dynamic>> GetDuAnList()
        {
            try
            {
                var duAns = await _context.DuAns
                    .Where(d => d.TrangThai) // Chỉ lấy dự án có trạng thái true
                    .Select(d => new { idDuAn = d.IdDuAn, tenDuAn = d.TenDuAn })
                    .ToListAsync();
                return duAns;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi truy vấn danh sách dự án: " + ex.Message);
            }
        }
    }
}