using API.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SinhVienController : Controller
    {
        private readonly ModuleDiemDanhDbContext _context;
        public SinhVienController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sinhViens = await _context.SinhViens
                .Include(x => x.NhomXuong)
                .Include(x => x.VaiTro)
                .ToListAsync();
            return Ok(sinhViens);
        }

        // GET: api/SinhVien/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var sinhVien = await _context.SinhViens
                .Include(x => x.NhomXuong)
                .Include(x => x.VaiTro)
                .FirstOrDefaultAsync(x => x.IdSinhVien == id);

            if (sinhVien == null)
                return NotFound();

            return Ok(sinhVien);
        }

        // POST: api/SinhVien
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SinhVien sinhVien)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            sinhVien.IdSinhVien = Guid.NewGuid();
            sinhVien.NgayTao = DateTime.Now;
            sinhVien.TrangThai = true;

            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "SinhVien");
        }

        // PUT: api/SinhVien/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SinhVien updatedSinhVien)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
                return NotFound();

            sinhVien.TenSinhVien = updatedSinhVien.TenSinhVien;
            sinhVien.MaSinhVien = updatedSinhVien.MaSinhVien;
            sinhVien.Email = updatedSinhVien.Email;
            sinhVien.IdNhomXuong = updatedSinhVien.IdNhomXuong;
            sinhVien.IdVaiTro = updatedSinhVien.IdVaiTro;
            sinhVien.TrangThai = updatedSinhVien.TrangThai;
            sinhVien.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(sinhVien);
        }

        // POST: api/SinhVien/doi-trang-thai/{id}
        [HttpPost("doi-trang-thai/{id}")]
        public async Task<IActionResult> DoiTrangThai(Guid id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
                return NotFound();

            sinhVien.TrangThai = !sinhVien.TrangThai;
            sinhVien.NgayCapNhat = DateTime.Now;

            _context.SinhViens.Update(sinhVien);
            await _context.SaveChangesAsync();

            return Ok(sinhVien);
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            var sinhViens = _context.SinhViens.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachSinhVien");
                // Header
                worksheet.Cell(1, 1).Value = "Mã Sinh Viên";
                worksheet.Cell(1, 2).Value = "Tên Sinh Viên";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Trạng Thái";
                worksheet.Cell(1, 5).Value = "Ngày Tạo";

                int row = 2;
                foreach (var sv in sinhViens)
                {
                    worksheet.Cell(row, 1).Value = sv.MaSinhVien;
                    worksheet.Cell(row, 2).Value = sv.TenSinhVien;
                    worksheet.Cell(row, 3).Value = sv.Email;
                    worksheet.Cell(row, 4).Value = sv.TrangThai ? "Hoạt động" : "Ngừng";
                    worksheet.Cell(row, 5).Value = ((DateTime)sv.NgayTao).ToString("yyyy-MM-dd");
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachSinhVien.xlsx"
                    );
                }
            }
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
            var worksheet = package.Workbook.Worksheets[0];

            Guid.TryParse(worksheet.Cells[2, 4].Text, out Guid idNhomXuong);
            Guid.TryParse(worksheet.Cells[2, 5].Text, out Guid idVaiTro);

            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                MaSinhVien = worksheet.Cells[2, 1].Text,
                TenSinhVien = worksheet.Cells[2, 2].Text,
                Email = worksheet.Cells[2, 3].Text,
                IdNhomXuong = idNhomXuong,
                IdVaiTro = idVaiTro,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            await _context.SinhViens.AddAsync(sv);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã import sinh viên: {sv.TenSinhVien}";

            return RedirectToAction("Index");
        }

        
    }
}
