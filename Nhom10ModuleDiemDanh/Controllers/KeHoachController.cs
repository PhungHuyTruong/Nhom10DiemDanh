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
        private readonly ModuleDiemDanhDbContext _context;

        public KeHoachController(ModuleDiemDanhDbContext context, IKeHoachService keHoachService, HttpClient httpClient)
        {
            _keHoachService = keHoachService;
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<IActionResult> Index(string tuKhoa = "", string trangThai = "", string idBoMon = "", string idCapDoDuAn = "", string idHocKy = "", string namHoc = "")
        {
            // Lấy dữ liệu cho bộ lọc từ API
            var boMonsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}api/QuanLyBoMons");
            var capDoDuAnsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}api/CapDoDuAn");
            var hocKysResponse = await _httpClient.GetAsync($"{_apiBaseUrl}api/HocKy");
            var duAnsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}api/DuAn");

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
            var duAns = _context.DuAns
        .Select(d => new { d.IdDuAn, d.TenDuAn })
        .ToList();

            ViewBag.DuAnList = new SelectList(duAns, "IdDuAn", "TenDuAn");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(KeHoachViewModel model)
        {
            Console.WriteLine("===> IdDuAn nhận được từ form: " + model.IdDuAn);
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không được để trống" });
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
            var response = await _httpClient.GetAsync($"https://localhost:7296/api/KeHoach/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var keHoach = JsonSerializer.Deserialize<KeHoachViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // This is where you're fetching DuAns
            var duAns = _context.DuAns
                .Select(d => new { d.IdDuAn, d.TenDuAn })
                .ToList();

            ViewBag.DuAnList = new SelectList(duAns, "IdDuAn", "TenDuAn", keHoach.IdDuAn);

            return View(keHoach);
        }


        // Trong KeHoachController.cs

        [HttpPost]
        public async Task<IActionResult> Edit(KeHoachViewModel model)
        {
            // Luôn tải lại danh sách dự án cho ViewBag, để đảm bảo DropdownList có dữ liệu khi trả về View
            var duAns = _context.DuAns
                .Select(d => new { d.IdDuAn, d.TenDuAn })
                .ToList();
            ViewBag.DuAnList = new SelectList(duAns, "IdDuAn", "TenDuAn", model.IdDuAn);

            // Kiểm tra các lỗi validation từ Model Binding
            if (!ModelState.IsValid)
            {
                // Trả về JSON với các lỗi validation
                // Thu thập tất cả các lỗi từ ModelState
                var errors = ModelState.Values
                                     .SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                return Json(new { success = false, message = "Dữ liệu nhập không hợp lệ.", errors = errors });
            }

            // Kiểm tra validation logic riêng (Thời gian bắt đầu, kết thúc)
            if (model.ThoiGianBatDau >= model.ThoiGianKetThuc)
            {
                // Thêm lỗi vào ModelState và trả về JSON
                ModelState.AddModelError("", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
                var errors = ModelState.Values
                                     .SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                return Json(new { success = false, message = "Thời gian không hợp lệ.", errors = errors });
            }

            try
            {
                await _keHoachService.UpdateKeHoach(model);
                return Json(new { success = true }); // Thành công, trả về JSON thành công
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ service và trả về JSON
                return Json(new { success = false, message = "Lỗi khi cập nhật kế hoạch: " + ex.Message });
            }
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

        //[HttpPost]
        //public async Task<IActionResult> ToggleStatus(Guid id)
        //{
        //    await _keHoachService.ToggleStatus(id);
        //    var keHoach = await _keHoachService.GetKeHoachById(id);
        //    return Json(new { success = true, trangThai = keHoach.TrangThai == 1 ? "Hoạt động" : "Ngừng hoạt động" });
        //}
    }
}