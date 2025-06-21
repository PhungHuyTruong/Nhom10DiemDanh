using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using API.Enums;
using ClosedXML.Excel;
using OfficeOpenXml;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KHNXCaHocsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public KHNXCaHocsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/KHNXCaHocs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KHNXCaHoc>>> GetKHNXCaHocs()
        {
            var kHNXCaHocs = await _context.KHNXCaHocs
                .Include(k => k.KeHoachNhomXuong)
                .Include(c => c.CaHoc)
                .ToListAsync();
            if (kHNXCaHocs == null || !kHNXCaHocs.Any())
            {
                return NotFound(new { success = false, message = "Không có ca học nào." });
            }
            return Ok(new { success = true, message = "Lấy danh sách ca học thành công", data = kHNXCaHocs });
        }

        // GET: api/KHNXCaHocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KHNXCaHoc>> GetKHNXCaHoc(Guid id)
        {
            var kHNXCaHoc = await _context.KHNXCaHocs
                .Include(k => k.KeHoachNhomXuong)
                .Include(c => c.CaHoc)
                .FirstOrDefaultAsync(k => k.IdNXCH == id);
            if (kHNXCaHoc == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });
            }
            return Ok(new { success = true, message = "Lấy ca học thành công", data = kHNXCaHoc });
        }

        // PUT: api/KHNXCaHocs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKHNXCaHoc(Guid id, [FromBody] KHNXCaHocUpdateDto dto)
        {
            var entity = await _context.KHNXCaHocs.FirstOrDefaultAsync(x => x.IdNXCH == id);
            if (entity == null)
                return NotFound(new { success = false, message = "Không tìm thấy ca học" });

            entity.Buoi = dto.Buoi;
            entity.NgayHoc = dto.NgayHoc;
            entity.IdCaHoc = dto.IdCaHoc;
            entity.NoiDung = dto.NoiDung;
            entity.LinkOnline = dto.LinkOnline;
            entity.DiemDanhTre = dto.DiemDanhTre;
           // entity.TrangThai = dto.TrangThai;
            entity.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật thành công" });
        }


        private bool KHNXCaHocExists(Guid id)
        {
            throw new NotImplementedException();
        }

        // POST: api/KHNXCaHocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
        [HttpPost]
        public async Task<ActionResult<KHNXCaHoc>> PostKHNXCaHoc(KHNXCaHoc kHNXCaHoc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            kHNXCaHoc.IdNXCH = Guid.NewGuid();
            kHNXCaHoc.NgayTao = DateTime.Now;
            kHNXCaHoc.NgayCapNhat = DateTime.Now;

            _context.KHNXCaHocs.Add(kHNXCaHoc);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Log lỗi chi tiết ra console của server để debug
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    return StatusCode(500, new { success = false, message = $"Lỗi cơ sở dữ liệu: {ex.InnerException.Message}" });
                }
                return StatusCode(500, new { success = false, message = $"Lỗi cơ sở dữ liệu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = $"Đã xảy ra lỗi không xác định: {ex.Message}" });
            }

            return CreatedAtAction(nameof(GetKHNXCaHoc), new { id = kHNXCaHoc.IdNXCH }, new
            {
                success = true,
                message = "Thêm ca học thành công",
                data = kHNXCaHoc
            });
        }
        // ... existing code ...

        // DELETE: api/KHNXCaHocs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKHNXCaHoc(Guid id)
        {
            var kHNXCaHoc = await _context.KHNXCaHocs.FindAsync(id);
            if (kHNXCaHoc == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });
            }
            _context.KHNXCaHocs.Remove(kHNXCaHoc);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Xóa ca học thành công" });
        }

        [HttpGet("GetCaHocsForDropdown")]
        public async Task<ActionResult<IEnumerable<object>>> GetCaHocsForDropdown()
        {
            var caHocs = await _context.CaHocs
                .Select(c => new
                {
                    c.IdCaHoc,
                    c.TenCaHoc,
                    c.ThoiGianBatDau,
                    c.ThoiGianKetThuc
                })
                .Where(c => c.TenCaHoc != null && c.ThoiGianBatDau != null && c.ThoiGianKetThuc != null) // Ensure essential fields are not null
                .ToListAsync();

            if (caHocs == null || !caHocs.Any())
            {
                return NotFound(new { success = false, message = "Không có ca học nào để hiển thị." });
            }

            return Ok(new { success = true, message = "Lấy danh sách ca học thành công", data = caHocs });
        }


        ///enum trạng thái
        private TrangThaiCaHoc TinhTrangThai(KHNXCaHoc caHoc)
        {
            if (caHoc.CaHoc?.ThoiGianBatDau == null || caHoc.CaHoc?.ThoiGianKetThuc == null)
                return TrangThaiCaHoc.SapDienRa;

            var now = DateTime.Now;
            var start = caHoc.NgayHoc.Date + caHoc.CaHoc.ThoiGianBatDau.Value;
            var end = caHoc.NgayHoc.Date + caHoc.CaHoc.ThoiGianKetThuc.Value;

            if (now < start)
                return TrangThaiCaHoc.SapDienRa;
            else if (now >= start && now <= end)
                return TrangThaiCaHoc.DangDienRa;
            else
                return TrangThaiCaHoc.DaDienRa;
        }

        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate([FromQuery] Guid? idKHNX)
        {
            var query = _context.KHNXCaHocs.Include(x => x.CaHoc).AsQueryable();
            if (idKHNX.HasValue)
                query = query.Where(x => x.IdKHNX == idKHNX.Value);

            var data = await query.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DanhSachCaHoc");

            // Header
            worksheet.Cell(1, 1).Value = "Buoi";
            worksheet.Cell(1, 2).Value = "NgayHoc";
            worksheet.Cell(1, 3).Value = "ThoiGian";
            worksheet.Cell(1, 4).Value = "CaHoc";
            worksheet.Cell(1, 5).Value = "NoiDung";
            worksheet.Cell(1, 6).Value = "LinkOnline";
            worksheet.Cell(1, 7).Value = "DiemDanhTre";
            worksheet.Cell(1, 8).Value = "TrangThai";

            int row = 2;
            foreach (var item in data)
            {
                var start = item.CaHoc?.ThoiGianBatDau;
                var end = item.CaHoc?.ThoiGianKetThuc;

                var thoiGian = (start.HasValue && end.HasValue)
                    ? $"{start.Value:hh\\:mm} - {end.Value:hh\\:mm}"
                    : "";

                var trangThai = "Không xác định";
                if (start.HasValue && end.HasValue)
                {
                    var now = DateTime.Now;
                    var batDau = item.NgayHoc.Date + start.Value;
                    var ketThuc = item.NgayHoc.Date + end.Value;

                    if (now < batDau)
                        trangThai = "Sắp diễn ra";
                    else if (now >= batDau && now <= ketThuc)
                        trangThai = "Đang diễn ra";
                    else
                        trangThai = "Đã diễn ra";
                }

                worksheet.Cell(row, 1).Value = item.Buoi;
                worksheet.Cell(row, 2).Value = item.NgayHoc.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 3).Value = thoiGian;
                worksheet.Cell(row, 4).Value = item.CaHoc?.TenCaHoc ?? "";
                worksheet.Cell(row, 5).Value = item.NoiDung;
                worksheet.Cell(row, 6).Value = item.LinkOnline;
                worksheet.Cell(row, 7).Value = item.DiemDanhTre;
                worksheet.Cell(row, 8).Value = trangThai;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DanhSachCaHoc.xlsx");
        }






        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file, [FromForm] Guid idKHNX)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            var importHistory = new ImportHistory
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                ImportDate = DateTime.Now,
                ImportedBy = "Tên người dùng hoặc null", // Nếu có thông tin user thì lấy, không thì để null
                Type = "KHNXCaHoc"
            };

            int successCount = 0;
            int failCount = 0;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                {
                    try
                    {
                        var buoi = worksheet.Cells[row, 1].Text;
                        var ngayHoc = DateTime.TryParse(worksheet.Cells[row, 2].Text, out var date) ? date : DateTime.Now;
                        var thoiGian = worksheet.Cells[row, 3].Text;
                        var tenCaHocExcel = worksheet.Cells[row, 4].Text;
                        var noiDung = worksheet.Cells[row, 5].Text;
                        var linkOnline = worksheet.Cells[row, 6].Text;
                        var diemDanhTre = worksheet.Cells[row, 7].Text;

                        var caHocInDb = await _context.CaHocs.FirstOrDefaultAsync(c => c.TenCaHoc == tenCaHocExcel);
                        Guid? idCaHoc = caHocInDb?.IdCaHoc;

                        var caHoc = new KHNXCaHoc
                        {
                            IdNXCH = Guid.NewGuid(),
                            Buoi = buoi,
                            NgayHoc = ngayHoc,
                            ThoiGian = thoiGian,
                            NoiDung = noiDung,
                            LinkOnline = linkOnline,
                            IdCaHoc = idCaHoc,
                            DiemDanhTre = diemDanhTre,
                            IdKHNX = idKHNX, // Gán đúng IdKHNX cho từng ca học
                            NgayTao = DateTime.Now,
                            NgayCapNhat = DateTime.Now
                        };

                        _context.KHNXCaHocs.Add(caHoc);
                        successCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }

                await _context.SaveChangesAsync();

                importHistory.Status = failCount == 0 ? "Success" : (successCount > 0 ? "PartialSuccess" : "Failed");
                importHistory.Message = $"Import thành công {successCount} dòng, thất bại {failCount} dòng.";
            }
            catch (Exception ex)
            {
                importHistory.Status = "Failed";
                importHistory.Message = $"Lỗi: {ex.Message}";
            }

            _context.ImportHistory.Add(importHistory);
            await _context.SaveChangesAsync();

            // Redirect về Index và truyền lại idKHNX
            return RedirectToAction("Index", "KHNXCaHocs", new { idKHNX = idKHNX });
        }




    }
}
