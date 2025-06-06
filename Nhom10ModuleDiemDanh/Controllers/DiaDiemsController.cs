using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using API.Data;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class DiaDiemsController : Controller
    {
        private readonly HttpClient _client;

        public DiaDiemsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("MyApi");
            _client.BaseAddress = new Uri("http://localhost:5017/api/");
        }

        // GET: DiaDiems
        public async Task<IActionResult> Indexs(string searchString, string status)
        {
            var response = await _client.GetAsync("DiaDiem");
            if (!response.IsSuccessStatusCode)
                return View(new List<DiaDiem>());

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<DiaDiem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!string.IsNullOrEmpty(searchString))
            {
                data = data.Where(d => d.TenDiaDiem != null && d.TenDiaDiem.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                bool statusBool = status == "true";
                data = data.Where(d => d.TrangThai == statusBool).ToList();
            }

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentStatus = status;

            return View(data);
        }

        // GET: Create
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(DiaDiem model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("DiaDiem", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Lỗi tạo địa điểm: {errorMsg}");
                return View(model); // Giữ lại form để người dùng sửa
            }

            return RedirectToAction("Indexs"); // Thành công → quay về danh sách
        }

        // GET: Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"DiaDiem/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var diaDiem = JsonSerializer.Deserialize<DiaDiem>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(diaDiem);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DiaDiem model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"DiaDiem/{model.IdDiaDiem}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Indexs");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _client.DeleteAsync($"DiaDiem/{id}");
            if (!response.IsSuccessStatusCode)
                return BadRequest("Không thể xóa địa điểm.");

            return RedirectToAction("Indexs");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            // Gọi API GET để lấy thông tin địa điểm
            var getResponse = await _client.GetAsync($"DiaDiem/{id}");
            if (!getResponse.IsSuccessStatusCode) return NotFound();

            var json = await getResponse.Content.ReadAsStringAsync();
            var diaDiem = JsonSerializer.Deserialize<DiaDiem>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (diaDiem == null) return NotFound();

            diaDiem.TrangThai = !diaDiem.TrangThai;
            diaDiem.NgayCapNhat = DateTime.Now;

            // Gửi PUT cập nhật
            var putJson = JsonSerializer.Serialize(diaDiem);
            var content = new StringContent(putJson, Encoding.UTF8, "application/json");
            var putResponse = await _client.PutAsync($"DiaDiem/{id}", content);

            if (!putResponse.IsSuccessStatusCode)
                return BadRequest("Không thể đổi trạng thái.");

            return RedirectToAction("Indexs");
        }

        // GET: Details
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _client.GetAsync($"DiaDiem/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Không tìm thấy địa điểm.");

            var json = await response.Content.ReadAsStringAsync();
            var diaDiem = JsonSerializer.Deserialize<DiaDiem>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (diaDiem == null)
                return NotFound("Không có dữ liệu.");

            return View(diaDiem);
        }
    }
}
