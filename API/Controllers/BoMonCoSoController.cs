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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoMonCoSoViewModel>>> GetBoMonCoSos(string? tenBoMon, Guid? idCoSo)
        {
            var query = _context.BoMonCoSos
                .Include(b => b.QuanLyBoMon)
                .Include(b => b.CoSo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tenBoMon))
            {
                query = query.Where(b => b.QuanLyBoMon != null && b.QuanLyBoMon.TenBoMon.Contains(tenBoMon));
            }

            if (idCoSo.HasValue && idCoSo != Guid.Empty)
            {
                query = query.Where(b => b.IdCoSo == idCoSo);
            }

            var result = await query.Select(b => new BoMonCoSoViewModel
            {
                IdBoMonCoSo = b.IdBoMonCoSo,
                IdBoMon = b.IdBoMon,
                TenBoMon = b.QuanLyBoMon != null ? b.QuanLyBoMon.TenBoMon : "Không xác định",
                IdCoSo = b.IdCoSo,
                TenCoSo = b.CoSo != null ? b.CoSo.TenCoSo : "Không xác định", // Hiển thị tên cơ sở thực tế
                TrangThai = b.TrangThai,
                NgayTao = b.NgayTao,
                NgayCapNhat = b.NgayCapNhat
            }).ToListAsync();

            return Ok(result);
        }

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

        [HttpPost]
        public async Task<ActionResult<BoMonCoSoViewModel>> CreateBoMonCoSo(BoMonCoSoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy danh sách tất cả cơ sở từ bảng CoSos
            var coSos = await _context.CoSos.ToListAsync();

            // Nếu chọn "Tất cả cơ sở" (IdCoSo = null hoặc Guid.Empty), tạo bản ghi cho từng cơ sở
            if (!model.IdCoSo.HasValue || model.IdCoSo == Guid.Empty)
            {
                if (!coSos.Any())
                {
                    return BadRequest(new { message = "Không có cơ sở nào trong hệ thống." });
                }

                // Tạo nhiều bản ghi cho từng cơ sở
                var createdModels = new List<BoMonCoSoViewModel>();
                foreach (var coSo in coSos)
                {
                    var boMonCoSo = new BoMonCoSo
                    {
                        IdBoMonCoSo = Guid.NewGuid(),
                        IdBoMon = model.IdBoMon,
                        IdCoSo = coSo.IdCoSo, // Gán IdCoSo thực tế
                        TrangThai = model.TrangThai,
                        NgayTao = DateTime.Now
                    };

                    _context.BoMonCoSos.Add(boMonCoSo);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        return BadRequest(new { message = $"Lỗi khi thêm: {ex.InnerException?.Message}" });
                    }

                    var createdModel = new BoMonCoSoViewModel
                    {
                        IdBoMonCoSo = boMonCoSo.IdBoMonCoSo,
                        IdBoMon = boMonCoSo.IdBoMon,
                        IdCoSo = boMonCoSo.IdCoSo,
                        TenCoSo = coSo.TenCoSo,
                        TrangThai = boMonCoSo.TrangThai,
                        NgayTao = boMonCoSo.NgayTao
                    };

                    // Gán TenBoMon
                    var boMon = await _context.QuanLyBoMons.FirstOrDefaultAsync(b => b.IDBoMon == boMonCoSo.IdBoMon);
                    createdModel.TenBoMon = boMon?.TenBoMon ?? "Không xác định";

                    createdModels.Add(createdModel);
                }

                // Trả về danh sách các bản ghi vừa tạo
                return Ok(createdModels);
            }
            else
            {
                // Nếu chọn một cơ sở cụ thể, tạo một bản ghi như hiện tại
                var boMonCoSo = new BoMonCoSo
                {
                    IdBoMonCoSo = Guid.NewGuid(),
                    IdBoMon = model.IdBoMon,
                    IdCoSo = model.IdCoSo,
                    TrangThai = model.TrangThai,
                    NgayTao = DateTime.Now
                };

                _context.BoMonCoSos.Add(boMonCoSo);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return BadRequest(new { message = $"Lỗi khi thêm: {ex.InnerException?.Message}" });
                }

                model.IdBoMonCoSo = boMonCoSo.IdBoMonCoSo;
                model.NgayTao = boMonCoSo.NgayTao;

                // Gán TenCoSo dựa trên IdCoSo
                if (boMonCoSo.IdCoSo.HasValue)
                {
                    var coSo = await _context.CoSos.FindAsync(boMonCoSo.IdCoSo);
                    model.TenCoSo = coSo?.TenCoSo ?? "Không xác định";
                }

                // Gán TenBoMon
                var boMon = await _context.QuanLyBoMons.FirstOrDefaultAsync(b => b.IDBoMon == model.IdBoMon);
                model.TenBoMon = boMon?.TenBoMon ?? "Không xác định";

                return CreatedAtAction(nameof(GetBoMonCoSo), new { id = boMonCoSo.IdBoMonCoSo }, model);
            }
        }

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

        [HttpPut("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var boMonCoSo = await _context.BoMonCoSos.FindAsync(id);
            if (boMonCoSo == null)
            {
                return NotFound();
            }

            boMonCoSo.TrangThai = !boMonCoSo.TrangThai;
            boMonCoSo.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { TrangThai = boMonCoSo.TrangThai });
        }

        [HttpGet("GetCoSos")]
        public async Task<ActionResult<IEnumerable<CoSo>>> GetCoSos()
        {
            return await _context.CoSos.ToListAsync();
        }

        [HttpGet("GetBoMons")]
        public async Task<ActionResult<IEnumerable<QuanLyBoMon>>> GetBoMons()
        {
            return await _context.QuanLyBoMons.ToListAsync();
        }
    }
}