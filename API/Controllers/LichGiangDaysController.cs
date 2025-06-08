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
    public class LichGiangDaysController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public LichGiangDaysController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/LichGiangDays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichGiangDay>>> GetLichGiangDays()
        {
            return await _context.LichGiangDays.ToListAsync();
        }

        // GET: api/LichGiangDays/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LichGiangDay>> GetLichGiangDay(Guid id)
        {
            var lichGiangDay = await _context.LichGiangDays.FindAsync(id);

            if (lichGiangDay == null)
            {
                return NotFound();
            }

            return lichGiangDay;
        }

        // PUT: api/LichGiangDays/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLichGiangDay(Guid id, LichGiangDay lichGiangDay)
        {
            if (id != lichGiangDay.IdLichDay)
            {
                return BadRequest();
            }

            _context.Entry(lichGiangDay).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LichGiangDayExists(id))
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

        // POST: api/LichGiangDays
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LichGiangDay>> PostLichGiangDay(LichGiangDay lichGiangDay)
        {
            _context.LichGiangDays.Add(lichGiangDay);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLichGiangDay", new { id = lichGiangDay.IdLichDay }, lichGiangDay);
        }

        // DELETE: api/LichGiangDays/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLichGiangDay(Guid id)
        {
            var lichGiangDay = await _context.LichGiangDays.FindAsync(id);
            if (lichGiangDay == null)
            {
                return NotFound();
            }

            _context.LichGiangDays.Remove(lichGiangDay);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LichGiangDayExists(Guid id)
        {
            return _context.LichGiangDays.Any(e => e.IdLichDay == id);
        }
    }
}
