using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using API.Data;
using System.Net.Http;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class HocKyController : Controller
    {
        private readonly HttpClient _client;
        private const int PageSize = 10;

        public HocKyController(IHttpClientFactory factory)
        {
            var client = factory.CreateClient("MyApi"); 
            client.BaseAddress = new Uri("https://localhost:7296/api/"); 
            _client = client;
        }

        public async Task<IActionResult> Index(string searchTen, bool? trangThai, int page = 1)
        {
            var response = await _client.GetAsync("HocKy");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<HocKy>());
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return View(new List<HocKy>());
            }

            var data = JsonSerializer.Deserialize<List<HocKy>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Lọc theo tên
            if (!string.IsNullOrEmpty(searchTen))
            {
                data = data.Where(h => h.TenHocKy.Contains(searchTen, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Lọc theo trạng thái
            if (trangThai.HasValue)
            {
                data = data.Where(h => h.TrangThai == trangThai.Value).ToList();
            }

            // Tổng số phần tử sau khi lọc
            int totalItems = data.Count;

            // Phân trang
            var hocKys = data
                .OrderByDescending(h => h.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Truyền thông tin phân trang qua ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.SearchTen = searchTen;
            ViewBag.TrangThai = trangThai;

            return View(hocKys);
        }

        //public async Task<IActionResult> Index(string? searchTenHocKy, bool? trangThai, int pageNumber = 1)
        //{
        //    var query = $"HocKy?search={searchTenHocKy}&trangThai={trangThai}&page={pageNumber}";
        //    var response = await _client.GetAsync(query);

        //    if (!response.IsSuccessStatusCode)
        //        return View(new List<HocKy>());

        //    var json = await response.Content.ReadAsStringAsync();
        //    var result = JsonSerializer.Deserialize<JsonElement>(json);

        //    // 👉 Dùng đúng key viết thường (do System.Text.Json trả về camelCase)
        //    if (!result.TryGetProperty("items", out var itemsProp) ||
        //        !result.TryGetProperty("totalItems", out var totalItemsProp) ||
        //        !result.TryGetProperty("page", out var pageProp) ||
        //        !result.TryGetProperty("pageSize", out var pageSizeProp))
        //    {
        //        // Trường hợp JSON không đúng định dạng mong đợi
        //        return View(new List<HocKy>());
        //    }

        //    var data = JsonSerializer.Deserialize<List<HocKy>>(itemsProp.GetRawText());

        //    ViewBag.TotalItems = totalItemsProp.GetInt32();
        //    ViewBag.PageNumber = pageProp.GetInt32();
        //    ViewBag.PageSize = pageSizeProp.GetInt32();
        //    ViewBag.SearchTenHocKy = searchTenHocKy;
        //    ViewBag.TrangThai = trangThai;

        //    return View(data);
        //}


        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(HocKy model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("HocKy", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"HocKy/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var hocKy = JsonSerializer.Deserialize<HocKy>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(hocKy);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(HocKy model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", model);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"HocKy/{model.IdHocKy}", content);

            if (response.IsSuccessStatusCode)
            {
                // Trả về json báo thành công
                return Json(new { success = true });
            }

            ModelState.AddModelError("", "Cập nhật thất bại.");
            return PartialView("Edit", model);
        }


        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            HocKy hocKy = null;

            // Gọi API lấy bản ghi hiện tại
            var getResponse = await _client.GetAsync($"HocKy/{id}");
            if (!getResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await getResponse.Content.ReadAsStringAsync();
            hocKy = JsonSerializer.Deserialize<HocKy>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (hocKy == null)
            {
                return NotFound();
            }

            // Đảo trạng thái (giả định thuộc tính là 'TrangThai' kiểu bool)
            hocKy.TrangThai = !hocKy.TrangThai;
            hocKy.NgayCapNhat = DateTime.Now; // nếu có thuộc tính này

            // Gửi PUT để cập nhật lại
            var putJson = JsonSerializer.Serialize(hocKy);
            var content = new StringContent(putJson, Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"HocKy/{id}", content);
            if (!putResponse.IsSuccessStatusCode)
            {
                return BadRequest("Không thể cập nhật trạng thái.");
            }

            return RedirectToAction("Index");
        }
    }
}
