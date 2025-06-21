using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly HttpClient _client;
        private const int PageSize = 10;

        public SinhVienController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi");
            client.BaseAddress = new Uri("https://localhost:7296/api/");
            _client = client;
        }

        public async Task<IActionResult> Index(string searchMa, bool? trangThai, int page = 1)
        {
            var response = await _client.GetAsync("SinhVien");

            if (!response.IsSuccessStatusCode)
                return View(new List<SinhVien>());

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return View(new List<SinhVien>());

            var data = JsonSerializer.Deserialize<List<SinhVien>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!string.IsNullOrEmpty(searchMa))
                data = data.Where(sv => sv.MaSinhVien.Contains(searchMa, StringComparison.OrdinalIgnoreCase)).ToList();

            if (trangThai.HasValue)
                data = data.Where(sv => sv.TrangThai == trangThai.Value).ToList();

            int totalItems = data.Count;

            var sinhViens = data
                .OrderByDescending(sv => sv.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.SearchMa = searchMa;
            ViewBag.TrangThai = trangThai;

            return View(sinhViens);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(SinhVien model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("SinhVien", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"SinhVien/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var sinhVien = JsonSerializer.Deserialize<SinhVien>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(sinhVien);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SinhVien model)
        {
            if (!ModelState.IsValid)
                return PartialView("Edit", model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"SinhVien/{model.IdSinhVien}", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            ModelState.AddModelError("", "Cập nhật thất bại.");
            return PartialView("Edit", model);
        }

        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var response = await _client.GetAsync($"SinhVien/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var sinhVien = JsonSerializer.Deserialize<SinhVien>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (sinhVien == null) return NotFound();

            sinhVien.TrangThai = !sinhVien.TrangThai;
            sinhVien.NgayCapNhat = DateTime.Now;

            var putJson = JsonSerializer.Serialize(sinhVien);
            var content = new StringContent(putJson, Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"SinhVien/{id}", content);
            if (!putResponse.IsSuccessStatusCode)
                return BadRequest("Không thể cập nhật trạng thái.");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _client.DeleteAsync($"SinhVien/{id}");
            if (!response.IsSuccessStatusCode)
                return BadRequest();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DownloadTemplate()
        {
            var response = await _client.GetAsync("SinhVien/download-template");

            if (!response.IsSuccessStatusCode)
                return NotFound("Không thể tải template từ API.");

            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = "DanhSachSinhVien.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                ModelState.AddModelError("", "File không hợp lệ.");
                return RedirectToAction("Index");
            }

            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            var response = await _client.PostAsync("SinhVien/import-excel", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Import thành công!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Import thất bại.";
            return RedirectToAction("Index");
        }



    }
}
