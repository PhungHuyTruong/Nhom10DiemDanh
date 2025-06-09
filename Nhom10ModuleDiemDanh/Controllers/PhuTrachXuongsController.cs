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

            return View(data);
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
            await LoadVaiTroList(); // Thêm dòng này

            return View(model);
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