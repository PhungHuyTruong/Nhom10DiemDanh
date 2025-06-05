using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using API.Data;
using System.Net.Http;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class CapDoDuAnController : Controller
    {
        private readonly HttpClient _client;
        private const int PageSize = 10;

        public CapDoDuAnController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi");
            client.BaseAddress = new Uri("https://localhost:7296/api/");
            _client = client;
        }

        public async Task<IActionResult> Index(string searchTen, bool? trangThai, int page = 1)
        {
            var response = await _client.GetAsync("CapDoDuAn");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<CapDoDuAn>());
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return View(new List<CapDoDuAn>());
            }

            var data = JsonSerializer.Deserialize<List<CapDoDuAn>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!string.IsNullOrEmpty(searchTen))
            {
                data = data.Where(h => h.TenCapDoDuAn.Contains(searchTen, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (trangThai.HasValue)
            {
                data = data.Where(h => h.TrangThai == trangThai.Value).ToList();
            }

            int totalItems = data.Count;

            var listPaged = data
                .OrderByDescending(h => h.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.SearchTen = searchTen;
            ViewBag.TrangThai = trangThai;

            return View(listPaged);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CapDoDuAn model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("CapDoDuAn", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"CapDoDuAn/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<CapDoDuAn>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CapDoDuAn model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"CapDoDuAn/{model.IdCDDA}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var response = await _client.PostAsync($"CapDoDuAn/doi-trang-thai/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Không thể đổi trạng thái.");
            return RedirectToAction(nameof(Index));
        }

    }
}
