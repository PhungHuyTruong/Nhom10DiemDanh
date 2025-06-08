using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Data;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class DuAnsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7296/api/DuAns";
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DuAnsController()
        {
            _httpClient = new HttpClient();
        }

        // GET: DuAns
        public async Task<IActionResult> Index(string? search, string? capDo, string? hocKy, string? monHoc, string? trangThai)
        {
            var url = $"{_apiUrl}?search={search}&capDo={capDo}&hocKy={hocKy}&monHoc={monHoc}&trangThai={trangThai}";
            var response = await _httpClient.GetAsync(url);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Lỗi API: {raw}";
                return View(new List<DuAn>());
            }

            var result = JsonSerializer.Deserialize<ApiResult<List<DuAn>>>(raw, _jsonOptions);
            if (result == null || !result.Success || result.Data == null)
            {
                TempData["Error"] = "Không thể lấy danh sách dự án.";
                return View(new List<DuAn>());
            }

            ViewBag.Search = search;
            return View(result.Data);
        }

        // GET: DuAns/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return NotFound(raw);

            var result = JsonSerializer.Deserialize<ApiResult<DuAn>>(raw, _jsonOptions);
            return View(result?.Data);
        }

        // GET: DuAns/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: DuAns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDuAn,CapDuAn,MoTa,IdCDDA,IdBoMon,IdHocKy")] DuAn duAn)
        {
            if (!ModelState.IsValid)
                return PartialView(duAn);

            var content = new StringContent(JsonSerializer.Serialize(duAn), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            var raw = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? Ok() : BadRequest(raw);
        }

        // GET: DuAns/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return NotFound(raw);

            var result = JsonSerializer.Deserialize<ApiResult<DuAn>>(raw, _jsonOptions);
            return PartialView(result?.Data);
        }

        // POST: DuAns/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(DuAn duAn)
        {
            if (!ModelState.IsValid)
                return PartialView(duAn);

            var content = new StringContent(JsonSerializer.Serialize(duAn), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}?id={duAn.IdDuAn}", content);
            var raw = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? Ok() : BadRequest(raw);
        }

        // POST: DuAns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi xóa dự án: {raw}");
            }

            return RedirectToAction(nameof(Index));
        }

        // Class tạm chứa định dạng API response
        private class ApiResult<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
