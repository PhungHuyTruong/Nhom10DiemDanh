using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoSoController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public CoSoController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoSoViewModel>>> GetCoSos(
     [FromQuery] string tenCoSo = null,
     [FromQuery] string trangThai = null)
        {
            var coSos = _context.CoSos.AsQueryable();

            if (!string.IsNullOrEmpty(tenCoSo))
            {
                coSos = coSos.Where(c => c.TenCoSo.Contains(tenCoSo));
            }

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
            {
                bool isActive = trangThai == "Hoạt động";
                coSos = coSos.Where(c => c.TrangThai == isActive);
            }

            var coSoViewModels = await coSos.Select(c => new CoSoViewModel
            {
                IdCoSo = c.IdCoSo,
                TenCoSo = c.TenCoSo,
                MaCoSo = c.MaCoSo,
                DiaChi = c.DiaChi,
                SDT = c.SDT,
                Email = c.Email,
                TrangThai = c.TrangThai ? "Hoạt động" : "Tắt",
                IdDiaDiem = c.IdDiaDiem,
                //IdIP = c.IdIP,
                //IdCaHoc = c.IdCaHoc
            }).ToListAsync();

            return Ok(coSoViewModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoSoViewModel>> GetCoSo(Guid id)
        {
            var coSo = await _context.CoSos.FirstOrDefaultAsync(c => c.IdCoSo == id);

            if (coSo == null)
            {
                return NotFound();
            }

            var coSoViewModel = new CoSoViewModel
            {
                IdCoSo = coSo.IdCoSo,
                TenCoSo = coSo.TenCoSo,
                MaCoSo = coSo.MaCoSo,
                DiaChi = coSo.DiaChi,
                SDT = coSo.SDT,
                Email = coSo.Email,
                TrangThai = coSo.TrangThai ? "Hoạt động" : "Tắt",
                IdDiaDiem = coSo.IdDiaDiem,
                //IdIP = coSo.IdIP,
                //IdCaHoc = coSo.IdCaHoc
            };

            return Ok(coSoViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<CoSoViewModel>> CreateCoSo(CoSoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var coSo = new CoSo
            {
                TenCoSo = model.TenCoSo,
                MaCoSo = model.MaCoSo,
                DiaChi = model.DiaChi,
                SDT = model.SDT,
                Email = model.Email,
                TrangThai = model.TrangThai == "Hoạt động",
                IdDiaDiem = model.IdDiaDiem,
                //IdIP = model.IdIP,
                //IdCaHoc = model.IdCaHoc
            };

            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            model.IdCoSo = coSo.IdCoSo;
            return CreatedAtAction(nameof(GetCoSo), new { id = coSo.IdCoSo }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoSo(Guid id, CoSoViewModel model)
        {
            if (id != model.IdCoSo)
            {
                return BadRequest();
            }

            var coSo = await _context.CoSos.FindAsync(id);
            if (coSo == null)
            {
                return NotFound();
            }

            coSo.TenCoSo = model.TenCoSo;
            coSo.MaCoSo = model.MaCoSo;
            coSo.DiaChi = model.DiaChi;
            coSo.SDT = model.SDT;
            coSo.Email = model.Email;
            coSo.TrangThai = model.TrangThai == "Hoạt động";
            coSo.IdDiaDiem = model.IdDiaDiem;
            //coSo.IdIP = model.IdIP;
            //coSo.IdCaHoc = model.IdCaHoc;
            coSo.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoSo(Guid id)
        {
            var coSo = await _context.CoSos.FindAsync(id);
            if (coSo == null)
            {
                return NotFound();
            }

            _context.CoSos.Remove(coSo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var coSo = await _context.CoSos.FindAsync(id);
            if (coSo == null)
            {
                return NotFound();
            }

            coSo.TrangThai = !coSo.TrangThai;
            coSo.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}