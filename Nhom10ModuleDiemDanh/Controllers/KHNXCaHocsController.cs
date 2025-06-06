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
            _httpClient.BaseAddress = new Uri("https://localhost:7296/api/"); // Web API base
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: KHNXCaHocs
        public async Task<IActionResult> Index(string keyword, int? trangThai, string caHoc, DateTime? ngay)
        {
            var response = await _httpClient.GetAsync("KHNXCaHocs");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Filter = null;
                return View(new List<KHNXCaHoc>());
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<KHNXCaHoc>>>();
            var data = result?.data ?? new List<KHNXCaHoc>();

            if (!string.IsNullOrWhiteSpace(keyword))
                data = data.Where(x => (x.NoiDung != null && x.NoiDung.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();

            if (trangThai != null)
                data = data.Where(x => x.TrangThai == trangThai).ToList();

            if (!string.IsNullOrEmpty(caHoc))
                data = data.Where(x => x.Buoi != null && x.Buoi.Equals(caHoc, StringComparison.OrdinalIgnoreCase)).ToList();

            if (ngay.HasValue)
                data = data.Where(x => x.NgayHoc.Date == ngay.Value.Date).ToList();

            // Gửi lại giá trị filter ra View để giữ form lọc
            ViewBag.Filter = new { keyword, trangThai, caHoc, ngay = ngay?.ToString("yyyy-MM-dd") };

            return View(data);
        }


        // GET: KHNXCaHocs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"KHNXCaHocs/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<KHNXCaHoc>>();
            return View(result?.data);
        }

        // GET: KHNXCaHocs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KHNXCaHocs/Create
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([FromBody] KHNXCaHoc model)
{
    if (!ModelState.IsValid)
    {
        return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
    }

    var response = await _httpClient.PostAsJsonAsync("KHNXCaHocs", model);
    if (response.IsSuccessStatusCode)
    {
        return Json(new { success = true });
    }

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KHNXCaHoc model)
        {
            if (id != model.IdNXCH) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var response = await _httpClient.PutAsJsonAsync($"KHNXCaHocs/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi khi cập nhật: {errorMsg}");
            return View(model);
        }

        // POST: KHNXCaHocs/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            var response = await _httpClient.DeleteAsync($"KHNXCaHocs/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = errorMsg });
        }
    }
}
