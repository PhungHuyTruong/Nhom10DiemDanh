using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoMonCoSoController : ControllerBase
    {

        private readonly ModuleDiemDanhDbContext _context;

        public BoMonCoSoController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/BoMonCoSo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoMonCoSoViewModel>>> GetBoMonCoSos(string? tenBoMon, Guid? idCoSo)
        {
            var query = _context.BoMonCoSos
                .Include(b => b.QuanLyBoMon)
                .Include(b => b.CoSo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tenBoMon))
            {
                query = query.Where(b => b.QuanLyBoMon.TenBoMon.Contains(tenBoMon));
            }

            if (idCoSo.HasValue)
            {
                query = query.Where(b => b.IdCoSo == idCoSo);
            }

            var result = await query.Select(b => new BoMonCoSoViewModel
            {
                IdBoMonCoSo = b.IdBoMonCoSo,
                IdBoMon = b.IdBoMon,
                TenBoMon = b.QuanLyBoMon != null ? b.QuanLyBoMon.TenBoMon : "Không xác định",
                IdCoSo = b.IdCoSo,
                TenCoSo = b.CoSo != null ? b.CoSo.TenCoSo : "Không xác định",
                TrangThai = b.TrangThai,
                NgayTao = b.NgayTao,
                NgayCapNhat = b.NgayCapNhat
            }).ToListAsync();


            return result;
        }

        // GET: api/BoMonCoSo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BoMonCoSoViewModel>> GetBoMonCoSo(Guid id)
        {
            var boMonCoSo = await _context.BoMonCoSos
                .Include(b => b.QuanLyBoMon)
                .Include(b => b.CoSo)
                .FirstOrDefaultAsync(b => b.IdBoMonCoSo == id);

            if (boMonCoSo == null)
            {
                return NotFound();
            }

            return new BoMonCoSoViewModel
            {
                IdBoMonCoSo = boMonCoSo.IdBoMonCoSo,
                IdBoMon = boMonCoSo.IdBoMon,
                TenBoMon = boMonCoSo.QuanLyBoMon?.TenBoMon ?? "Không xác định",
                IdCoSo = boMonCoSo.IdCoSo,
                TenCoSo = boMonCoSo.CoSo?.TenCoSo ?? "Không xác định",
                TrangThai = boMonCoSo.TrangThai,
                NgayTao = boMonCoSo.NgayTao,
                NgayCapNhat = boMonCoSo.NgayCapNhat
            };
        }

        // POST: api/BoMonCoSo
        [HttpPost]
        public async Task<ActionResult<BoMonCoSoViewModel>> CreateBoMonCoSo(BoMonCoSoViewModel model)
        {
            var boMonCoSo = new BoMonCoSo
            {
                IdBoMonCoSo = Guid.NewGuid(),
                IdBoMon = model.IdBoMon,
                IdCoSo = model.IdCoSo,
                TrangThai = model.TrangThai,
                NgayTao = DateTime.Now
            };

            _context.BoMonCoSos.Add(boMonCoSo);
            await _context.SaveChangesAsync();

            model.IdBoMonCoSo = boMonCoSo.IdBoMonCoSo;
            model.NgayTao = boMonCoSo.NgayTao;
            return CreatedAtAction(nameof(GetBoMonCoSo), new { id = boMonCoSo.IdBoMonCoSo }, model);
        }

        // PUT: api/BoMonCoSo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoMonCoSo(Guid id, BoMonCoSoViewModel model)
        {
            if (id != model.IdBoMonCoSo)
            {
                return BadRequest();
            }

            var boMonCoSo = await _context.BoMonCoSos.FindAsync(id);
            if (boMonCoSo == null)
            {
                return NotFound();
            }

            boMonCoSo.IdBoMon = model.IdBoMon;
            boMonCoSo.IdCoSo = model.IdCoSo;
            boMonCoSo.TrangThai = model.TrangThai;
            boMonCoSo.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/BoMonCoSo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoMonCoSo(Guid id)
        {
            var boMonCoSo = await _context.BoMonCoSos.FindAsync(id);
            if (boMonCoSo == null)
            {
                return NotFound();
            }

            _context.BoMonCoSos.Remove(boMonCoSo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/BoMonCoSo/GetCoSos
        [HttpGet("GetCoSos")]
        public async Task<ActionResult<IEnumerable<CoSo>>> GetCoSos()
        {
            return await _context.CoSos.ToListAsync();
        }

        // GET: api/BoMonCoSo/GetBoMons
        [HttpGet("GetBoMons")]
        public async Task<ActionResult<IEnumerable<QuanLyBoMon>>> GetBoMons()
        {
            return await _context.QuanLyBoMons.ToListAsync();
        }
    }
}
