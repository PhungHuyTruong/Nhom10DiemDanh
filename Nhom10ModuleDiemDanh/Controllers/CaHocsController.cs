// ... (các using như cũ)

using API.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AppView.Controllers
{
    public class CaHocsController : Controller
    {
        private readonly string _apiBase = "https://localhost:7296/api/CaHocs";

        public async Task<IActionResult> Index(string tenCaHoc, int? trangThai, int page = 1)
        {
            List<CaHoc> danhSach = new();
            int currentPage = page;
            int totalPages = 1;

            try
            {
                using (var http = new HttpClient())
                {
                    var url = $"{_apiBase}/filter?page={page}&pageSize=5";
                    var query = new List<string>();
                    if (!string.IsNullOrEmpty(tenCaHoc))
                        query.Add($"tenCaHoc={Uri.EscapeDataString(tenCaHoc)}");
                    if (trangThai.HasValue)
                        query.Add($"trangThai={trangThai}");

                    if (query.Count > 0)
                        url += "&" + string.Join("&", query);

                    var response = await http.GetAsync(url);
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject<dynamic>(json);

                    danhSach = JsonConvert.DeserializeObject<List<CaHoc>>(result.data.ToString());
                    currentPage = result.pagination.currentPage;
                    totalPages = result.pagination.totalPages;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
            }

            ViewBag.TenCaHoc = tenCaHoc;
            ViewBag.TrangThai = trangThai;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;

            return View(danhSach);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            CaHoc caHoc = null;
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync($"{_apiBase}/{id}");
                var json = await response.Content.ReadAsStringAsync();
                var wrapper = JsonConvert.DeserializeObject<ApiResponse<CaHoc>>(json);
                caHoc = wrapper.data;
            }

            return View(caHoc);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CaHoc caHoc)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(caHoc), Encoding.UTF8, "application/json");
                    var response = await http.PostAsync(_apiBase, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"API lỗi {response.StatusCode}: {err}";
                        return View(caHoc);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi xử lý: " + ex.Message;
                return View(caHoc);
            }
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            CaHoc caHoc = null;
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync($"{_apiBase}/{id}");
                var json = await response.Content.ReadAsStringAsync();
                var wrapper = JsonConvert.DeserializeObject<ApiResponse<CaHoc>>(json);
                caHoc = wrapper.data;
            }

            return View(caHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CaHoc caHoc)
        {
            using (var http = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(caHoc), Encoding.UTF8, "application/json");
                var response = await http.PutAsync($"{_apiBase}?id={id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"API lỗi {response.StatusCode}: {err}";
                    return View(caHoc);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            using (var http = new HttpClient())
            {
                var response = await http.DeleteAsync($"{_apiBase}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Xoá thất bại: {response.StatusCode} - {error}";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
