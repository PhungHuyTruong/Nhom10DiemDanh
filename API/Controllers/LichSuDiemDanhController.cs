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
            // Lấy tất cả các ca học mà sinh viên thuộc nhóm xưởng đó
            var caHocQuery = _context.KHNXCaHocs
                .Include(k => k.CaHoc)
                .Include(k => k.KeHoachNhomXuong)
                    .ThenInclude(knx => knx.NhomXuong)
                .Include(k => k.KeHoachNhomXuong)
                    .ThenInclude(knx => knx.KeHoach)
                        .ThenInclude(kh => kh.DuAn)
                            .ThenInclude(da => da.HocKy)
                .AsQueryable();

            if (IdHocKy.HasValue)
                caHocQuery = caHocQuery.Where(x => x.KeHoachNhomXuong.KeHoach.DuAn.IdHocKy == IdHocKy.Value);
            if (IdNhomXuong.HasValue)
                caHocQuery = caHocQuery.Where(x => x.KeHoachNhomXuong.NhomXuong.IdNhomXuong == IdNhomXuong.Value);

            // Lọc các ca học mà sinh viên thuộc nhóm xưởng đó (bạn cần xác định logic này, ví dụ dựa vào bảng phân công nhóm xưởng)
            // Ví dụ: caHocQuery = caHocQuery.Where(x => x.KeHoachNhomXuong.NhomXuong.SinhViens.Any(sv => sv.IdSinhVien == IdSinhVien));

            var caHocs = await caHocQuery.ToListAsync();

            // Lấy tất cả điểm danh của sinh viên này
            var diemDanhs = await _context.LichSuDiemDanhs
                .Include(lsdd => lsdd.DiemDanh)
                .Where(lsdd => lsdd.DiemDanh.IdSinhVien == IdSinhVien)
                .ToListAsync();

            // Kết hợp ca học với điểm danh
            var result = caHocs.Select(caHoc =>
            {
                var gioBatDau = caHoc.NgayHoc.Date + (caHoc.CaHoc?.ThoiGianBatDau ?? TimeSpan.Zero);
                var gioKetThuc = caHoc.NgayHoc.Date + (caHoc.CaHoc?.ThoiGianKetThuc ?? TimeSpan.Zero);

                var diemDanhList = diemDanhs.Where(d => d.IdNXCH == caHoc.IdNXCH).ToList();
                DateTime? checkIn = diemDanhList
                    .Where(d => d.TrangThai == 3 || d.TrangThai == 4)
                    .OrderBy(d => d.ThoiGianDiemDanh)
                    .Select(d => (DateTime?)d.ThoiGianDiemDanh)
                    .FirstOrDefault();

                DateTime? checkOut = diemDanhList
                    .Where(d => d.TrangThai == 5 || d.TrangThai == 6)
                    .OrderByDescending(d => d.ThoiGianDiemDanh)
                    .Select(d => (DateTime?)d.ThoiGianDiemDanh)
                    .FirstOrDefault();

                var now = DateTime.Now;

                // Khởi tạo mặc định
                string trangThai = "Vắng mặt";
                string checkInStr = "Chưa checkin";
                string checkOutStr = "Chưa checkout";

                // Nếu lịch học chưa đến
                if (now < gioBatDau)
                {
                    trangThai = "Chưa diễn ra";
                    checkInStr = "Chưa checkin";
                    checkOutStr = "Chưa checkout";
                }
                else if (now >= gioBatDau && now <= gioKetThuc)
                {
                    trangThai = "Đang diễn ra";
                    checkInStr = checkIn != null ? checkIn.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa checkin";
                    checkOutStr = checkOut != null ? checkOut.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa checkout";
                }
                else // Đã qua ca học
                {
                    if (checkIn == null && checkOut == null)
                    {
                        trangThai = "Vắng mặt";
                        checkInStr = "Chưa checkin";
                        checkOutStr = "Chưa checkout";
                    }
                    else if (checkIn != null && checkOut == null)
                    {
                        trangThai = "Đã check-in";
                        checkInStr = checkIn.Value.ToString("dd/MM/yyyy HH:mm");
                        checkOutStr = "Chưa checkout";
                    }
                    else if (checkIn == null && checkOut != null)
                    {
                        trangThai = "Đã check-out";
                        checkInStr = "Chưa checkin";
                        checkOutStr = checkOut.Value.ToString("dd/MM/yyyy HH:mm");
                    }
                    else if (checkIn != null && checkOut != null)
                    {
                        trangThai = "Có mặt";
                        checkInStr = checkIn.Value.ToString("dd/MM/yyyy HH:mm");
                        checkOutStr = checkOut.Value.ToString("dd/MM/yyyy HH:mm");
                    }
                }

                return new
                {
                    NgayHoc = caHoc.NgayHoc.Date + (caHoc.CaHoc?.ThoiGianBatDau ?? TimeSpan.Zero),
                    CaHoc = caHoc.CaHoc?.TenCaHoc ?? "",
                    DiemDanhMuonToiDa = caHoc.DiemDanhTre,
                    NoiDung = caHoc.NoiDung ?? "Không có mô tả",
                    NhomXuong = caHoc.KeHoachNhomXuong.NhomXuong.TenNhomXuong,
                    HocKy = caHoc.KeHoachNhomXuong.KeHoach.DuAn.HocKy.TenHocKy,
                    CheckIn = checkInStr,
                    CheckOut = checkOutStr,
                    TrangThai = trangThai,
                    // Thêm 2 dòng này:
                    GioBatDau = caHoc.CaHoc?.ThoiGianBatDau != null ? caHoc.CaHoc.ThoiGianBatDau.Value.ToString(@"hh\:mm") : "",
                    GioKetThuc = caHoc.CaHoc?.ThoiGianKetThuc != null ? caHoc.CaHoc.ThoiGianKetThuc.Value.ToString(@"hh\:mm") : ""
                };
            })
            .OrderBy(x => x.NgayHoc)
            .ToList();  

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
            // Header
            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Ngày học";
            worksheet.Cell(1, 3).Value = "Ca học";
            worksheet.Cell(1, 4).Value = "Điểm danh muộn";
            worksheet.Cell(1, 5).Value = "Nội dung";
            worksheet.Cell(1, 6).Value = "Check in";
            worksheet.Cell(1, 7).Value = "Check out";
            worksheet.Cell(1, 8).Value = "Trạng thái";
            worksheet.Cell(1, 9).Value = "Nhóm xưởng";
            worksheet.Cell(1, 10).Value = "Học kỳ";

            // Group dữ liệu theo buổi học
            var grouped = data
                .GroupBy(lsdd => new { lsdd.IdNXCH, lsdd.IdDiemDanh })
                .Select(g =>
                {
                    var first = g.First();
                    var checkIn = g.Where(x => x.TrangThai == 3 || x.TrangThai == 4)
                                   .OrderBy(x => x.ThoiGianDiemDanh)
                                   .Select(x => (DateTime?)x.ThoiGianDiemDanh)
                                   .FirstOrDefault();
                    var checkOut = g.Where(x => x.TrangThai == 5 || x.TrangThai == 6)
                                    .OrderBy(x => x.ThoiGianDiemDanh)
                                    .Select(x => (DateTime?)x.ThoiGianDiemDanh)
                                    .FirstOrDefault();
                    var ngayHoc = first.KHNXCaHoc?.NgayHoc ?? first.ThoiGianDiemDanh;

                    string trangThai;
                    if (ngayHoc > DateTime.Now)
                        trangThai = "Chưa diễn ra";
                    else if (checkIn != null && checkOut == null)
                        trangThai = "Đã check-in";
                    else if (checkIn == null && checkOut != null)
                        trangThai = "Đã check-out";
                    else if (checkIn != null && checkOut != null)
                        trangThai = "Có mặt";
                    else
                        trangThai = "Vắng mặt";

                    return new
                    {
                        NgayHoc = ngayHoc,
                        CaHoc = first.KHNXCaHoc?.CaHoc?.TenCaHoc ?? "",
                        DiemDanhMuonToiDa = (first.KHNXCaHoc?.DiemDanhTre ?? "") + " phút",
                        NoiDung = first.KHNXCaHoc != null && !string.IsNullOrEmpty(first.KHNXCaHoc.NoiDung) ? first.KHNXCaHoc.NoiDung : (first.NoiDungBuoiHoc ?? "Không có mô tả"),
                        CheckIn = checkIn,
                        CheckOut = checkOut,
                        TrangThai = trangThai,
                        NhomXuong = first.KHNXCaHoc?.KeHoachNhomXuong?.NhomXuong?.TenNhomXuong ?? "",
                        HocKy = first.KHNXCaHoc?.KeHoachNhomXuong?.KeHoach?.DuAn?.HocKy?.TenHocKy ?? ""
                    };
                })
                .OrderBy(x => x.NgayHoc)
                .ToList();

            int row = 2;
            int stt = 1;
            foreach (var item in grouped)
            {
                worksheet.Cell(row, 1).Value = stt;
                worksheet.Cell(row, 2).Value = item.NgayHoc.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 3).Value = item.CaHoc;
                worksheet.Cell(row, 4).Value = item.DiemDanhMuonToiDa;
                worksheet.Cell(row, 5).Value = item.NoiDung;
                worksheet.Cell(row, 6).Value = item.CheckIn == null ? "Chưa checkin" : item.CheckIn.Value.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 7).Value = item.CheckOut == null ? "Chưa checkout" : item.CheckOut.Value.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 8).Value = item.TrangThai;
                worksheet.Cell(row, 9).Value = item.NhomXuong;
                worksheet.Cell(row, 10).Value = item.HocKy;
                row++;
                stt++;
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