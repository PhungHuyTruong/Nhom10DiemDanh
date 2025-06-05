using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: api/IPs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IP>>> GetIPs()
        {
            return await _context.IPs.ToListAsync();
        }

        // GET: api/IPs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IP>> GetIP(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);

            if (ip == null)
            {
                return NotFound();
            }

            return ip;
        }

        // PUT: api/IPs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIP(Guid id, IP ip)
        {
            if (id != ip.IdIP)
            {
                return BadRequest();
            }

            var existingIP = await _context.IPs.FindAsync(id);
            if (existingIP == null)
            {
                return NotFound();
            }

            existingIP.KieuIP = ip.KieuIP;
            existingIP.IP_DaiIP = ip.IP_DaiIP;
            existingIP.TrangThai = ip.TrangThai;
            existingIP.NgayCapNhat = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IPExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/IPs
        // POST: api/IPs
        [HttpPost]
        public async Task<ActionResult<IP>> PostIP(IP ip)
        {
            if (ip == null || string.IsNullOrEmpty(ip.KieuIP) || string.IsNullOrEmpty(ip.IP_DaiIP))
            {
                return BadRequest("KieuIP and IP_DaiIP are required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tự tạo IdIP nếu chưa có
            if (ip.IdIP == Guid.Empty)
            {
                ip.IdIP = Guid.NewGuid();
            }

            ip.NgayTao = DateTime.Now;

            _context.IPs.Add(ip);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IPExists(ip.IdIP))
                {
                    return Conflict();
                }
                throw;
            }

            return CreatedAtAction(nameof(GetIP), new { id = ip.IdIP }, ip);
        }


        // DELETE: api/IPs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIP(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null)
            {
                return NotFound();
            }

            _context.IPs.Remove(ip);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IPExists(Guid id)
        {
            return _context.IPs.Any(e => e.IdIP == id);
        }
    }
}