using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom10ModuleDiemDanh.Models;
using API.Data;
using System.Net.Http.Json;
using System.Text.Json;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class KeHoachNhomXuongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBase = "https://localhost:7296/api/";

        public KeHoachNhomXuongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(Guid? idKeHoach)
        {
            var client = _httpClientFactory.CreateClient();
            var allData = new List<KeHoachNhomXuong>();

            if (idKeHoach.HasValue)
            {
                allData = await client.GetFromJsonAsync<List<KeHoachNhomXuong>>($"{_apiBase}KeHoachNhomXuongs/ByKeHoach/{idKeHoach.Value}");
                ViewBag.TenKeHoach = allData?.FirstOrDefault()?.KeHoach?.TenKeHoach ?? "Không rõ";
                ViewBag.IdKeHoach = idKeHoach.Value;
            }
            else
            {
                allData = await client.GetFromJsonAsync<List<KeHoachNhomXuong>>($"{_apiBase}KeHoachNhomXuongs");
                ViewBag.TenKeHoach = "Tất cả kế hoạch";
                ViewBag.IdKeHoach = null;
            }
            return View(allData ?? new List<KeHoachNhomXuong>());
        }

        public async Task<IActionResult> ByKeHoach(Guid idKeHoach)
        {
            return RedirectToAction("Index", new { idKeHoach = idKeHoach });
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var item = await client.GetFromJsonAsync<KeHoachNhomXuong>($"{_apiBase}KeHoachNhomXuongs/{id}");
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create(Guid idKeHoach)
        {
            var model = new KeHoachNhomXuong { IdKeHoach = idKeHoach };
            await PopulateNhomXuongDropdown(idKeHoach);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KeHoachNhomXuong model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateNhomXuongDropdown(model.IdKeHoach);
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var responsePost = await client.PostAsJsonAsync($"{_apiBase}KeHoachNhomXuongs", model);

            if (responsePost.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { idKeHoach = model.IdKeHoach });
            }

            // Xử lý lỗi từ API (nếu có)
            var errorContent = await responsePost.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi từ API: {errorContent}");
            await PopulateNhomXuongDropdown(model.IdKeHoach);
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var model = await client.GetFromJsonAsync<KeHoachNhomXuong>($"{_apiBase}KeHoachNhomXuongs/{id}");
            if (model == null) return NotFound();
            await PopulateNhomXuongDropdown(model.IdKeHoach, false);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KeHoachNhomXuong model)
        {
            if (id != model.IdKHNX) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateNhomXuongDropdown(model.IdKeHoach, false);
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"{_apiBase}KeHoachNhomXuongs/{id}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { idKeHoach = model.IdKeHoach });
            }

            ModelState.AddModelError(string.Empty, "Không thể cập nhật.");
            await PopulateNhomXuongDropdown(model.IdKeHoach, false);
            return View(model);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var item = await client.GetFromJsonAsync<KeHoachNhomXuong>($"{_apiBase}KeHoachNhomXuongs/{id}");
            var idKeHoach = item?.IdKeHoach;

            var response = await client.DeleteAsync($"{_apiBase}KeHoachNhomXuongs/{id}");
            return RedirectToAction("Index", new { idKeHoach = idKeHoach });
        }

        // HÀM NÀY VẪN CẦN THIẾT CHO FORM CREATE/EDIT
        private async Task PopulateNhomXuongDropdown(Guid idKeHoach, bool filterByKeHoach = true)
        {
            var client = _httpClientFactory.CreateClient();
            string apiUrl = filterByKeHoach
                ? $"{_apiBase}KeHoachNhomXuongs/NhomXuongs/ByKeHoach/{idKeHoach}"
                : $"{_apiBase}NhomXuongs";

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var nhomXuongs = await response.Content.ReadFromJsonAsync<List<NhomXuong>>();
                    ViewBag.DanhSachNhomXuong = new SelectList(nhomXuongs, "IdNhomXuong", "TenNhomXuong");
                }
                else
                {
                    ViewBag.DanhSachNhomXuong = new SelectList(new List<NhomXuong>(), "IdNhomXuong", "TenNhomXuong");
                }
            }
            catch
            {
                ViewBag.DanhSachNhomXuong = new SelectList(new List<NhomXuong>(), "IdNhomXuong", "TenNhomXuong");
            }
        }
    }
}