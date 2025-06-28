using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using Rotativa.AspNetCore;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichGiangDaysController : Controller
    {
        private readonly HttpClient _httpClient;

        public LichGiangDaysController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7296/");
        }

        // GET: LichGiangDays
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string hinhThuc, string xuong, string caHoc)
        {
            var response = await _httpClient.GetAsync("api/LichGiangDays");
            if (!response.IsSuccessStatusCode)
                return View(new List<LichGiangDayDTO>());

            var data = await response.Content.ReadFromJsonAsync<List<LichGiangDayDTO>>();

            if (fromDate.HasValue)
                data = data.Where(x => x.NgayTao.Date >= fromDate.Value.Date).ToList();

            if (toDate.HasValue)
                data = data.Where(x => x.NgayTao.Date <= toDate.Value.Date).ToList();

            if (!string.IsNullOrEmpty(hinhThuc))
                data = data.Where(x => x.HTGiangDay != null && x.HTGiangDay.Equals(hinhThuc, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(xuong))
                data = data.Where(x => x.TenNhomXuong != null && x.TenNhomXuong.Contains(xuong, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(caHoc))
                data = data.Where(x => x.TenCaHoc != null && x.TenCaHoc.Contains(caHoc, StringComparison.OrdinalIgnoreCase)).ToList();

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf(DateTime? fromDate, DateTime? toDate, string hinhThuc, string xuong, string caHoc)
        {
            var response = await _httpClient.GetAsync("api/LichGiangDays");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var data = await response.Content.ReadFromJsonAsync<List<LichGiangDayDTO>>();

            if (fromDate.HasValue)
                data = data.Where(x => x.NgayTao.Date >= fromDate.Value.Date).ToList();

            if (toDate.HasValue)
                data = data.Where(x => x.NgayTao.Date <= toDate.Value.Date).ToList();

            if (!string.IsNullOrEmpty(hinhThuc))
                data = data.Where(x => x.HTGiangDay != null && x.HTGiangDay.Equals(hinhThuc, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(xuong))
                data = data.Where(x => x.TenNhomXuong != null && x.TenNhomXuong.Contains(xuong, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(caHoc))
                data = data.Where(x => x.TenCaHoc != null && x.TenCaHoc.Contains(caHoc, StringComparison.OrdinalIgnoreCase)).ToList();

            return new ViewAsPdf("PdfExport", data)
            {
                FileName = $"LichGiangDay_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"api/LichGiangDays/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<LichGiangDayDTO>();
            return View(dto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichGiangDayDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/LichGiangDays", dto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Không thể tạo mới lịch giảng dạy.");
            return View(dto);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"api/LichGiangDays/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<LichGiangDayDTO>();
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LichGiangDayDTO dto)
        {
            if (id != dto.IdLichDay) return BadRequest();

            var response = await _httpClient.PutAsJsonAsync($"api/LichGiangDays/{id}", dto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Không thể cập nhật lịch giảng dạy.");
            return View(dto);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"api/LichGiangDays/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<LichGiangDayDTO>();
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/LichGiangDays/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
