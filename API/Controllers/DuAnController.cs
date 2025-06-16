using API.Data;
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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.DuAns
                    .Where(x => x.TrangThai == true)
                    .Select(x => new
                    {
                        idDuAn = x.IdDuAn,
                        tenDuAn = x.TenDuAn
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi nội bộ: {ex.Message}");
            }
        }

        // GET: api/DuAns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DuAn>> GetDuAn(Guid id)
        {
            var duAn = await _context.DuAns
                .Include(x => x.CapDoDuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.HocKy)
                .FirstOrDefaultAsync(x => x.IdDuAn == id);

            if (duAn == null)
                return NotFound();

            return duAn;
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
        public async Task<IActionResult> PutDuAn(Guid id, DuAn duAn)
        {
            if (id != duAn.IdDuAn)
                return BadRequest("Id không khớp");

            var existing = await _context.DuAns.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.TenDuAn = duAn.TenDuAn;
            existing.MaDuAn = duAn.MaDuAn;
            existing.MoTa = duAn.MoTa;
            existing.IdCDDA = duAn.IdCDDA;
            existing.IdBoMon = duAn.IdBoMon;
            existing.IdHocKy = duAn.IdHocKy;
            existing.TrangThai = duAn.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            _context.Entry(existing).State = EntityState.Modified;
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
