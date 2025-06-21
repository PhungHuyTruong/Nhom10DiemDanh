using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichHocsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public LichHocsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/LichHocs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichHoc>>> GetLichHocs()
        {
            return await _context.LichHocs.ToListAsync();
        }

        // GET: api/LichHocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LichHoc>> GetLichHoc(Guid id)
        {
            var lichHoc = await _context.LichHocs.FindAsync(id);

            if (lichHoc == null)
            {
                return NotFound();
            }

            return lichHoc;
        }

        // PUT: api/LichHocs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLichHoc(Guid id, LichHoc lichHoc)
        {
            if (id != lichHoc.IDLichHoc)
            {
                return BadRequest();
            }

            _context.Entry(lichHoc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LichHocExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LichHocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LichHoc>> PostLichHoc(LichHoc lichHoc)
        {
            _context.LichHocs.Add(lichHoc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLichHoc", new { id = lichHoc.IDLichHoc }, lichHoc);
        }

        // DELETE: api/LichHocs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLichHoc(Guid id)
        {
            var lichHoc = await _context.LichHocs.FindAsync(id);
            if (lichHoc == null)
            {
                return NotFound();
            }

            _context.LichHocs.Remove(lichHoc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LichHocExists(Guid id)
        {
            return _context.LichHocs.Any(e => e.IDLichHoc == id);
        }
    }
}
