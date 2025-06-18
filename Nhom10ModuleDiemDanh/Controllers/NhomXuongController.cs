using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Nhom10ModuleDiemDanh.Models;
using DtoApi = API.Models.PhuTrachXuongDto;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class NhomXuongsController : Controller
    {
        private readonly ModuleDiemDanhDbContext _context;
        private readonly HttpClient _client;
        public NhomXuongsController(ModuleDiemDanhDbContext context , IHttpClientFactory factory)
        {
            _context = context;
            _client = factory.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index(int page = 1, string search = "", int? trangThai = null)
        {
            int pageSize = 5;

            var query = _context.NhomXuongs
                .Include(x => x.DuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.PhuTrachXuong)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.TenNhomXuong.Contains(search));

            if (trangThai.HasValue)
                query = query.Where(x => x.TrangThai == trangThai);

            var totalItems = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Pagination = new
            {
                currentPage = page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;

            await LoadGiangVienDropdown();
            LoadDuAnDropdown();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            await LoadGiangVienDropdown();
            LoadDuAnDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhomXuong nhom)
        {
            if (!ModelState.IsValid)
            {
                await LoadGiangVienDropdown();
                LoadDuAnDropdown();
                return View(nhom);
            }

            nhom.IdNhomXuong = Guid.NewGuid();
            nhom.NgayTao = DateTime.Now;
            nhom.TrangThai = 1;
            _context.NhomXuongs.Add(nhom);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _context.NhomXuongs.FindAsync(id);
            if (model == null)
                return NotFound();

            await LoadGiangVienDropdown(model.IdPhuTrachXuong);
            LoadDuAnDropdown(model.IdDuAn);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, NhomXuong model)
        {
            if (id != model.IdNhomXuong || !ModelState.IsValid)
            {
                await LoadGiangVienDropdown(model.IdPhuTrachXuong);
                LoadDuAnDropdown(model.IdDuAn);
                return View(model);
            }

            var exist = await _context.NhomXuongs.FindAsync(id);
            if (exist == null)
                return NotFound();

            exist.TenNhomXuong = model.TenNhomXuong;
            exist.IdDuAn = model.IdDuAn;
            exist.IdBoMon = model.IdBoMon;
            exist.IdPhuTrachXuong = model.IdPhuTrachXuong;
            exist.MoTa = model.MoTa;
            exist.TrangThai = model.TrangThai;
            exist.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var nx = await _context.NhomXuongs.FindAsync(id);
            if (nx == null) return NotFound();

            nx.TrangThai = nx.TrangThai == 1 ? 0 : 1;
            nx.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadGiangVienDropdown(Guid? selectedId = null)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7296/api/PhuTrachXuongs/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<DtoApi>>(json);
                    ViewBag.GiangViens = new SelectList(list, "IdNhanVien", "TenNhanVien", selectedId);
                }
                else
                {
                    ViewBag.GiangViens = new SelectList(new List<DtoApi>(), "IdNhanVien", "TenNhanVien");
                }
            }
        }

        private void LoadDuAnDropdown(Guid? selectedId = null)
        {
            var duAns = _context.DuAns
                .Where(x => x.TrangThai)
                .Select(x => new { x.IdDuAn, x.TenDuAn })
                .ToList();

            ViewBag.DuAns = new SelectList(duAns, "IdDuAn", "TenDuAn", selectedId);
        }

        [HttpGet]
        public async Task<IActionResult> GetBoMonByDuAn(Guid id)
        {
            var duAn = await _context.DuAns.FirstOrDefaultAsync(x => x.IdDuAn == id);
            if (duAn == null || duAn.IdBoMon == null)
                return NotFound();

            var boMon = await _context.QuanLyBoMons.FirstOrDefaultAsync(x => x.IDBoMon == duAn.IdBoMon);
            if (boMon == null)
                return NotFound();

            var data = new
            {
                idBoMon = boMon.IDBoMon,
                maBoMon = boMon.MaBoMon
            };

            return Json(data);
        }

        public async Task<IActionResult> DownloadTemplate()
        {
            var response = await _client.GetAsync("NhomXuongs/download-data");

            if (!response.IsSuccessStatusCode)
                return NotFound("Không thể tải template từ API.");

            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = "Template_NhomXuong.xlsx";

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ImportError"] = "Vui lòng chọn file Excel.";
                return RedirectToAction("Index");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(memoryStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "file", file.FileName);

            using var client = new HttpClient();
            var response = await client.PostAsync("https://localhost:7296/api/NhomXuongs/import-excel", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(json);
                TempData["ImportSuccess"] = result?.message?.ToString() ?? "Import thành công.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ImportError"] = "Import thất bại: " + error;
            }

            // ✅ QUAY VỀ TRANG INDEX để thấy dữ liệu mới
            return RedirectToAction("Index", new { page = 1 });
        }




    }
}
