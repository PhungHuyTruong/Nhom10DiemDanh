using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using Nhom10ModuleDiemDanh.Models; // chứa ResponseWrapper<T>

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class KHNXCaHocsController : Controller
    {
        private readonly HttpClient _httpClient;

        public KHNXCaHocsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7296/api/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: KHNXCaHocs
        [HttpGet]
        public async Task<IActionResult> Index(Guid idKHNX, string tenNhomXuong, string keyword, string trangThai, string caHoc, DateTime? ngay)
        {
            ViewBag.IdKHNX = idKHNX;
            ViewBag.TenNhomXuong = tenNhomXuong;
            var response = await _httpClient.GetAsync("KHNXCaHocs");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Filter = null;
                return View(new List<KHNXCaHoc>());
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<KHNXCaHoc>>>();
            var data = result?.data ?? new List<KHNXCaHoc>();
            data = data.Where(x => x.IdKHNX == idKHNX).ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                data = data.Where(x =>
                    (!string.IsNullOrEmpty(x.NoiDung) && x.NoiDung.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(x.Buoi) && x.Buoi.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // ✅ BỔ SUNG đoạn lọc ca học theo IdCaHoc
            if (!string.IsNullOrEmpty(caHoc))
            {
                if (Guid.TryParse(caHoc, out var caHocGuid))
                {
                    data = data.Where(x => x.IdCaHoc == caHocGuid).ToList();
                }
            }


            // Sau khi lấy danh sách KHNXCaHoc
            var caHocResponse = await _httpClient.GetAsync("KHNXCaHocs/GetCaHocsForDropdown");


            var caHocList = new List<CaHoc>();

            if (caHocResponse.IsSuccessStatusCode)
            {
                var caHocResult = await caHocResponse.Content.ReadFromJsonAsync<ApiResponse<List<CaHoc>>>();

                caHocList = caHocResult?.data ?? new List<CaHoc>();
            }

            ViewBag.CaHocList = caHocList;


            if (ngay.HasValue)
                data = data.Where(x => x.NgayHoc.Date == ngay.Value.Date).ToList();

            if (!string.IsNullOrEmpty(trangThai))
            {
                var now = DateTime.Now;

                if (trangThai == "sapdienra")
                {
                    data = data.Where(x =>
                        x.CaHoc?.ThoiGianBatDau.HasValue == true &&
                        new DateTime(x.NgayHoc.Year, x.NgayHoc.Month, x.NgayHoc.Day,
                                     x.CaHoc.ThoiGianBatDau.Value.Hours,
                                     x.CaHoc.ThoiGianBatDau.Value.Minutes, 0) > now).ToList();
                }
                else if (trangThai == "dangdienra")
                {
                    data = data.Where(x =>
                        x.CaHoc?.ThoiGianBatDau.HasValue == true &&
                        x.CaHoc?.ThoiGianKetThuc.HasValue == true &&
                        new DateTime(x.NgayHoc.Year, x.NgayHoc.Month, x.NgayHoc.Day,
                                     x.CaHoc.ThoiGianBatDau.Value.Hours,
                                     x.CaHoc.ThoiGianBatDau.Value.Minutes, 0) <= now &&
                        new DateTime(x.NgayHoc.Year, x.NgayHoc.Month, x.NgayHoc.Day,
                                     x.CaHoc.ThoiGianKetThuc.Value.Hours,
                                     x.CaHoc.ThoiGianKetThuc.Value.Minutes, 0) >= now).ToList();
                }
                else if (trangThai == "dadienra")
                {
                    data = data.Where(x =>
                        x.CaHoc?.ThoiGianKetThuc.HasValue == true &&
                        new DateTime(x.NgayHoc.Year, x.NgayHoc.Month, x.NgayHoc.Day,
                                     x.CaHoc.ThoiGianKetThuc.Value.Hours,
                                     x.CaHoc.ThoiGianKetThuc.Value.Minutes, 0) < now).ToList();
                }
            }

            ViewBag.Filter = new
            {
                keyword,
                trangThai,
                caHoc,
                ngay = ngay?.ToString("yyyy-MM-dd")
            };

            return View(data);
        }

        // GET: KHNXCaHocs/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            // Gọi API để lấy dữ liệu ca học
            var response = await _httpClient.GetAsync($"KHNXCaHocs/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<KHNXCaHoc>>();
            var caHoc = result?.data;
            if (caHoc == null)
                return NotFound();

            // Trả về JSON cho JS
            return Json(new
            {
                idNXCH = caHoc.IdNXCH,
                idKHNX = caHoc.IdKHNX,
                buoi = caHoc.Buoi,
                ngayHoc = caHoc.NgayHoc,
                idCaHoc = caHoc.IdCaHoc,
                noiDung = caHoc.NoiDung,
                linkOnline = caHoc.LinkOnline,
                diemDanhTre = caHoc.DiemDanhTre
            });
        }

        // GET: KHNXCaHocs/Create
        public IActionResult Create() => View();

        // POST: KHNXCaHocs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] KHNXCaHoc model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            var response = await _httpClient.PostAsJsonAsync("KHNXCaHocs", model);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            var errorMsg = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = errorMsg });
        }

        // GET: KHNXCaHocs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"KHNXCaHocs/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<KHNXCaHoc>>();
            return View(result?.data);
        }

        // POST: KHNXCaHocs/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [FromBody] KHNXCaHoc model)
        {
            if (id != model.IdNXCH)
                return BadRequest(new { success = false, message = "ID không khớp" });

            var response = await _httpClient.PutAsJsonAsync($"KHNXCaHocs/{id}", model);
            if (response.IsSuccessStatusCode)
                return Ok(new { success = true });

            var errorMsg = await response.Content.ReadAsStringAsync();
            return BadRequest(new { success = false, message = errorMsg });
        }

        // POST: KHNXCaHocs/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            var response = await _httpClient.DeleteAsync($"KHNXCaHocs/{id}");
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            var errorMsg = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = errorMsg });
        }

        public async Task<IActionResult> DownloadTemplate(Guid idKHNX)
        {
            var response = await _httpClient.GetAsync($"KHNXCaHocs/download-template?idKHNX={idKHNX}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Không thể tải template từ API.");

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateCaHoc.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file, Guid idKHNX)
        {
            try
            {
                if (file == null || file.Length <= 0)
                {
                    TempData["Error"] = "File không hợp lệ.";
                    return RedirectToAction("Index", new { idKHNX });
                }

                Console.WriteLine($"Client side - IdKHNX: {idKHNX}");
                Console.WriteLine($"Client side - File name: {file.FileName}");

                var content = new MultipartFormDataContent();
                var stream = new StreamContent(file.OpenReadStream());
                stream.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(stream, "file", file.FileName);
                content.Add(new StringContent(idKHNX.ToString()), "idKHNX");

                var response = await _httpClient.PostAsync("KHNXCaHocs/import-excel", content);

                // Đọc response detail nếu có lỗi
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error Response: {errorContent}");
                    TempData["Error"] = $"Import thất bại. Chi tiết: {errorContent}";
                    return RedirectToAction("Index", new { idKHNX });
                }

                TempData["Success"] = "Import thành công!";
                return RedirectToAction("Index", new { idKHNX });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client Error: {ex.Message}");
                TempData["Error"] = $"Lỗi khi import: {ex.Message}";
                return RedirectToAction("Index", new { idKHNX });
            }
        }

    }
}