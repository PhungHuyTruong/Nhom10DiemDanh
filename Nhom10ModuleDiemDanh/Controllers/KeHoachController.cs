using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom10ModuleDiemDanh.Models;
using Nhom10ModuleDiemDanh.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class KeHoachController : Controller
    {
        private readonly IKeHoachService _keHoachService;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7296/";

        public KeHoachController(IKeHoachService keHoachService, HttpClient httpClient)
        {
            _keHoachService = keHoachService;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(string tuKhoa = "", string trangThai = "", string idBoMon = "", string idCapDoDuAn = "", string idHocKy = "", string namHoc = "")
        {
            // Lấy dữ liệu cho bộ lọc từ API
            var boMonsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/QuanLyBoMon");
            var capDoDuAnsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/CapDoDuAn");
            var hocKysResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/HocKy");
            var duAnsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/DuAn");

            if (boMonsResponse.IsSuccessStatusCode && capDoDuAnsResponse.IsSuccessStatusCode && hocKysResponse.IsSuccessStatusCode && duAnsResponse.IsSuccessStatusCode)
            {
                var boMonsJson = await boMonsResponse.Content.ReadAsStringAsync();
                var capDoDuAnsJson = await capDoDuAnsResponse.Content.ReadAsStringAsync();
                var hocKysJson = await hocKysResponse.Content.ReadAsStringAsync();
                var duAnsJson = await duAnsResponse.Content.ReadAsStringAsync();

                var boMons = JsonSerializer.Deserialize<List<QuanLyBoMon>>(boMonsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    .Where(b => b.TrangThai)
                    .Select(b => new { Value = b.IDBoMon, Text = b.TenBoMon });
                var capDoDuAns = JsonSerializer.Deserialize<List<CapDoDuAn>>(capDoDuAnsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    .Where(c => c.TrangThai)
                    .Select(c => new { Value = c.IdCDDA, Text = c.TenCapDoDuAn });
                var hocKys = JsonSerializer.Deserialize<List<HocKy>>(hocKysJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    .Where(h => h.TrangThai)
                    .Select(h => new { Value = h.IdHocKy, Text = h.TenHocKy });
                var duAns = JsonSerializer.Deserialize<List<DuAn>>(duAnsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    .Where(d => d.TrangThai)
                    .Select(d => new { Value = d.IdDuAn, Text = d.TenDuAn });

                ViewData["BoMons"] = new SelectList(boMons, "Value", "Text", idBoMon);
                ViewData["CapDoDuAns"] = new SelectList(capDoDuAns, "Value", "Text", idCapDoDuAn);
                ViewData["HocKys"] = new SelectList(hocKys, "Value", "Text", idHocKy);
                ViewData["DuAns"] = new SelectList(duAns, "Value", "Text");
            }
            else
            {
                ViewData["BoMons"] = new SelectList(new List<object>(), "Value", "Text");
                ViewData["CapDoDuAns"] = new SelectList(new List<object>(), "Value", "Text");
                ViewData["HocKys"] = new SelectList(new List<object>(), "Value", "Text");
                ViewData["DuAns"] = new SelectList(new List<object>(), "Value", "Text");
            }

            ViewData["tuKhoa"] = tuKhoa;
            ViewData["trangThai"] = trangThai;
            ViewData["idBoMon"] = idBoMon;
            ViewData["idCapDoDuAn"] = idCapDoDuAn;
            ViewData["idHocKy"] = idHocKy;
            ViewData["namHoc"] = namHoc;

            var keHoachs = await _keHoachService.GetAllKeHoachs(tuKhoa, trangThai, idBoMon, idCapDoDuAn, idHocKy, namHoc);
            return View(keHoachs ?? new List<KeHoachViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KeHoachViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không được để trống" });
                }

                if (string.IsNullOrEmpty(model.TenKeHoach))
                {
                    return Json(new { success = false, message = "Tên kế hoạch không được để trống" });
                }

                if (string.IsNullOrEmpty(model.NoiDung))
                {
                    return Json(new { success = false, message = "Nội dung không được để trống" });
                }

                if (model.ThoiGianBatDau >= model.ThoiGianKetThuc)
                {
                    return Json(new { success = false, message = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu" });
                }

                await _keHoachService.CreateKeHoach(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tạo kế hoạch: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDuAnList()
        {
            try
            {
                var duAns = await _keHoachService.GetDuAnList();
                return Json(duAns);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lấy danh sách dự án: " + ex.Message });
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _keHoachService.GetKeHoachById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(KeHoachViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ThoiGianBatDau >= model.ThoiGianKetThuc)
                {
                    ModelState.AddModelError("", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
                    return View(model);
                }
                await _keHoachService.UpdateKeHoach(model);
                return Json(new { success = true });
            }
            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _keHoachService.GetKeHoachById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _keHoachService.DeleteKeHoach(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            await _keHoachService.ToggleStatus(id);
            var keHoach = await _keHoachService.GetKeHoachById(id);
            return Json(new { success = true, trangThai = keHoach.TrangThai == 1 ? "Hoạt động" : "Ngừng hoạt động" });
        }
    }
}