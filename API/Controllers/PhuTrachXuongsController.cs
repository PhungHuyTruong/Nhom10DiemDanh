using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DtoApi = API.Models.PhuTrachXuongDto;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhuTrachXuongsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public PhuTrachXuongsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.PhuTrachXuongs
                .Include(x => x.VaiTroNhanViens)
                    .ThenInclude(vtnv => vtnv.VaiTro)
                .Include(x => x.CoSo)
                .Select(x => new PhuTrachXuongDto
                {
                    IdNhanVien = x.IdNhanVien,
                    MaNhanVien = x.MaNhanVien,
                    TenNhanVien = x.TenNhanVien,
                    EmailFE = x.EmailFE,
                    EmailFPT = x.EmailFPT,
                    IdCoSo = x.IdCoSo,
                    TenCoSo = x.CoSo.TenCoSo,
                    TrangThai = x.TrangThai,
                    NgayTao = x.NgayTao,
                    NgayCapNhat = x.NgayCapNhat,

                    // ✅ Lấy danh sách tên vai trò
                    TenVaiTros = x.VaiTroNhanViens
                        .Where(v => v.TrangThai == true) // nếu có trạng thái
                        .Select(v => v.VaiTro.TenVaiTro)
                        .ToList()
                })
                .ToListAsync();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pt = await _context.PhuTrachXuongs.Include(x => x.CoSo).FirstOrDefaultAsync(x => x.IdNhanVien == id);
            return pt == null ? NotFound() : Ok(pt);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PhuTrachXuongDto model)
        {
            if (model == null) return BadRequest();

            var nv = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = model.TenNhanVien,
                MaNhanVien = model.MaNhanVien,
                EmailFE = model.EmailFE,
                EmailFPT = model.EmailFPT,
                IdCoSo = model.IdCoSo,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            await _context.PhuTrachXuongs.AddAsync(nv);

            // Gán danh sách vai trò
            foreach (var idVaiTro in model.IdVaiTros)
            {
                _context.VaiTroNhanViens.Add(new VaiTroNhanVien
                {
                    IdVTNV = Guid.NewGuid(),
                    IdNhanVien = nv.IdNhanVien,
                    IdVaiTro = idVaiTro,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "PhuTrachXuongs");
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PhuTrachXuongDto model)
        {
            var pt = await _context.PhuTrachXuongs
                .Include(x => x.VaiTroNhanViens)
                .FirstOrDefaultAsync(x => x.IdNhanVien == id);

            if (pt == null) return NotFound();

            pt.TenNhanVien = model.TenNhanVien;
            pt.MaNhanVien = model.MaNhanVien;
            pt.EmailFE = model.EmailFE;
            pt.EmailFPT = model.EmailFPT;
            pt.IdCoSo = model.IdCoSo;
            pt.NgayCapNhat = DateTime.Now;
            pt.TrangThai = model.TrangThai;

            // Xóa các vai trò cũ
            _context.VaiTroNhanViens.RemoveRange(pt.VaiTroNhanViens);

            // Thêm lại danh sách vai trò mới
            if (model.IdVaiTros != null)
            {
                foreach (var idVaiTro in model.IdVaiTros)
                {
                    _context.VaiTroNhanViens.Add(new VaiTroNhanVien
                    {
                        IdVTNV = Guid.NewGuid(),
                        IdNhanVien = pt.IdNhanVien,
                        IdVaiTro = idVaiTro,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(); // ✅ Không dùng Redirect trong API
        }



        [HttpPost("doi-trang-thai/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var pt = await _context.PhuTrachXuongs.FindAsync(id);
            if (pt == null) return NotFound();

            pt.TrangThai = !pt.TrangThai;
            pt.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // API lấy danh sách Cơ Sở
        [HttpGet("cosos")]
        public async Task<IActionResult> GetCoSoList()
        {
            var csList = await _context.CoSos.Select(c => new { c.IdCoSo, c.TenCoSo }).ToListAsync();
            return Ok(csList);
        }

        [HttpGet("vaitros")]
        public async Task<IActionResult> GetVaiTroList()
        {
            var list = await _context.VaiTros
                .Select(v => new { v.IdVaiTro, v.TenVaiTro })
                .ToListAsync();
            return Ok(list);
        }
    }

}