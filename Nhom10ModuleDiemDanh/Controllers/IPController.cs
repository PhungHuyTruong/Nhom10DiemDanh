using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using API.Data;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class IPController : Controller
    {
        private readonly HttpClient _client;

        public IPController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi");
            client.BaseAddress = new Uri("https://localhost:7296/api/");
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("IPs");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<IP>());
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return View(new List<IP>());
            }

            var data = JsonSerializer.Deserialize<List<IP>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(IP model)
        {
            // Kiểm tra trước khi gửi
            if (string.IsNullOrWhiteSpace(model.KieuIP) || string.IsNullOrWhiteSpace(model.IP_DaiIP))
            {
                ModelState.AddModelError(string.Empty, "KieuIP and IP_DaiIP are required.");
                return View(model);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("IPs", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Lấy lỗi từ response để debug
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"API error: {errorMsg}");
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"IPs/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var ip = JsonSerializer.Deserialize<IP>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(ip);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, IP model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"IPs/{id}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _client.GetAsync($"IPs/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var ip = JsonSerializer.Deserialize<IP>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(ip);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _client.DeleteAsync($"IPs/{id}");
            return RedirectToAction("Index");
        }

    }
}
