using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaHocsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public CaHocsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var caHoc = await _context.CaHocs.FindAsync(id);
            if (caHoc == null)
                return NotFound();

            return Ok(caHoc);
        }

        // GET: api/CaHoc/ByCoSo/{id}
        [HttpGet("ByCoSo/{coSoId}")]
        public async Task<IActionResult> GetCaHocTheoCoSo(Guid coSoId)
        {
            var caHocs = await _context.CaHocs
                .Where(c => c.CoSoId == coSoId)
                .ToListAsync();

            return Ok(caHocs);
        }

        // POST: api/CaHoc
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CaHoc caHoc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.CaHocs.Add(caHoc);
            await _context.SaveChangesAsync();
            return Ok(caHoc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCaHoc(Guid id, [FromBody] CaHoc caHoc)
        {
            if (id != caHoc.IdCaHoc) return BadRequest();

            var existing = await _context.CaHocs.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TenCaHoc = caHoc.TenCaHoc;
            existing.ThoiGianBatDau = caHoc.ThoiGianBatDau;
            existing.ThoiGianKetThuc = caHoc.ThoiGianKetThuc;
            existing.TrangThai = caHoc.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            _context.CaHocs.Update(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
