using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;
using API.Data;
using System.Net.Http;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichHocController : Controller
    {
        private readonly HttpClient _client;
        private const int PageSize = 10;

        public LichHocController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi");
            client.BaseAddress = new Uri("https://localhost:7296/api/");
            _client = client;
        }

        public async Task<IActionResult> Index(Guid? idHocKy, int? trangThai, int page = 1)
        {
            var response = await _client.GetAsync("LichHocs");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<LichHoc>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<LichHoc>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (idHocKy.HasValue)
            {
                data = data.Where(h => h.IDHocKy == idHocKy.Value).ToList();
            }

            if (trangThai.HasValue)
            {
                data = data.Where(h => h.TrangThai == trangThai.Value).ToList();
            }

            int totalItems = data.Count;
            var lichHocs = data
                .OrderByDescending(h => h.IDLichHoc)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.IdHocKy = idHocKy;
            ViewBag.TrangThai = trangThai;

            // Truyền danh sách SelectList trạng thái xuống view
            ViewBag.TrangThaiList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Tất cả", Selected = !trangThai.HasValue },
                new SelectListItem { Value = "0", Text = "Chưa học", Selected = trangThai == 0 },
                new SelectListItem { Value = "1", Text = "Đã học", Selected = trangThai == 1 }
            };

            return View(lichHocs);
        }

        public IActionResult Create()
        {
            return View(new LichHoc());
        }

        [HttpPost]
        public async Task<IActionResult> Create(LichHoc model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("LichHocs", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"LichHocs/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var lichHoc = JsonSerializer.Deserialize<LichHoc>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(lichHoc);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LichHoc model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"LichHocs/{model.IDLichHoc}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        // GET: hiển thị confirm delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _client.GetAsync($"LichHocs/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var lichHoc = JsonSerializer.Deserialize<LichHoc>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(lichHoc);
        }

        // POST: thực hiện xóa
        [HttpPost]
        public async Task<IActionResult> Delete(LichHoc model)
        {
            var response = await _client.DeleteAsync($"LichHocs/{model.IDLichHoc}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return BadRequest("Không thể xóa dữ liệu.");
        }

    }
}
