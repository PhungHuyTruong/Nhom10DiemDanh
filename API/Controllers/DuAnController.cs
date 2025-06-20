using API.Data;
using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DuAnController : Controller
    {
        private readonly ModuleDiemDanhDbContext _context;

        public DuAnController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/DuAns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DuAnDto>>> GetAll()
        {
            var data = await _context.DuAns
                .Include(x => x.CapDoDuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.HocKy)
                .Select(x => new DuAnDto
                {
                    IdDuAn = x.IdDuAn,
                    TenDuAn = x.TenDuAn,
                    MoTa = x.MoTa,
                    TrangThai = x.TrangThai,
                    IdCDDA = x.IdCDDA,
                    TenCapDo = x.CapDoDuAn != null ? x.CapDoDuAn.TenCapDoDuAn : null,
                    IdBoMon = x.IdBoMon,
                    TenBoMon = x.QuanLyBoMon != null ? x.QuanLyBoMon.TenBoMon : null,
                    IdHocKy = x.IdHocKy,
                    TenHocKy = x.HocKy != null ? x.HocKy.TenHocKy : null
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/DuAns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DuAnDto>> GetDuAn(Guid id)
        {
            var duAn = await _context.DuAns
                .Include(x => x.CapDoDuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.HocKy)
                .FirstOrDefaultAsync(x => x.IdDuAn == id);

            if (duAn == null)
                return NotFound();

            var dto = new DuAnDto
            {
                IdDuAn = duAn.IdDuAn,
                TenDuAn = duAn.TenDuAn,
                MoTa = duAn.MoTa,
                TrangThai = duAn.TrangThai,
                TenCapDo = duAn.CapDoDuAn?.TenCapDoDuAn,
                TenHocKy = duAn.HocKy?.TenHocKy,
                TenBoMon = duAn.QuanLyBoMon?.TenBoMon
            };

            return Ok(dto);
        }

        // POST: api/DuAns
        [HttpPost]
        public async Task<ActionResult<DuAn>> PostDuAn(DuAn duAn)
        {
            duAn.IdDuAn = Guid.NewGuid();
            duAn.NgayTao = DateTime.Now;
            _context.DuAns.Add(duAn);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDuAn), new { id = duAn.IdDuAn }, duAn);
        }

        // PUT: api/DuAns/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDuAn(Guid id, DuAnDto dto)
        {
            if (id != dto.IdDuAn)
                return BadRequest("Id không khớp.");

            var duAn = await _context.DuAns.FindAsync(id);
            if (duAn == null)
                return NotFound();

            // Kiểm tra khóa ngoại có tồn tại
            if (!await _context.CapDoDuAns.AnyAsync(x => x.IdCDDA == dto.IdCDDA))
                return BadRequest("IdCDDA không hợp lệ.");

            if (!await _context.QuanLyBoMons.AnyAsync(x => x.IDBoMon == dto.IdBoMon))
                return BadRequest("IdBoMon không hợp lệ.");

            if (!await _context.HocKys.AnyAsync(x => x.IdHocKy == dto.IdHocKy))
                return BadRequest("IdHocKy không hợp lệ.");

            // Gán giá trị từ DTO vào entity
            duAn.TenDuAn = dto.TenDuAn;
            duAn.MoTa = dto.MoTa;
            duAn.TrangThai = dto.TrangThai;
            duAn.IdCDDA = dto.IdCDDA;
            duAn.IdBoMon = dto.IdBoMon;
            duAn.IdHocKy = dto.IdHocKy;
            duAn.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/DuAns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDuAn(Guid id)
        {
            var duAn = await _context.DuAns.FindAsync(id);
            if (duAn == null)
                return NotFound();

            _context.DuAns.Remove(duAn);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
