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

    [HttpGet]
    public async Task<IActionResult> GetLichHocTongHop()
    {
        var data = await (from khnxch in _context.KHNXCaHocs
                          join khnx in _context.KeHoachNhomXuongs on khnxch.IdKHNX equals khnx.IdKHNX
                          join kh in _context.KeHoachs on khnx.IdKeHoach equals kh.IdKeHoach
                          join nx in _context.NhomXuongs on khnx.IdNhomXuong equals nx.IdNhomXuong
                          join da in _context.DuAns on kh.IdDuAn equals da.IdDuAn
                          join hk in _context.HocKys on da.IdHocKy equals hk.IdHocKy
                          select new LichHocViewDto
                          {
                              TenHocKy = hk.TenHocKy,
                              TenDuAn = da.TenDuAn,
                              TenKeHoach = kh.TenKeHoach,
                              TenNhomXuong = nx.TenNhomXuong,
                              NgayHoc = khnxch.NgayHoc,
                              Buoi = khnxch.Buoi,
                              ThoiGian = khnxch.ThoiGian,
                              NoiDung = khnxch.NoiDung,
                              LinkOnline = khnxch.LinkOnline
                          }).ToListAsync();

        return Ok(data);
    }

    public class LichHocViewDto
    {
        public string TenHocKy { get; set; }
        public string TenDuAn { get; set; }
        public string TenKeHoach { get; set; }
        public string TenNhomXuong { get; set; }
        public DateTime NgayHoc { get; set; }
        public string Buoi { get; set; }
        public string ThoiGian { get; set; }
        public string NoiDung { get; set; }
        public string LinkOnline { get; set; }
    }
}
