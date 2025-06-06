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
    public class KHNXCaHocsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public KHNXCaHocsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/KHNXCaHocs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KHNXCaHoc>>> GetKHNXCaHocs()
        {
            var kHNXCaHocs = await _context.KHNXCaHocs
                .Include(k => k.KeHoachNhomXuong)
                .Include(c => c.CaHoc)
                .ToListAsync();
            if (kHNXCaHocs == null || !kHNXCaHocs.Any())
            {
                return NotFound(new { success = false, message = "Không có ca học nào." });
            }
            return Ok(new { success = true, message = "Lấy danh sách ca học thành công", data = kHNXCaHocs });
        }

        // GET: api/KHNXCaHocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KHNXCaHoc>> GetKHNXCaHoc(Guid id)
        {
            var kHNXCaHoc = await _context.KHNXCaHocs
                .Include(k => k.KeHoachNhomXuong)
                .Include(c => c.CaHoc)
                .FirstOrDefaultAsync(k => k.IdNXCH == id);
            if (kHNXCaHoc == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });
            }
            return Ok(new { success = true, message = "Lấy ca học thành công", data = kHNXCaHoc });
        }

        // PUT: api/KHNXCaHocs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKHNXCaHoc(Guid id, KHNXCaHoc kHNXCaHoc)
        {
            if (id != kHNXCaHoc.IdNXCH)
            {
                return BadRequest(new { success = false, message = "ID không khớp." });
            }
            _context.Entry(kHNXCaHoc).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KHNXCaHocExists(id))
                {
                    return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool KHNXCaHocExists(Guid id)
        {
            throw new NotImplementedException();
        }

        // POST: api/KHNXCaHocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KHNXCaHoc>> PostKHNXCaHoc(KHNXCaHoc kHNXCaHoc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            kHNXCaHoc.IdNXCH = Guid.NewGuid();
            kHNXCaHoc.NgayTao = DateTime.Now;
            kHNXCaHoc.NgayCapNhat = DateTime.Now;
            _context.KHNXCaHocs.Add(kHNXCaHoc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetKHNXCaHoc), new { id = kHNXCaHoc.IdNXCH }, new
            {
                success = true,
                message = "Thêm ca học thành công",
                data = kHNXCaHoc
            });
        }

        // DELETE: api/KHNXCaHocs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKHNXCaHoc(Guid id)
        {
            var kHNXCaHoc = await _context.KHNXCaHocs.FindAsync(id);
            if (kHNXCaHoc == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });
            }
            _context.KHNXCaHocs.Remove(kHNXCaHoc);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Xóa ca học thành công" });
        }
  
    }
}
