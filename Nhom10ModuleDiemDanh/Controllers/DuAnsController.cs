using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Dtos;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class DuAnsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ModuleDiemDanhDbContext _dbContext;
        private readonly string _apiUrl = "https://localhost:7296/api/DuAn";

        public DuAnsController(IHttpClientFactory httpClientFactory, ModuleDiemDanhDbContext dbContext)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
            _dbContext = dbContext;
        }

        // GET: DuAns
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(_apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var duAnList = await response.Content.ReadFromJsonAsync<List<DuAnDto>>();
                return View(duAnList);
            }

            return View(new List<DuAn>());
        }

        // 🧠 Tải danh sách dropdown dùng chung
        private async Task LoadDropdownsAsync()
        {
            var capDoList = await _dbContext.CapDoDuAns.ToListAsync();
            var boMonList = await _dbContext.QuanLyBoMons.ToListAsync();
            var hocKyList = await _dbContext.HocKys.ToListAsync();

            ViewBag.IdCDDA = new SelectList(capDoList, "IdCDDA", "TenCapDoDuAn");
            ViewBag.IdBoMon = new SelectList(boMonList, "IDBoMon", "TenBoMon");
            ViewBag.IdHocKy = new SelectList(hocKyList, "IdHocKy", "TenHocKy");
        }

        // GET: DuAns/Create (modal)
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return PartialView("Create", new DuAn());
        }

        // POST: DuAns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDuAn,TenDuAn,MoTa,IdCDDA,IdBoMon,IdHocKy,TrangThai")] DuAn duAn)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return PartialView("Create", duAn); // ⚠️ Vẫn là PartialView vì dùng modal
            }

            duAn.NgayTao = DateTime.Now;
            duAn.NgayCapNhat = null;

            var response = await _httpClient.PostAsJsonAsync(_apiUrl, duAn);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true }); // ✅ Cho JS biết thành công và reload
            }

            ModelState.AddModelError(string.Empty, "Lỗi khi gọi API tạo dự án.");
            await LoadDropdownsAsync();
            return PartialView("Create", duAn);
        }

        // GET: DuAns/Edit
        [HttpGet] // ⚠️ BẮT BUỘC có attribute này để route GET khớp khi gọi từ browser
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<DuAnDto>();
            if (dto == null) return NotFound();
            await LoadDropdownsAsync();
            return PartialView("Edit", dto); // Modal edit
        }

        // POST: DuAns/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DuAnDto dto)
        {
            if (dto.IdDuAn == Guid.Empty)
                return NotFound();
            var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{dto.IdDuAn}", dto);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            await LoadDropdownsAsync();
            return PartialView("Edit", dto);
        }

        // DELETE: nếu cần
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var duAn = await response.Content.ReadFromJsonAsync<DuAn>();
            return View(duAn);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
