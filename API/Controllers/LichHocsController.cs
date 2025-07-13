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
        var sinhVien = await _context.SinhViens
            .AsNoTracking()
            .FirstOrDefaultAsync(sv => sv.IdSinhVien == idSinhVien);

        if (sinhVien == null || sinhVien.IdNhomXuong == null)
            return NotFound("Không tìm thấy sinh viên hoặc sinh viên chưa thuộc nhóm xưởng");

        Guid idNhomXuong = sinhVien.IdNhomXuong.Value;

        var data = await (from khnxch in _context.KHNXCaHocs
                          join khnx in _context.KeHoachNhomXuongs on khnxch.IdKHNX equals khnx.IdKHNX
                          join kh in _context.KeHoachs on khnx.IdKeHoach equals kh.IdKeHoach
                          join nx in _context.NhomXuongs on khnx.IdNhomXuong equals nx.IdNhomXuong
                          join da in _context.DuAns on kh.IdDuAn equals da.IdDuAn
                          join lgd in _context.LichGiangDays on khnxch.IdNXCH equals lgd.IdNXCH
                          join dd in _context.DiaDiems on lgd.IdDiaDiem equals dd.IdDiaDiem
                          join gv in _context.PhuTrachXuongs on nx.IdPhuTrachXuong equals gv.IdNhanVien
                          where nx.IdNhomXuong == idNhomXuong
                          select new LichHocViewDto
                          {
                              TenDuAn = da.TenDuAn,
                              TenNhomXuong = nx.TenNhomXuong,
                              NgayHoc = khnxch.NgayHoc,
                              Buoi = khnxch.Buoi,
                              ThoiGian = khnxch.ThoiGian,
                              NoiDung = khnxch.NoiDung,
                              LinkOnline = khnxch.LinkOnline,
                              TenGiangVien = gv.TenNhanVien,
                              TenDiaDiem = dd.TenDiaDiem
                          }).ToListAsync();

        return Ok(data);
    }



    public class LichHocViewDto
    {
        public string TenDuAn { get; set; }
        public string TenNhomXuong { get; set; }
        public DateTime NgayHoc { get; set; }
        public string Buoi { get; set; }
        public string ThoiGian { get; set; }
        public string NoiDung { get; set; }
        public string LinkOnline { get; set; }
        public string TenGiangVien { get; set; }     // thêm
        public string TenDiaDiem { get; set; }       // thêm
    }

}
