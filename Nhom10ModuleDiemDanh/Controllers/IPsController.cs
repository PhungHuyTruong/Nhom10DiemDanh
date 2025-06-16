using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class IPsController : Controller
    {
        private readonly HttpClient _httpClient;

        public IPsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(Guid coSoId, bool? trangThai, string? tuKhoa)
        {
            ViewBag.CoSoId = coSoId;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuKhoa = tuKhoa;

            var response = await _httpClient.GetAsync($"https://localhost:7296/api/IPs/ByCoSo/{coSoId}");
            if (!response.IsSuccessStatusCode)
                return View(new List<IP>());

            var content = await response.Content.ReadAsStringAsync();
            var ips = JsonSerializer.Deserialize<List<IP>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Bộ lọc phía client
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                ips = ips.Where(ip => ip.KieuIP.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            if (trangThai.HasValue)
            {
                ips = ips.Where(ip => ip.TrangThai == trangThai.Value).ToList();
            }

            return View(ips);
        }


        [HttpGet]
        public IActionResult Create(Guid? coSoId)
        {
            if (coSoId == null || coSoId == Guid.Empty)
                return BadRequest("Thiếu thông tin cơ sở.");

            var model = new IP
            {
                IdCoSo = coSoId.Value,
                TrangThai = true // Gợi ý: gán mặc định đang hoạt động
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Create(IP model)
        {
            if (!ModelState.IsValid)
            {
                // Gán lại ViewBag.CoSoId nếu cần
                ViewBag.CoSoId = model.IdCoSo;

                return View(model); // KHÔNG Redirect ở đây
            }

            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7296/api/IPs", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { coSoId = model.IdCoSo });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Thêm thất bại: " + errorContent);

            return View(model); // Không redirect để giữ lại thông tin
        }


        public async Task<IActionResult> Edit(Guid id, Guid coSoId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7296/api/IPs/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var ip = JsonSerializer.Deserialize<IP>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.CoSoId = coSoId;
            return PartialView("Edit", ip);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IP model)
        {
            model.NgayCapNhat = DateTime.Now;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"https://localhost:7296/api/IPs/{model.IdIP}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { coSoId = model.IdCoSo });

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Cập nhật thất bại: " + error);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id, Guid coSoId)
        {
            var getResponse = await _httpClient.GetAsync($"https://localhost:7296/api/IPs/{id}");
            if (!getResponse.IsSuccessStatusCode)
                return NotFound();

            var json = await getResponse.Content.ReadAsStringAsync();
            var ip = JsonSerializer.Deserialize<IP>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (ip == null) return NotFound();

            ip.TrangThai = !ip.TrangThai;
            ip.NgayCapNhat = DateTime.Now;

            var putJson = JsonSerializer.Serialize(ip);
            var content = new StringContent(putJson, Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync($"https://localhost:7296/api/IPs/{id}", content);
            if (!putResponse.IsSuccessStatusCode)
                ModelState.AddModelError(string.Empty, "Không thể cập nhật trạng thái.");

            return RedirectToAction("Index", new { coSoId });
        }
    }
}
