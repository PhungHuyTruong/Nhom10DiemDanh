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

        public PhuTrachXuongsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("MyApi");
            _client.BaseAddress = new Uri("https://localhost:7296/api/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("PhuTrachXuongs/GetAll");
            if (!response.IsSuccessStatusCode)
                return View(new List<DtoApi>());

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<DtoApi>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            await LoadCoSoList();
            await LoadVaiTroList();

            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCoSoList();
            await LoadVaiTroList(); // Thêm dòng này
            return View(new DtoNhom10());
        }

        [HttpPost]
        public async Task<IActionResult> Create(DtoNhom10 model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCoSoList();
                await LoadVaiTroList();
                return View(model); // ✅ Trả lại đúng kiểu DtoNhom10 khi lỗi
            }

            // Tạo entity để gửi lên API
            var entity = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = model.TenNhanVien,
                MaNhanVien = model.MaNhanVien,
                EmailFE = model.EmailFE,
                EmailFPT = model.EmailFPT,
                IdCoSo = model.IdCoSo,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            // Gửi kèm danh sách vai trò
            var payload = new
            {
                entity.IdNhanVien,
                entity.TenNhanVien,
                entity.MaNhanVien,
                entity.EmailFE,
                entity.EmailFPT,
                entity.IdCoSo,
                entity.TrangThai,
                entity.NgayTao,
                IdVaiTros = model.IdVaiTros // ✅ danh sách vai trò từ View
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("PhuTrachXuongs", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Thêm mới thất bại: {response.StatusCode}, {errorBody}");

                await LoadCoSoList();
                await LoadVaiTroList();

                // ❗ Phải trả lại DTO (không trả PhuTrachXuong)
                return View(model);
            }

            return RedirectToAction("Index", "PhuTrachXuongs");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _client.GetAsync($"PhuTrachXuongs/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<DtoNhom10>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            await LoadCoSoList();
            await LoadVaiTroList();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(DtoNhom10 model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCoSoList();
                await LoadVaiTroList();
                return View(model);
            }

            var payload = new
            {
                model.IdNhanVien,
                model.TenNhanVien,
                model.MaNhanVien,
                model.EmailFE,
                model.EmailFPT,
                model.IdCoSo,
                model.TrangThai,
                model.NgayTao,
                model.NgayCapNhat,
                IdVaiTros = model.IdVaiTros
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"PhuTrachXuongs/{model.IdNhanVien}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Cập nhật thất bại: {error}");
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