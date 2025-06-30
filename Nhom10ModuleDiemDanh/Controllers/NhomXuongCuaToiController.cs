using API.Data; // dùng lại model từ API
    using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class NhomXuongCuaToiController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7296/api/NhomXuongCuaToi";

        public NhomXuongCuaToiController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index(string search, string status)
        {
            try
            {
                var email = User.Identity.IsAuthenticated
                    ? User.FindFirst(ClaimTypes.Email)?.Value
                    : null;

                var requestUrl = string.IsNullOrEmpty(email)
                    ? _apiUrl
                    : $"{_apiUrl}?email={email}";

                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể lấy dữ liệu nhóm xưởng.";
                    return View(new List<NhomXuong>());
                }

                var nhomXuongs = await response.Content.ReadFromJsonAsync<List<NhomXuong>>();

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    nhomXuongs = nhomXuongs
                        .Where(x => x.TenNhomXuong != null && x.TenNhomXuong.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Trạng thái
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "active")
                        nhomXuongs = nhomXuongs.Where(x => x.TrangThai == 1).ToList();
                    else if (status == "inactive")
                        nhomXuongs = nhomXuongs.Where(x => x.TrangThai == 0).ToList();
                }

                ViewBag.Search = search;
                ViewBag.Status = status;

                return View(nhomXuongs);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View(new List<NhomXuong>());
            }
        }

        public async Task<IActionResult> DanhSachSinhVien(Guid idNhomXuong, string search, string status)
        {
            var sinhVienApiUrl = $"https://localhost:7296/api/SinhVienNhomXuongApi/{idNhomXuong}";
            var response = await _httpClient.GetAsync(sinhVienApiUrl);
            if (!response.IsSuccessStatusCode) return View(new List<SinhVien>());

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<SinhVien>>(json);

            // 🔍 Lọc theo tên
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.TenSinhVien.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 📌 Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active") list = list.Where(s => s.TrangThai).ToList();
                else if (status == "inactive") list = list.Where(s => !s.TrangThai).ToList();
            }

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.IdNhomXuong = idNhomXuong;
            return View(list);
        }


        [HttpPost]
        public async Task<IActionResult> DoiTrangThaiSinhVien(Guid idSinhVien, Guid idNhomXuong)
        {
            // Gọi API đúng với route: POST api/SinhVien/doi-trang-thai/{id}
            var response = await _httpClient.PostAsync(
                $"https://localhost:7296/api/SinhVien/doi-trang-thai/{idSinhVien}",
                null // Không truyền body vì API không cần
            );

            // Có thể kiểm tra response nếu muốn
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Lỗi khi đổi trạng thái sinh viên.";
            }

            return RedirectToAction("DanhSachSinhVien", new { idNhomXuong });
        }


    }
}
