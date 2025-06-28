using Microsoft.AspNetCore.Mvc;
using API.Data;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LichSuDiemDanhController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;
        public LichSuDiemDanhController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/LichSuDiemDanh?IdSinhVien=...&IdHocKy=...&IdNhomXuong=...
        [HttpGet]
        public async Task<IActionResult> GetLichSuDiemDanh(Guid IdSinhVien, Guid? IdHocKy, Guid? IdNhomXuong)
        {
            var query = _context.LichSuDiemDanhs
            .Include(lsdd => lsdd.DiemDanh)
            .Include(lsdd => lsdd.KHNXCaHoc)
                .ThenInclude(khnxch => khnxch.CaHoc)
            .Include(lsdd => lsdd.KHNXCaHoc)
                .ThenInclude(khnxch => khnxch.KeHoachNhomXuong)
                    .ThenInclude(khnx => khnx.NhomXuong)
            .Include(lsdd => lsdd.KHNXCaHoc)
                .ThenInclude(khnxch => khnxch.KeHoachNhomXuong)
                    .ThenInclude(khnx => khnx.KeHoach)
                        .ThenInclude(kh => kh.DuAn)
            .Where(lsdd => lsdd.DiemDanh.IdSinhVien == IdSinhVien);

            if (IdHocKy.HasValue)
            {
                query = query.Where(lsdd => lsdd.KHNXCaHoc.KeHoachNhomXuong.KeHoach.DuAn.IdHocKy == IdHocKy.Value);
            }
            if (IdNhomXuong.HasValue)
            {
                query = query.Where(lsdd => lsdd.KHNXCaHoc.KeHoachNhomXuong.NhomXuong.IdNhomXuong == IdNhomXuong.Value);
            }

            var result = await query
                .OrderBy(lsdd => lsdd.ThoiGianDiemDanh)
                .Select(lsdd => new
                {
                    lsdd.IdLSDD,
                    NgayHoc = lsdd.ThoiGianDiemDanh,
                    CaHoc = lsdd.KHNXCaHoc.CaHoc.TenCaHoc,
                    DiemDanhMuonToiDa = lsdd.KHNXCaHoc.DiemDanhTre,
                    NoiDung = lsdd.NoiDungBuoiHoc,
                    TrangThaiDiHoc = lsdd.TrangThai, // bạn có thể map sang chuỗi "Có mặt"/"Vắng mặt"
                    NhomXuong = lsdd.KHNXCaHoc.KeHoachNhomXuong.NhomXuong.TenNhomXuong,
                    HocKy = lsdd.KHNXCaHoc.KeHoachNhomXuong.KeHoach.DuAn.HocKy.TenHocKy
                })
                .ToListAsync();

            return Ok(result);
            
        }

        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate([FromQuery] Guid? IdSinhVien, [FromQuery] Guid? IdHocKy, [FromQuery] Guid? IdNhomXuong)
        {
            var query = _context.LichSuDiemDanhs
                .Include(lsdd => lsdd.DiemDanh)
                .Include(lsdd => lsdd.KHNXCaHoc)
                    .ThenInclude(khnxch => khnxch.CaHoc)
                .Include(lsdd => lsdd.KHNXCaHoc)
                    .ThenInclude(khnxch => khnxch.KeHoachNhomXuong)
                        .ThenInclude(khnx => khnx.NhomXuong)
                .Include(lsdd => lsdd.KHNXCaHoc)
                    .ThenInclude(khnxch => khnxch.KeHoachNhomXuong)
                        .ThenInclude(khnx => khnx.KeHoach)
                            .ThenInclude(kh => kh.DuAn)
                                .ThenInclude(da => da.HocKy)
                .AsQueryable();

            if (IdSinhVien.HasValue)
            {
                query = query.Where(lsdd => lsdd.DiemDanh.IdSinhVien == IdSinhVien.Value);
            }
            if (IdHocKy.HasValue)
            {
                query = query.Where(lsdd => lsdd.KHNXCaHoc.KeHoachNhomXuong.KeHoach.DuAn.IdHocKy == IdHocKy.Value);
            }
            if (IdNhomXuong.HasValue)
            {
                query = query.Where(lsdd => lsdd.KHNXCaHoc.KeHoachNhomXuong.NhomXuong.IdNhomXuong == IdNhomXuong.Value);
            }

            var data = await query.OrderBy(lsdd => lsdd.ThoiGianDiemDanh).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("LichSuDiemDanh");

            // Header
            worksheet.Cell(1, 1).Value = "Ngày học";
            worksheet.Cell(1, 2).Value = "Ca học";
            worksheet.Cell(1, 3).Value = "Điểm danh muộn tối đa (phút)";
            worksheet.Cell(1, 4).Value = "Nội dung";
            worksheet.Cell(1, 5).Value = "Trạng thái đi học";
            worksheet.Cell(1, 6).Value = "Nhóm xưởng";
            worksheet.Cell(1, 7).Value = "Học kỳ";

            int row = 2;
            foreach (var item in data)
            {
                var trangThai = item.TrangThai == 1 ? "Có mặt" : "Vắng mặt";

                worksheet.Cell(row, 1).Value = item.ThoiGianDiemDanh.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 2).Value = item.KHNXCaHoc.CaHoc?.TenCaHoc ?? "";
                worksheet.Cell(row, 3).Value = item.KHNXCaHoc.DiemDanhTre;
                worksheet.Cell(row, 4).Value = item.NoiDungBuoiHoc;
                worksheet.Cell(row, 5).Value = trangThai;
                worksheet.Cell(row, 6).Value = item.KHNXCaHoc.KeHoachNhomXuong.NhomXuong?.TenNhomXuong ?? "";
                worksheet.Cell(row, 7).Value = item.KHNXCaHoc.KeHoachNhomXuong.KeHoach.DuAn.HocKy?.TenHocKy ?? "";

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "LichSuDiemDanh.xlsx");
        }
    }
}