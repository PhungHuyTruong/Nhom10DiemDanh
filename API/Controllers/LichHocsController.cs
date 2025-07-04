using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichHocViewsController : ControllerBase   // Đặt tên khác để tránh nhầm
    {
        private readonly ModuleDiemDanhDbContext _context;

        public LichHocViewsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/LichHocViews
        // Trả danh sách toàn bộ lịch học (JOIN 4 bảng)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichHocDto>>> GetLichHocList()
        {
            var result = await _context.KHNXCaHocs

                // => KHNXCaHoc → KếHoạchNhómXưởng
                .Include(k => k.KeHoachNhomXuong)

                    // => KếHoạch
                    .ThenInclude(knx => knx.KeHoach)

                // => NhómXưởng
                .Include(k => k.KeHoachNhomXuong)
                    .ThenInclude(knx => knx.NhomXuong)

                .Select(k => new LichHocDto
                {
                    IdKHNXCH = k.IdNXCH,
                    TenKeHoach = k.KeHoachNhomXuong.KeHoach.TenKeHoach,
                    TenNhomXuong = k.KeHoachNhomXuong.NhomXuong.TenNhomXuong,
                    TenCaHoc = k.CaHoc.TenCaHoc,     // hoặc k.CaHoc.TenCaHoc nếu có nav‑prop
                    NgayHoc = k.NgayHoc,
                    TrangThai = k.TrangThai
                })
                .ToListAsync();

            return result;
        }

        // GET: api/LichHocViews/5
        // Chi tiết 1 lịch học theo Id KHNXCaHoc
        [HttpGet("{id}")]
        public async Task<ActionResult<LichHocDto>> GetLichHoc(Guid id)
        {
            var item = await _context.KHNXCaHocs
                .Include(k => k.KeHoachNhomXuong)
                    .ThenInclude(knx => knx.KeHoach)
                .Include(k => k.KeHoachNhomXuong)
                    .ThenInclude(knx => knx.NhomXuong)
                .Where(k => k.IdNXCH == id)
                .Select(k => new LichHocDto
                {
                    IdKHNXCH = k.IdNXCH,
                    TenKeHoach = k.KeHoachNhomXuong.KeHoach.TenKeHoach,
                    TenNhomXuong = k.KeHoachNhomXuong.NhomXuong.TenNhomXuong,
                    TenCaHoc = k.CaHoc.TenCaHoc,
                    NgayHoc = k.NgayHoc,
                    TrangThai = k.TrangThai
                })
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();
            return item;
        }

        // ❌ KHÔNG cho phép POST/PUT/DELETE
        [HttpPost, HttpPut, HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]         // Ẩn khỏi Swagger
        public IActionResult BlockWriteActions() =>
            StatusCode(StatusCodes.Status405MethodNotAllowed,
                       "Chỉ cho phép hiển thị dữ liệu.");
    }
    public class LichHocDto
    {
        public Guid IdKHNXCH { get; set; }      // Khóa chính từ KHNXCaHoc
        public string TenKeHoach { get; set; }  // Tên kế hoạch
        public string TenNhomXuong { get; set; } // Tên nhóm xưởng
        public string TenCaHoc { get; set; }    // Tên ca hoặc khung giờ
        public DateTime NgayHoc { get; set; }
        public int TrangThai { get; set; }      // (nếu vẫn muốn hiển thị)
    }

}
