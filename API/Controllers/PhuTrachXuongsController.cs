using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhuTrachXuongsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public PhuTrachXuongsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/PhuTrachXuongs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhuTrachXuong>>> GetAll()
        {
            var result = await _context.PhuTrachXuongs
                .Include(p => p.CoSo)
                .Include(p => p.DiemDanhs)
                .Include(p => p.NhomXuongs)
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/PhuTrachXuongs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PhuTrachXuong>> GetById(Guid id)
        {
            var item = await _context.PhuTrachXuongs
                .Include(p => p.CoSo)
                .Include(p => p.DiemDanhs)
                .Include(p => p.NhomXuongs)
                .FirstOrDefaultAsync(p => p.IdNhanVien == id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/PhuTrachXuongs
        [HttpPost]
        public async Task<ActionResult<PhuTrachXuong>> Create(PhuTrachXuong model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.IdNhanVien = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.NgayCapNhat = DateTime.Now;

            _context.PhuTrachXuongs.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.IdNhanVien }, model);
        }

        // PUT: api/PhuTrachXuongs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PhuTrachXuong model)
        {
            if (id != model.IdNhanVien)
                return BadRequest("Id không khớp");

            var existing = await _context.PhuTrachXuongs.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Cập nhật thông tin
            existing.TenNhanVien = model.TenNhanVien;
            existing.MaNhanVien = model.MaNhanVien;
            existing.EmailFE = model.EmailFE;
            existing.EmailFPT = model.EmailFPT;
            existing.IdCoSo = model.IdCoSo;
            existing.TrangThai = model.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/PhuTrachXuongs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.PhuTrachXuongs.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.PhuTrachXuongs.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
