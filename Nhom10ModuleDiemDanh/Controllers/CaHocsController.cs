// ... (các using như cũ)

using API.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class CaHocsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const int PageSize = 10;

        public CaHocsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Hiển thị danh sách ca học theo cơ sở
        public async Task<IActionResult> Index(Guid coSoId, string searchTen, int? trangThai, int page = 1)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7296/api/CaHocs/ByCoSo/{coSoId}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.CoSoId = coSoId;
                return View(new List<CaHoc>());
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                ViewBag.CoSoId = coSoId;
                return View(new List<CaHoc>());
            }

            var data = System.Text.Json.JsonSerializer.Deserialize<List<CaHoc>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Lọc theo tên ca học
            if (!string.IsNullOrEmpty(searchTen))
            {
                data = data.Where(c => c.TenCaHoc != null && c.TenCaHoc.Contains(searchTen, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Lọc theo trạng thái (0 hoặc 1)
            if (trangThai.HasValue)
            {
                data = data.Where(c => c.TrangThai == trangThai.Value).ToList();
            }

            int totalItems = data.Count;

            var caHocs = data
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Truyền dữ liệu lọc và phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.SearchTen = searchTen;
            ViewBag.TrangThai = trangThai;
            ViewBag.CoSoId = coSoId;

            return View(caHocs);
        }

        // GET: Create
        public IActionResult Create(Guid coSoId)
        {
            var model = new CaHoc { CoSoId = coSoId };
            return View(model);
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(CaHoc model)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7296/api/CaHocs", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { coSoId = model.CoSoId });

            // Đọc nội dung lỗi để debug
            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Thêm thất bại: " + errorContent);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id, Guid coSoId)
        {
            // 1. Lấy CaHoc hiện tại
            var getResponse = await _httpClient.GetAsync($"https://localhost:7296/api/CaHocs/{id}");
            if (!getResponse.IsSuccessStatusCode)
                return NotFound();

            var json = await getResponse.Content.ReadAsStringAsync();
            var caHoc = System.Text.Json.JsonSerializer.Deserialize<CaHoc>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (caHoc == null)
                return NotFound();

            // 2. Đảo trạng thái
            caHoc.TrangThai = caHoc.TrangThai == 1 ? 0 : 1;
            caHoc.NgayCapNhat = DateTime.Now;

            // 3. Gửi PUT để cập nhật lại
            var putJson = System.Text.Json.JsonSerializer.Serialize(caHoc);
            var content = new StringContent(putJson, Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync($"https://localhost:7296/api/CaHocs/{id}", content);
            if (!putResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật trạng thái.");
            }

            return RedirectToAction("Index", new { coSoId = coSoId });
        }

        // GET: CaHocs/Edit/{id}?coSoId=...
        public async Task<IActionResult> Edit(Guid id, Guid coSoId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7296/api/CaHocs/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var caHoc = System.Text.Json.JsonSerializer.Deserialize<CaHoc>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (caHoc == null)
                return NotFound();

            ViewBag.CoSoId = coSoId;
            return PartialView("Edit", caHoc);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CaHoc model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.NgayCapNhat = DateTime.Now;

            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"https://localhost:7296/api/CaHocs/{model.IdCaHoc}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { coSoId = model.CoSoId });

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Cập nhật thất bại: " + error);
            return View(model);
        }

    }
}