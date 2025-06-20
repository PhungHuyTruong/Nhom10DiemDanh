using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
// Note: We are removing 'using Microsoft.EntityFrameworkCore;' as we no longer access the DbContext directly.
// We might need to define or reference the DTO models. For simplicity, they are defined below.

namespace Nhom10ModuleDiemDanh.Controllers
{
    #region Data Transfer Objects (DTOs) - Represents the data from the API

    // DTO for displaying KeHoachNhomXuong information
    public class KeHoachNhomXuongDto
    {
        public Guid IdKHNX { get; set; }
        public Guid IdNhomXuong { get; set; }
        public Guid IdKeHoach { get; set; }
        public string TenKeHoach { get; set; } // Assumed from API
        public string TenNhomXuong { get; set; } // Assumed from API
        public string ThoiGianThucTe { get; set; }
        public int SoBuoi { get; set; }
        public int SoSinhVien { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int TrangThai { get; set; }
    }

    // DTO for creating a new KeHoachNhomXuong
    public class CreateKeHoachNhomXuongDto
    {
        public Guid IdNhomXuong { get; set; }
        public Guid IdKeHoach { get; set; }
        public string ThoiGianThucTe { get; set; }
        public int SoBuoi { get; set; }
        public int SoSinhVien { get; set; }
        public int TrangThai { get; set; }
    }

    // DTO for updating an existing KeHoachNhomXuong
    public class UpdateKeHoachNhomXuongDto
    {
        public string ThoiGianThucTe { get; set; }
        public int SoBuoi { get; set; }
        public int SoSinhVien { get; set; }
        public int TrangThai { get; set; }
    }

    // DTOs for populating dropdowns
    public class KeHoachDto
    {
        public Guid IdKeHoach { get; set; }
        public string NoiDung { get; set; }
    }

    public class NhomXuongDto
    {
        public Guid IdNhomXuong { get; set; }
        public string MoTa { get; set; }
    }

    #endregion

    public class KeHoachNhomXuongsController : Controller
    {
        private readonly HttpClient _httpClient;

        public KeHoachNhomXuongsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            // Set the base address of the API
            _httpClient.BaseAddress = new Uri("http://localhost:5017/api/");
        }

        // GET: KeHoachNhomXuongs
        public async Task<IActionResult> Index(Guid? idKeHoach)
        {
            var response = await _httpClient.GetAsync("KeHoachNhomXuongs");
            List<KeHoachNhomXuongDto> keHoachNhomXuongs = new List<KeHoachNhomXuongDto>();
            if (response.IsSuccessStatusCode)
            {
                keHoachNhomXuongs = await response.Content.ReadFromJsonAsync<List<KeHoachNhomXuongDto>>();
            }

            // Nếu có idKeHoach, lọc danh sách
            if (idKeHoach.HasValue)
            {
                keHoachNhomXuongs = keHoachNhomXuongs
                    .Where(x => x.IdKeHoach == idKeHoach.Value)
                    .ToList();
            }

            return View(keHoachNhomXuongs);
        }

        // GET: KeHoachNhomXuongs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"KeHoachNhomXuongs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var keHoachNhomXuong = await response.Content.ReadFromJsonAsync<KeHoachNhomXuongDto>();
                return View(keHoachNhomXuong);
            }
            return NotFound();
        }

        // GET: KeHoachNhomXuongs/Create
        public async Task<IActionResult> Create()
        {
            // Populate dropdowns by calling the respective APIs
            await PopulateDropdowns();
            return View();
        }

        // POST: KeHoachNhomXuongs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateKeHoachNhomXuongDto createDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("KeHoachNhomXuongs", createDto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Error creating resource. Please try again.");
            }
            await PopulateDropdowns();
            return View(createDto);
        }

        // GET: KeHoachNhomXuongs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"KeHoachNhomXuongs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var keHoachNhomXuong = await response.Content.ReadFromJsonAsync<KeHoachNhomXuongDto>();
                await PopulateDropdowns(keHoachNhomXuong.IdKeHoach, keHoachNhomXuong.IdNhomXuong);
                return View(keHoachNhomXuong);
            }
            return NotFound();
        }

        // POST: KeHoachNhomXuongs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KeHoachNhomXuongDto keHoachNhomXuongDto)
        {
            if (id != keHoachNhomXuongDto.IdKHNX) return NotFound();

            if (ModelState.IsValid)
            {
                // Map the display DTO to an update DTO
                var updateDto = new UpdateKeHoachNhomXuongDto
                {
                    ThoiGianThucTe = keHoachNhomXuongDto.ThoiGianThucTe,
                    SoBuoi = keHoachNhomXuongDto.SoBuoi,
                    SoSinhVien = keHoachNhomXuongDto.SoSinhVien,
                    TrangThai = keHoachNhomXuongDto.TrangThai
                };

                var response = await _httpClient.PutAsJsonAsync($"KeHoachNhomXuongs/{id}", updateDto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Error updating resource. Please try again.");
            }
            await PopulateDropdowns(keHoachNhomXuongDto.IdKeHoach, keHoachNhomXuongDto.IdNhomXuong);
            return View(keHoachNhomXuongDto);
        }

        // GET: KeHoachNhomXuongs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"KeHoachNhomXuongs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var keHoachNhomXuong = await response.Content.ReadFromJsonAsync<KeHoachNhomXuongDto>();
                return View(keHoachNhomXuong);
            }
            return NotFound();
        }

        // POST: KeHoachNhomXuongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"KeHoachNhomXuongs/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            // Optionally, show an error to the user on the delete page.
            return RedirectToAction(nameof(Delete), new { id = id, error = true });
        }

        // Helper method to populate dropdown lists for Create and Edit views
        private async Task PopulateDropdowns(object selectedKeHoach = null, object selectedNhomXuong = null)
        {
            // Fetch KeHoachs for dropdown
            var keHoachResponse = await _httpClient.GetAsync("KeHoachs");
            if (keHoachResponse.IsSuccessStatusCode)
            {
                var keHoachs = await keHoachResponse.Content.ReadFromJsonAsync<List<KeHoachDto>>();
                // Assumes your KeHoach API returns 'NoiDung' for the display text
                ViewData["IdKeHoach"] = new SelectList(keHoachs, "IdKeHoach", "NoiDung", selectedKeHoach);
            }

            // Fetch NhomXuongs for dropdown
            var nhomXuongResponse = await _httpClient.GetAsync("NhomXuongs");
            if (nhomXuongResponse.IsSuccessStatusCode)
            {
                var nhomXuongs = await nhomXuongResponse.Content.ReadFromJsonAsync<List<NhomXuongDto>>();
                // Assumes your NhomXuong API returns 'MoTa' for the display text
                ViewData["IdNhomXuong"] = new SelectList(nhomXuongs, "IdNhomXuong", "MoTa", selectedNhomXuong);
            }
        }
    }
}
