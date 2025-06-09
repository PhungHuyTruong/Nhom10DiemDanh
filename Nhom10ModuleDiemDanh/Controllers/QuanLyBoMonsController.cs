using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class QuanLyBoMonsController : Controller
    {
        private readonly string apiUrl = "https://localhost:7296/api/QuanLyBoMons";
        public QuanLyBoMonsController()
        {

        }
        // GET: QuanLyBoMonsController
        public async Task<IActionResult> Index(int page = 1, string search = "", string status = "")
        {
            int pageSize = 5;
            var pagedData = new
            {
                data = new List<QuanLyBoMon>(), // Sử dụng object vì không có model cụ thể
                pagination = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalItems = 0,
                    totalPages = 0
                }
            };
            using (HttpClient client = new HttpClient())
            {
                // Encode các tham số để tránh lỗi khi có ký tự đặc biệt
                string encodedSearch = Uri.EscapeDataString(search ?? "");
                string encodedStatus = Uri.EscapeDataString(status ?? "");
                var url = $"{apiUrl}/paging?page={page}&pageSize={pageSize}&search={encodedSearch}&status={encodedStatus}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    pagedData = JsonConvert.DeserializeAnonymousType(json, pagedData);
                }
            }
            ViewBag.Pagination = pagedData.pagination;
            ViewBag.Search = search;
            ViewBag.Status = status;
            return View(pagedData.data);
        }

        // GET: QuanLyBoMonsController/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            QuanLyBoMon boMon = null;
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    boMon = JsonConvert.DeserializeObject<QuanLyBoMon>(json);
                }
            }
            if (boMon == null)
            {
                return NotFound();
            }
            return View(boMon);
        }

        // GET: QuanLyBoMonsController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuanLyBoMonsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDBoMon,MaBoMon,TenBoMon,CoSoHoatDong,NgayTao,NgayCapNhat,TrangThai")] QuanLyBoMon quanLyBoMon)
        {
            if (!ModelState.IsValid)
            {
                return View(quanLyBoMon);
            }
            using (HttpClient client = new HttpClient())
            {
                if (string.IsNullOrEmpty(quanLyBoMon.CoSoHoatDong))
                {
                    quanLyBoMon.CoSoHoatDong = "Chưa có";
                }
                StringContent content = new StringContent(JsonConvert.SerializeObject(quanLyBoMon), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(apiUrl, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                }
            }

            return View(quanLyBoMon);
        }

        // GET: QuanLyBoMonsController/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            QuanLyBoMon boMon = new QuanLyBoMon();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    boMon = JsonConvert.DeserializeObject<QuanLyBoMon>(json);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(boMon);
        }

        // POST: QuanLyBoMonsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("IDBoMon,MaBoMon,TenBoMon,CoSoHoatDong,NgayTao,NgayCapNhat")] QuanLyBoMon quanLyBoMon)
        {
            if (id != quanLyBoMon.IDBoMon)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(quanLyBoMon);
            }


            using (HttpClient client = new HttpClient())
            {
                // Bước 1: Lấy dữ liệu gốc từ API để giữ nguyên trạng thái
                var responseGet = client.GetAsync($"{apiUrl}/{id}").Result;
                if (!responseGet.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Không tìm thấy bộ môn để cập nhật.");
                    return View(quanLyBoMon);
                }

                var currentDataJson = responseGet.Content.ReadAsStringAsync().Result;
                var currentData = JsonConvert.DeserializeObject<QuanLyBoMon>(currentDataJson);

                // Bước 2: Cập nhật thủ công các trường cần thay đổi
                currentData.MaBoMon = quanLyBoMon.MaBoMon;
                currentData.TenBoMon = quanLyBoMon.TenBoMon;
                currentData.CoSoHoatDong = quanLyBoMon.CoSoHoatDong;
                currentData.NgayCapNhat = DateTime.Now;
                // Giữ nguyên currentData.TrangThai

                // Bước 3: Gửi dữ liệu cập nhật
                StringContent content = new StringContent(JsonConvert.SerializeObject(currentData), System.Text.Encoding.UTF8, "application/json");
                var responsePut = client.PutAsync($"{apiUrl}/{id}", content).Result;

                if (responsePut.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật bộ môn. Vui lòng thử lại.");
                }
            }
            return View(quanLyBoMon);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            QuanLyBoMon qlbm = null;
            using (var httpClient = new HttpClient())
            {
                var res = await httpClient.GetAsync($"{apiUrl}/{id}");
                if (res.IsSuccessStatusCode)
                {
                    string json = await res.Content.ReadAsStringAsync();
                    qlbm = JsonConvert.DeserializeObject<QuanLyBoMon>(json);

                }
                else
                {
                    return NotFound();
                }
            }
            qlbm.TrangThai = !qlbm.TrangThai;
            qlbm.NgayCapNhat = DateTime.Now;

            using (var httpClient = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(qlbm);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{apiUrl}/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật trạng thái bộ môn. Vui lòng thử lại.");
                    return View(qlbm);
                }
            }
        }

    }
}