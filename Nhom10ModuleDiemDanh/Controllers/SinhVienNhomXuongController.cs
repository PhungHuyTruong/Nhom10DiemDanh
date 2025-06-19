using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class SinhVienNhomXuongController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "https://localhost:7296/api/SinhVienNhomXuongApi";

        public SinhVienNhomXuongController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("MyApi");
        }
        // GET: SinhVienNhomXuongApiController
        public async Task<IActionResult> DanhSach(Guid idNhomXuong)
        {
            var response = await _httpClient.GetAsync($"{_apiBase}/{idNhomXuong}");
            if (!response.IsSuccessStatusCode) return View(new List<SinhVien>());

            var data = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<SinhVien>>(data);

            ViewBag.IdNhomXuong = idNhomXuong;
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> XoaKhoiNhom(Guid id, Guid idNhomXuong)
        {
            var content = new StringContent(JsonConvert.SerializeObject(id));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await _httpClient.PostAsync($"{_apiBase}/xoa-khoi-nhom", content);
            return RedirectToAction("DanhSach", new { idNhomXuong });
        }

        public async Task<IActionResult> ThemSinhVien(Guid idNhomXuong, int page = 1)
        {
            var response = await _httpClient.GetAsync($"{_apiBase}/chua-co-nhom?page={page}&pageSize=5");
            var json = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject<dynamic>(json);
            var list = JsonConvert.DeserializeObject<List<SinhVien>>(result.data.ToString());

            ViewBag.IdNhomXuong = idNhomXuong;
            ViewBag.CurrentPage = (int)result.currentPage;
            ViewBag.TotalPages = (int)result.totalPages;

            // Gọi API lấy tên nhóm xưởng
            var nhomResponse = await _httpClient.GetAsync($"https://localhost:7296/api/NhomXuongs/{idNhomXuong}");
            if (nhomResponse.IsSuccessStatusCode)
            {
                var nhomJson = await nhomResponse.Content.ReadAsStringAsync();
                var nhom = JsonConvert.DeserializeObject<NhomXuong>(nhomJson);
                ViewBag.TenNhomXuong = nhom?.TenNhomXuong;
            }

            return View("ThemSinhVien", list);
        }


        [HttpPost]
        public async Task<IActionResult> GanSinhVienVaoNhom(Guid idNhomXuong, List<Guid> IdSinhVien)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(idNhomXuong.ToString()), "IdNhomXuong");

            foreach (var id in IdSinhVien)
            {
                formData.Add(new StringContent(id.ToString()), "IdSinhVien");
            }

            var response = await _httpClient.PostAsync($"{_apiBase}/gan-vao-nhom", formData);

            // ✅ Sau khi thêm thành công, chuyển về trang danh sách sinh viên trong nhóm
            return RedirectToAction("DanhSach", new { idNhomXuong });
        }


    }
}
