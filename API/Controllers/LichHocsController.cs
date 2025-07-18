using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ thêm dòng này

[Route("api/[controller]")]
[ApiController]
public class LichHocViewsController : ControllerBase
{
    private readonly ModuleDiemDanhDbContext _context;

    public LichHocViewsController(ModuleDiemDanhDbContext context)
    {
        _context = context;
    }

    [HttpGet("sinhvien/{idSinhVien}")]
    public async Task<IActionResult> GetLichHocTheoSinhVien(Guid idSinhVien)
    {
        // Lấy nhóm xưởng của sinh viên
        var sinhVien = await _context.SinhViens
            .AsNoTracking()
            .FirstOrDefaultAsync(sv => sv.IdSinhVien == idSinhVien);

        if (sinhVien == null || sinhVien.IdNhomXuong == null)
            return NotFound("Không tìm thấy sinh viên hoặc sinh viên chưa thuộc nhóm xưởng");

        Guid idNhomXuong = sinhVien.IdNhomXuong.Value;

        // Truy vấn lịch học thuộc nhóm xưởng của sinh viên
        var data = await (
                    from khnxch in _context.KHNXCaHocs
                    join khnx in _context.KeHoachNhomXuongs on khnxch.IdKHNX equals khnx.IdKHNX
                    join kh in _context.KeHoachs on khnx.IdKeHoach equals kh.IdKeHoach
                    join nx in _context.NhomXuongs on khnx.IdNhomXuong equals nx.IdNhomXuong
                    join da in _context.DuAns on kh.IdDuAn equals da.IdDuAn
                    join gv in _context.PhuTrachXuongs on nx.IdPhuTrachXuong equals gv.IdNhanVien
                    join coSo in _context.CoSos on gv.IdCoSo equals coSo.IdCoSo into csJoin
                    from cs in csJoin.DefaultIfEmpty()
                    join diaDiem in _context.DiaDiems on cs.IdCoSo equals diaDiem.IdCoSo into ddJoin
                    from dd in ddJoin.DefaultIfEmpty()
                    join ca in _context.CaHocs on khnxch.IdCaHoc equals ca.IdCaHoc into caJoin
                    from caHoc in caJoin.DefaultIfEmpty()
                    where nx.IdNhomXuong == idNhomXuong
                    orderby khnxch.NgayHoc
                    select new LichHocViewDto
                    {
                        NgayHoc = khnxch.NgayHoc,
                        Buoi = khnxch.Buoi,
                        ThoiGian = khnxch.ThoiGian,
                        TenCaHoc = caHoc != null ? caHoc.TenCaHoc : "Không rõ",
                        TenNhomXuong = nx.TenNhomXuong,
                        TenDuAn = da.TenDuAn,
                        LinkOnline = khnxch.LinkOnline,
                        DiaDiem = dd != null ? dd.TenDiaDiem : "Chưa cập nhật",
                        GiangVienPhuTrach = gv.TenNhanVien,
                        MoTa = "Chi tiết"
                    }
                ).ToListAsync();





        return Ok(data);
    }


    public class LichHocViewDto
    {
        public DateTime NgayHoc { get; set; }

        public string Buoi { get; set; }              // VD: "Buổi 1"
        public string TenCaHoc { get; set; }          // VD: "Ca 1"
        public string ThoiGian { get; set; }          // VD: "21:00 - 23:00"

        public string Ca => $"{TenCaHoc} - {(string.IsNullOrEmpty(ThoiGian) ? "Offline" : ThoiGian)}";

        public string TenNhomXuong { get; set; }
        public string TenDuAn { get; set; }
        public string LinkOnline { get; set; }
        public string DiaDiem { get; set; }
        public string GiangVienPhuTrach { get; set; }
        public string MoTa { get; set; } = "Chi tiết";
    }

}
