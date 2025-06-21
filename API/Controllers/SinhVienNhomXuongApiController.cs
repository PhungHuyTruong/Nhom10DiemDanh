using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SinhVienNhomXuongApiController : ControllerBase
    {

        private readonly ModuleDiemDanhDbContext _context;
        public SinhVienNhomXuongApiController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }
        // GET: api/<SinhVienNhomXuongApiController>
        [HttpGet("{idNhomXuong}")]
        public async Task<IActionResult> GetByNhomXuong(Guid idNhomXuong)
        {
            var sinhViens = await _context.SinhViens
                .Where(x => x.IdNhomXuong == idNhomXuong)
                .ToListAsync();

            return Ok(sinhViens);
        }

        [HttpPost("xoa-khoi-nhom")]
        public async Task<IActionResult> XoaKhoiNhom([FromBody] Guid idSinhVien)
        {
            var sv = await _context.SinhViens.FindAsync(idSinhVien);
            if (sv == null) return NotFound();

            sv.IdNhomXuong = null;
            sv.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("gan-vao-nhom")]
        public async Task<IActionResult> GanSinhVienVaoNhom([FromForm] GanNhomXuongRequest request)
        {
            if (request.IdNhomXuong == Guid.Empty || request.IdSinhVien == null || !request.IdSinhVien.Any())
                return BadRequest("Thiếu dữ liệu.");

            var sinhViens = await _context.SinhViens
                .Where(x => request.IdSinhVien.Contains(x.IdSinhVien))
                .ToListAsync();

            foreach (var sv in sinhViens)
            {
                sv.IdNhomXuong = request.IdNhomXuong;
                sv.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, count = sinhViens.Count });
        }

        [HttpGet("chua-co-nhom")]
        public async Task<IActionResult> GetSinhVienChuaCoNhom([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var query = _context.SinhViens.Where(x => x.IdNhomXuong == null);
            var totalItems = await query.CountAsync();

            var list = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                data = list,
                currentPage = page,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            });
        }

    }
}
