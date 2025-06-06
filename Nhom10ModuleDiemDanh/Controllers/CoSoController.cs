using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class CoSoController : Controller
    {
        private readonly HttpClient _client;

        public CoSoController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("MyApi");
            _client.BaseAddress = new Uri("http://localhost:5017/api/");
        }

        // GET: CoSo?tenCoSo=...&trangThai=...
        public async Task<IActionResult> Index(string tenCoSo, string trangThai)
        {
            var query = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(tenCoSo))
                query["tenCoSo"] = tenCoSo;
            if (!string.IsNullOrEmpty(trangThai))
                query["trangThai"] = trangThai;

            var url = "CoSo";
            if (query.Count > 0)
                url += "?" + string.Join("&", query.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

            var coSoList = await _client.GetFromJsonAsync<List<CoSoViewModel>>(url);

            ViewData["tenCoSo"] = tenCoSo;
            ViewData["trangThai"] = trangThai ?? "Tất cả trạng thái";

            return View(coSoList);
        }

        // GET: CoSo/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var coSo = await _client.GetFromJsonAsync<CoSoViewModel>($"CoSo/{id}");
            if (coSo == null) return NotFound();
            return PartialView("_DetailsPartial", coSo);
        }

        // GET: CoSo/Create
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new CoSoViewModel());
        }

        // POST: CoSo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoSoViewModel coSoViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
            }

            try
            {
                coSoViewModel.IdCoSo = Guid.NewGuid();
                coSoViewModel.DiaChi ??= string.Empty;
                coSoViewModel.SDT ??= string.Empty;
                coSoViewModel.Email ??= string.Empty;

                var response = await _client.PostAsJsonAsync("CoSo/Create", coSoViewModel);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    if (result != null && result.Success)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false, message = result?.Message ?? "Lỗi server khi tạo cơ sở" });
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi HTTP: {response.StatusCode}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể thêm cơ sở: " + ex.Message });
            }
        }

        // GET: CoSo/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var coSo = await _client.GetFromJsonAsync<CoSoViewModel>($"CoSo/{id}");
            if (coSo == null) return NotFound();
            return PartialView("_EditPartial", coSo);
        }

        // POST: CoSo/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CoSoViewModel coSoViewModel)
        {
            if (id != coSoViewModel.IdCoSo)
            {
                return Json(new { success = false, message = "ID không khớp." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
            }

            try
            {
                coSoViewModel.DiaChi ??= string.Empty;
                coSoViewModel.SDT ??= string.Empty;
                coSoViewModel.Email ??= string.Empty;

                var response = await _client.PostAsJsonAsync($"CoSo/Edit/{id}", coSoViewModel);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    if (result != null && result.Success)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false, message = result?.Message ?? "Lỗi server khi sửa cơ sở" });
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi HTTP: {response.StatusCode}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể sửa cơ sở: " + ex.Message });
            }
        }

        // GET: CoSo/Delete/{id}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var coSo = await _client.GetFromJsonAsync<CoSoViewModel>($"CoSo/{id}");
            if (coSo == null) return NotFound();

            return View(coSo);
        }

        // POST: CoSo/DeleteConfirmed/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _client.PostAsync($"CoSo/DeleteConfirmed/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Xóa cơ sở thất bại");
        }

        // POST: CoSo/ToggleStatus
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var response = await _client.PostAsJsonAsync("CoSo/ToggleStatus", new { id });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                if (result != null && result.Success)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = result?.Message ?? "Lỗi server khi thay đổi trạng thái" });
            }
            return Json(new { success = false, message = "Lỗi HTTP khi thay đổi trạng thái" });
        }

        // Các action CaHoc, Ip, DiaDiem vẫn giữ nguyên gọi _client hoặc cần logic riêng bạn chỉnh thêm
        [HttpGet]
        public async Task<IActionResult> CaHoc(Guid id)
        {
            var coSo = await _client.GetFromJsonAsync<CoSoViewModel>($"CoSo/{id}");
            if (coSo == null) return NotFound();
            return View("_CaHoc", coSo);
        }

        [HttpGet]
        public async Task<IActionResult> Ip(Guid id)
        {
            var coSo = await _client.GetFromJsonAsync<CoSoViewModel>($"CoSo/{id}");
            if (coSo == null) return NotFound();
            return View("_Ip", coSo);
        }

        [HttpGet]
        public async Task<IActionResult> DiaDiem(Guid id)
        {
            var coSo = await _client.GetFromJsonAsync<CoSoViewModel>($"CoSo/{id}");
            if (coSo == null) return NotFound();
            return View("_DiaDiem", coSo);
        }
    }

    // Định nghĩa class hỗ trợ đọc response JSON từ API nếu trả về kiểu success + message
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
