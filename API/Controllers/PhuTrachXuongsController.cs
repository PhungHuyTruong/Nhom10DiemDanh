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
                .Select(x => new API.Models.PhuTrachXuongDto
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
                    TenVaiTro = x.VaiTro.TenVaiTro,
                    NgayCapNhat = x.NgayCapNhat,
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
        public async Task<IActionResult> Create([FromForm] PhuTrachXuong model)
        {
            if (model == null)
                return BadRequest();

            // Gán lại các giá trị cần khởi tạo
            model.IdNhanVien = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.TrangThai = true;
            model.IdVaiTro = model.IdVaiTro;

            _context.PhuTrachXuongs.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "PhuTrachXuongs");
        }



        [HttpPut("{id}")] // hoặc [HttpPut] nếu muốn
        public async Task<IActionResult> Update(Guid id, [FromBody] PhuTrachXuong model)
        {

            var pt = await _context.PhuTrachXuongs.FindAsync(id);
            if (pt == null) return NotFound();

            pt.TenNhanVien = model.TenNhanVien;
            pt.MaNhanVien = model.MaNhanVien;
            pt.EmailFE = model.EmailFE;
            pt.EmailFPT = model.EmailFPT;
            pt.IdCoSo = model.IdCoSo;
            pt.IdNhanVien = model.IdNhanVien != Guid.Empty ? model.IdNhanVien : pt.IdNhanVien;
            pt.NgayCapNhat = DateTime.Now;
            pt.TrangThai = model.TrangThai;
            pt.IdVaiTro = model.IdVaiTro;

            await _context.SaveChangesAsync();

            return NoContent();
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