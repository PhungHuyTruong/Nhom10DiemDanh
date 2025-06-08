using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using API.Data;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichGiangDayController : Controller
    {
        private readonly HttpClient _client;

        public LichGiangDayController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi");
            client.BaseAddress = new Uri("https://localhost:7296/api/LichGiangDays/");
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<LichGiangDay>());
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return View(new List<LichGiangDay>());
            }

            var data = JsonSerializer.Deserialize<List<LichGiangDay>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(LichGiangDay model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync(id.ToString());
            var json = await response.Content.ReadAsStringAsync();
            var lichGiangDay = JsonSerializer.Deserialize<LichGiangDay>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(lichGiangDay);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, LichGiangDay model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(id.ToString(), content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _client.GetAsync(id.ToString());
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<LichGiangDay>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _client.DeleteAsync(id.ToString());
            return RedirectToAction("Index");
        }

    }
}
