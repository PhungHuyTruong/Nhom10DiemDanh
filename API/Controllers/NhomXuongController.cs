using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using OfficeOpenXml;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhomXuongsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public NhomXuongsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/NhomXuongs/paging
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int pageSize = 5,
            string? search = null,
            int? trangThai = null)
        {
            var query = _context.NhomXuongs
        .Include(x => x.DuAn)
        .Include(x => x.QuanLyBoMon)
        .Include(x => x.PhuTrachXuong)
        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(x => x.TenNhomXuong.ToLower().Contains(search));
            }

            if (trangThai != null)
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            var totalItems = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // ❌ bỏ .Select(...)

            return Ok(new
            {
                data, // Trả luôn object NhomXuong (có navigation)
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    totalItems,
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                }
            });
        }

        // GET: api/NhomXuongs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _context.NhomXuongs
                .Include(x => x.DuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.PhuTrachXuong)
                .FirstOrDefaultAsync(x => x.IdNhomXuong == id);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        // POST: api/NhomXuongs
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] NhomXuong model)
        {
            model.IdNhomXuong = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.TrangThai = 1;
            _context.NhomXuongs.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // PUT: api/NhomXuongs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, NhomXuong model)
        {
            var exist = await _context.NhomXuongs.FindAsync(id);
            if (exist == null) return NotFound();

            exist.TenNhomXuong = model.TenNhomXuong;
            exist.IdDuAn = model.IdDuAn;
            exist.IdBoMon = model.IdBoMon;
            exist.IdPhuTrachXuong = model.IdPhuTrachXuong;
            exist.MoTa = model.MoTa;
            exist.TrangThai = model.TrangThai;
            exist.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(exist);
        }

        // PUT: api/NhomXuongs/toggle-status/{id}
        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var nx = await _context.NhomXuongs.FindAsync(id);
            if (nx == null) return NotFound();

            nx.TrangThai = nx.TrangThai == 1 ? 0 : 1;
            nx.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(nx);
        }


        [HttpGet("download-data")]
        public IActionResult DownloadData()
        {
            var nhomXuongs = _context.NhomXuongs
         .Include(x => x.PhuTrachXuong)
         .Include(x => x.DuAn)
         .Include(x => x.QuanLyBoMon)
         .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DanhSachNhomXuong");

            // Header
            worksheet.Cell(1, 1).Value = "Tên Nhóm Xưởng";
            worksheet.Cell(1, 2).Value = "Mô Tả";
            worksheet.Cell(1, 3).Value = "Giảng Viên Phụ Trách";
            worksheet.Cell(1, 4).Value = "Tên Dự Án";
            worksheet.Cell(1, 5).Value = "Mã Bộ Môn";
            worksheet.Cell(1, 6).Value = "Trạng Thái";
            worksheet.Cell(1, 7).Value = "Ngày Tạo";
            worksheet.Cell(1, 8).Value = "Ngày Cập Nhật";

            int row = 2;
            foreach (var item in nhomXuongs)
            {
                worksheet.Cell(row, 1).Value = item.TenNhomXuong;
                worksheet.Cell(row, 2).Value = item.MoTa;
                worksheet.Cell(row, 3).Value = item.PhuTrachXuong?.TenNhanVien ?? "Không có";
                worksheet.Cell(row, 4).Value = item.DuAn?.TenDuAn ?? "Không có";
                worksheet.Cell(row, 5).Value = item.QuanLyBoMon?.MaBoMon ?? "Không có";
                worksheet.Cell(row, 6).Value = item.TrangThai == 1 ? "Hoạt động" : "Không hoạt động";
                worksheet.Cell(row, 7).Value = ((DateTime)item.NgayTao).ToString("yyyy-MM-dd");
                worksheet.Cell(row, 8).Value = item.NgayCapNhat?.ToString("yyyy-MM-dd") ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DanhSachNhomXuong.xlsx"
            );
        }


        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null) return BadRequest("Sheet trống.");

            var list = new List<NhomXuong>();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var ten = worksheet.Cells[row, 1].Text?.Trim();
                var moTa = worksheet.Cells[row, 2].Text?.Trim();
                var tenGiangVien = worksheet.Cells[row, 3].Text?.Trim();
                var tenDuAn = worksheet.Cells[row, 4].Text?.Trim();

                if (string.IsNullOrEmpty(ten)) continue;

                var giangVien = await _context.PhuTrachXuongs.FirstOrDefaultAsync(x => x.TenNhanVien == tenGiangVien);
                var duAn = await _context.DuAns.FirstOrDefaultAsync(x => x.TenDuAn == tenDuAn);

                if (giangVien == null || duAn == null) continue;

                var nhom = new NhomXuong
                {
                    IdNhomXuong = Guid.NewGuid(),
                    TenNhomXuong = ten,
                    MoTa = moTa,
                    IdPhuTrachXuong = giangVien.IdNhanVien,
                    IdDuAn = duAn.IdDuAn,
                    IdBoMon = duAn.IdBoMon,
                    TrangThai = 1,
                    NgayTao = DateTime.Now
                };

                list.Add(nhom);
            }

            if (list.Count > 0)
            {
                await _context.NhomXuongs.AddRangeAsync(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest("Không có dòng nào được import.");
            }
        }


    }
}