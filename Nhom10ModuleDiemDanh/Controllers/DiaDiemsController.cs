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
            _client = factory.CreateClient("ApiClient");
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
        [HttpGet]
        public IActionResult Create(Guid idCoSo)
        {
            // Kiểm tra xem IdCoSo có được truyền vào không
            if (idCoSo == Guid.Empty)
            {
                // Nếu không có IdCoSo, không thể tạo địa điểm. Chuyển hướng về trang nào đó hoặc báo lỗi.
                return BadRequest("Không có thông tin cơ sở để tạo địa điểm.");
            }

            // Tạo một model mới và gán IdCoSo vào đó
            var viewModel = new DiaDiem
            {
                IdCoSo = idCoSo
            };

            // Trả về View với model đã chứa sẵn IdCoSo (để điền vào trường ẩn)
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiaDiem model)
        {
            // BƯỚC 1: KIỂM TRA DỮ LIỆU TỪ FORM CÓ HỢP LỆ KHÔNG
            // Kiểm tra các [Required], [Range]... trong class DiaDiem
            if (!ModelState.IsValid)
            {
                // Nếu không hợp lệ, trả về ngay lập tức form cho người dùng sửa lại
                // các lỗi sẽ được hiển thị nhờ <div asp-validation-summary="All"></div>
                return View(model);
            }

            // BƯỚC 2: GỌI API VÀ XỬ LÝ LỖI
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("DiaDiem", content);

                // Xử lý kết quả trả về từ API
                if (response.IsSuccessStatusCode)
                {
                    // Thành công! Chuyển hướng người dùng về trang chi tiết của cơ sở họ vừa thêm địa điểm.
                    // Đây là trải nghiệm người dùng tốt hơn là về trang danh sách chung.
                    return RedirectToAction("Details", "CoSo", new { id = model.IdCoSo });
                }
                else
                {
                    // Nếu API trả về lỗi (ví dụ: tên trùng, IdCoSo không tồn tại...)
                    // Đọc nội dung lỗi và hiển thị trên form
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Lỗi từ API: {errorMsg}");
                    return View(model); // Giữ lại form để người dùng sửa
                }
            }
            catch (Exception ex)
            {
                // Bắt các lỗi về kết nối mạng hoặc API không chạy...
                ModelState.AddModelError(string.Empty, $"Không thể kết nối tới máy chủ. Vui lòng thử lại sau. Lỗi: {ex.Message}");
                return View(model);
            }
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
        // Trong DiaDiemsController.cs (Client)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            // Chỉ cần gọi POST đến endpoint chuyên dụng là đủ
            var response = await _client.PostAsync($"DiaDiem/doi-trang-thai/{id}", null);

            if (!response.IsSuccessStatusCode)
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
        // GET: DiaDiems/ListByCoSo/{idCoSo}
        public async Task<IActionResult> ListByCoSo(Guid idCoSo)
        {
            var response = await _client.GetAsync($"DiaDiem/by-coso/{idCoSo}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể tải danh sách địa điểm.";
                return View(new List<DiaDiem>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var diaDiems = JsonSerializer.Deserialize<List<DiaDiem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.IdCoSo = idCoSo;
            return View("Indexs", diaDiems); // Tái sử dụng view Indexs để hiển thị
        }
    }
}
