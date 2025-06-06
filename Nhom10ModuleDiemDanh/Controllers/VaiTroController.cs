using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using API.Data;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class VaiTroController : Controller
    {
        private readonly HttpClient _client;

        public VaiTroController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi");
            client.BaseAddress = new Uri("https://localhost:7296/api/"); // Cập nhật lại đúng base address nếu khác
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("VaiTro");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var vaiTros = JsonSerializer.Deserialize<List<VaiTro>>(data,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(vaiTros); // 🔍 Kiểm tra biến này có gì không
            }

            return View(new List<VaiTro>());
        }


        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(VaiTro model)
        {
            if (!ModelState.IsValid)
                return View(model); // nên có để hiển thị lỗi nhập liệu

            model.IdVaiTro = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.TrangThai = true;


            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("VaiTro", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model); // trả về form thay vì Index nếu lỗi
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"VaiTro/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var vaiTro = JsonSerializer.Deserialize<VaiTro>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(vaiTro);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, VaiTro model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"VaiTro/{id}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        // GET: VaiTro/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _client.GetAsync($"VaiTro/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var vaiTro = JsonSerializer.Deserialize<VaiTro>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(vaiTro);
        }

        // POST: VaiTro/DeleteConfirmed/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _client.DeleteAsync($"VaiTro/{id}");
            return RedirectToAction("Index");
        }
    }
}
