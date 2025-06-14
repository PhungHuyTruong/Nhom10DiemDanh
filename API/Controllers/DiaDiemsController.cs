using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiaDiemController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;
        public DiaDiemController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.DiaDiems.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var diaDiem = await _context.DiaDiems.FindAsync(id);
            if (diaDiem == null) return NotFound();
            return Ok(diaDiem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiaDiem diaDiem)
        {
            diaDiem.IdDiaDiem = Guid.NewGuid();
            diaDiem.NgayTao = DateTime.Now;
            //diaDiem.TrangThai = true;

            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            return Ok(diaDiem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DiaDiem updatedDiaDiem)
        {
            var diaDiem = await _context.DiaDiems.FindAsync(id);
            if (diaDiem == null) return NotFound();

            diaDiem.TenDiaDiem = updatedDiaDiem.TenDiaDiem;
            diaDiem.ViDo = updatedDiaDiem.ViDo;
            diaDiem.KinhDo = updatedDiaDiem.KinhDo;
            diaDiem.BanKinh = updatedDiaDiem.BanKinh;
            diaDiem.TrangThai = updatedDiaDiem.TrangThai;
            diaDiem.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(diaDiem);
        }

        [HttpPost("doi-trang-thai/{id}")]
        public async Task<IActionResult> DoiTrangThai(Guid id)
        {
            var diaDiem = await _context.DiaDiems.FindAsync(id);
            if (diaDiem == null) return NotFound();

            diaDiem.TrangThai = !diaDiem.TrangThai;
            diaDiem.NgayCapNhat = DateTime.Now;

            _context.DiaDiems.Update(diaDiem);
            await _context.SaveChangesAsync();

            return Ok(diaDiem);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var diaDiem = await _context.DiaDiems.FindAsync(id);
            if (diaDiem == null)
                return NotFound("Không tìm thấy địa điểm.");

            _context.DiaDiems.Remove(diaDiem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("by-coso/{idCoSo}")]
        public async Task<IActionResult> GetByCoSo(Guid idCoSo)
        {
            var diaDiems = await _context.DiaDiems
                .Where(dd => dd.IdCoSo == idCoSo)
                .ToListAsync();

            return Ok(diaDiems);
        }
    }
}
