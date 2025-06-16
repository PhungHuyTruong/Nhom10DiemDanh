using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http.Json;
using System.Net.Http;
using API.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Nhom10ModuleDiemDanh.Models;
using System.Text.Json.Serialization;
using Humanizer;
using DtoNhom10 = Nhom10ModuleDiemDanh.Models.PhuTrachXuongDto;
using DtoApi = API.Models.PhuTrachXuongDto;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class PhuTrachXuongsController : Controller
    {
        private readonly HttpClient _client;
        private const int PageSize = 10;

        public PhuTrachXuongsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("MyApi");
            _client.BaseAddress = new Uri("https://localhost:7296/api/");
        }

        public async Task<IActionResult> Index(string searchMa, bool? trangThai, int page = 1)
        {
            var response = await _client.GetAsync("PhuTrachXuongs/GetAll");

            if (!response.IsSuccessStatusCode)
                return View(new List<DtoApi>());

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return View(new List<DtoApi>());

            var data = JsonSerializer.Deserialize<List<DtoApi>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Lọc theo mã phụ trách (nếu có)
            if (!string.IsNullOrEmpty(searchMa))
                data = data.Where(p => p.MaNhanVien.Contains(searchMa, StringComparison.OrdinalIgnoreCase)).ToList();

            // Lọc theo trạng thái (nếu có)
            if (trangThai.HasValue)
                data = data.Where(p => p.TrangThai == trangThai.Value).ToList();

            // Phân trang
            int totalItems = data.Count;
            var items = data
                .OrderByDescending(p => p.NgayTao) // nếu có NgayTao
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Truyền dữ liệu phân trang về View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.SearchMa = searchMa;
            ViewBag.TrangThai = trangThai;

            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCoSoList();
            await LoadVaiTroList(); // Thêm dòng này
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PhuTrachXuong model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCoSoList();
                await LoadVaiTroList(); // Thêm dòng này
                return View(model);

            }

            model.IdNhanVien = Guid.Empty;
            model.NgayTao = DateTime.Now;
            model.TrangThai = true;
            model.CoSo = null;
            model.DiemDanhs = null;
            model.NhomXuongs = null;

            var jsonString = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("PhuTrachXuongs", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Thêm mới thất bại: {response.StatusCode}, {errorBody}");
                await LoadCoSoList();
                await LoadVaiTroList();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"PhuTrachXuongs/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<PhuTrachXuong>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await LoadCoSoList();
            await LoadVaiTroList();

            return PartialView("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PhuTrachXuong model)
        {

            if (!ModelState.IsValid)
            {
                await LoadCoSoList();
                await LoadVaiTroList();
                return View(model);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"PhuTrachXuongs/{model.IdNhanVien}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Cập nhật thất bại: {response.StatusCode}, {errorBody}");
                await LoadCoSoList();
                await LoadVaiTroList();
                return View(model);
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var response = await _client.PostAsync($"PhuTrachXuongs/doi-trang-thai/{id}", null);
            return RedirectToAction("Index");
        }

        private async Task LoadCoSoList()
        {
            var res = await _client.GetAsync("PhuTrachXuongs/cosos");
            if (!res.IsSuccessStatusCode)
            {
                ViewBag.CoSoList = new SelectList(new List<CoSoDto>(), "IdCoSo", "TenCoSo");
            }
            else
            {
                var json = await res.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<CoSoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.CoSoList = new SelectList(list, "IdCoSo", "TenCoSo");
            }
        }

        private async Task LoadVaiTroList()
        {
            var res = await _client.GetAsync("PhuTrachXuongs/vaitros");
            if (!res.IsSuccessStatusCode)
            {
                ViewBag.VaiTroList = new SelectList(new List<VaiTro>(), "IdVaiTro", "TenVaiTro");
            }
            else
            {
                var json = await res.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<VaiTro>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                ViewBag.VaiTroList = new SelectList(list, "IdVaiTro", "TenVaiTro");
            }
        }

    }

}