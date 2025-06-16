using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public IPsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null) return NotFound();
            return Ok(ip);
        }

        [HttpGet("ByCoSo/{id}")]
        public async Task<IActionResult> GetByCoSo(Guid id)
        {
            var result = await _context.IPs.Where(x => x.IdCoSo == id).ToListAsync();
            return Ok(result);
        }

        // ✅ POST: api/IPs
        [HttpPost]
        public async Task<IActionResult> Post(IP ip)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ip.IdIP = Guid.NewGuid(); // đảm bảo có ID
            ip.NgayCapNhat = DateTime.Now;

            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = ip.IdIP }, ip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] IP ip)
        {
            if (id != ip.IdIP) return BadRequest();

            var existing = await _context.IPs.FindAsync(id);
            if (existing == null) return NotFound();

            existing.KieuIP = ip.KieuIP;
            existing.IP_DaiIP = ip.IP_DaiIP;
            existing.TrangThai = ip.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            _context.IPs.Update(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
