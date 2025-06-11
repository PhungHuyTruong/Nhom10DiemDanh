using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        // Lấy danh sách cơ sở
        [HttpGet("cosos")]
        public async Task<IActionResult> GetCoSos()
        {
            var coSos = await _context.CoSos
                .Select(cs => new CoSoViewModel
                {
                    IdCoSo = cs.IdCoSo,
                    TenCoSo = cs.TenCoSo,
                    MaCoSo = cs.MaCoSo,
                    DiaChi = cs.DiaChi,
                    SDT = cs.SDT,
                    Email = cs.Email,
                    TrangThai = cs.TrangThai ? "Hoạt động" : "Tắt",
                    IdDiaDiem = cs.IdDiaDiem,
                    IdCaHoc = cs.IdCaHoc,
                    IPs = cs.IPs.Select(ip => new IPViewModel
                    {
                        IdIP = ip.IdIP,
                        KieuIP = ip.KieuIP,
                        IP_DaiIP = ip.IP_DaiIP,
                        NgayTao = ip.NgayTao,
                        NgayCapNhat = ip.NgayCapNhat,
                        TrangThai = ip.TrangThai,
                        IdCoSo = ip.IdCoSo
                    }).ToList()
                })
                .ToListAsync();

            return Ok(coSos);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.IPs
                .Select(ip => new IPDto
                {
                    IdIP = ip.IdIP,
                    KieuIP = ip.KieuIP,
                    IP_DaiIP = ip.IP_DaiIP,
                    NgayTao = ip.NgayTao,
                    NgayCapNhat = ip.NgayCapNhat,
                    TrangThai = ip.TrangThai,
                    IdCoSo = ip.IdCoSo
                }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null) return NotFound();

            var dto = new IPDto
            {
                IdIP = ip.IdIP,
                KieuIP = ip.KieuIP,
                IP_DaiIP = ip.IP_DaiIP,
                NgayTao = ip.NgayTao,
                NgayCapNhat = ip.NgayCapNhat,
                TrangThai = ip.TrangThai,
                IdCoSo = ip.IdCoSo
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IPDto dto)
        {
            if (!await _context.CoSos.AnyAsync(cs => cs.IdCoSo == dto.IdCoSo))
            {
                ModelState.AddModelError("IdCoSo", "Cơ sở không tồn tại.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                KieuIP = dto.KieuIP,
                IP_DaiIP = dto.IP_DaiIP,
                NgayTao = DateTime.Now,
                TrangThai = dto.TrangThai,
                IdCoSo = dto.IdCoSo
            };

            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            dto.IdIP = ip.IdIP;
            dto.NgayTao = ip.NgayTao;

            return CreatedAtAction(nameof(GetById), new { id = dto.IdIP }, dto);
        }

        [HttpPost("{idCoSo}")]
        public async Task<IActionResult> CreateWithCoSo(Guid idCoSo, IPDto dto)
        {
            if (!await _context.CoSos.AnyAsync(cs => cs.IdCoSo == idCoSo))
            {
                ModelState.AddModelError("IdCoSo", "Cơ sở không tồn tại.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                KieuIP = dto.KieuIP,
                IP_DaiIP = dto.IP_DaiIP,
                NgayTao = DateTime.Now,
                TrangThai = dto.TrangThai,
                IdCoSo = idCoSo
            };

            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            dto.IdIP = ip.IdIP;
            dto.NgayTao = ip.NgayTao;
            dto.IdCoSo = ip.IdCoSo;

            return CreatedAtAction(nameof(GetById), new { id = dto.IdIP }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, IPDto dto)
        {
            if (!await _context.CoSos.AnyAsync(cs => cs.IdCoSo == dto.IdCoSo))
            {
                ModelState.AddModelError("IdCoSo", "Cơ sở không tồn tại.");
                return BadRequest(ModelState);
            }

            var ip = await _context.IPs.FindAsync(id);
            if (ip == null) return NotFound();

            ip.KieuIP = dto.KieuIP;
            ip.IP_DaiIP = dto.IP_DaiIP;
            ip.NgayCapNhat = DateTime.Now;
            ip.TrangThai = dto.TrangThai;
            ip.IdCoSo = dto.IdCoSo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null) return NotFound();

            _context.IPs.Remove(ip);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}