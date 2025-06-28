using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;
using Rotativa.AspNetCore;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichSuDiemDanhController : Controller
    {
        private readonly HttpClient _httpClient;

        public LichSuDiemDanhController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7296/api/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult Index()
        {
            // Lấy IdSinhVien từ session
            var sinhVienId = HttpContext.Session.GetString("SinhVienId");

            // Kiểm tra xem sinh viên đã đăng nhập chưa
            if (string.IsNullOrEmpty(sinhVienId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Truyền IdSinhVien vào ViewBag để sử dụng trong JavaScript
            ViewBag.SinhVienId = sinhVienId;
            return View();
        }

        public async Task<IActionResult> DownloadTemplate(Guid? IdSinhVien, Guid? IdHocKy, Guid? IdNhomXuong)
        {
            // Nếu không truyền IdSinhVien, lấy từ session
            if (!IdSinhVien.HasValue)
            {
                var sinhVienId = HttpContext.Session.GetString("SinhVienId");
                if (!string.IsNullOrEmpty(sinhVienId) && Guid.TryParse(sinhVienId, out Guid id))
                {
                    IdSinhVien = id;
                }
            }
            var queryParams = new List<string>();
            if (IdSinhVien.HasValue) queryParams.Add($"IdSinhVien={IdSinhVien}");
            if (IdHocKy.HasValue) queryParams.Add($"IdHocKy={IdHocKy}");
            if (IdNhomXuong.HasValue) queryParams.Add($"IdNhomXuong={IdNhomXuong}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var response = await _httpClient.GetAsync($"LichSuDiemDanh/download-template{queryString}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Không thể tải template từ API.");

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LichSuDiemDanh.xlsx");
        }

        public async Task<IActionResult> ExportPdf(Guid? IdHocKy, Guid? IdNhomXuong)
        {
            var sinhVienId = HttpContext.Session.GetString("SinhVienId");
            if (string.IsNullOrEmpty(sinhVienId)) return RedirectToAction("Index", "Home");

            var queryParams = new List<string>();
            queryParams.Add($"IdSinhVien={sinhVienId}");
            if (IdHocKy.HasValue) queryParams.Add($"IdHocKy={IdHocKy}");
            if (IdNhomXuong.HasValue) queryParams.Add($"IdNhomXuong={IdNhomXuong}");
            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var response = await _httpClient.GetAsync($"LichSuDiemDanh{queryString}");
            if (!response.IsSuccessStatusCode)
                return Content("Không lấy được dữ liệu để xuất PDF!");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<LichSuDiemDanhViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new Rotativa.AspNetCore.ViewAsPdf("ExportPdf", data)
            {
                FileName = "LichSuDiemDanh.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
    }
}
