using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhomXuongsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public NhomXuongsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/NhomXuongs/paging
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int pageSize = 5,
            string? search = null,
            int? trangThai = null)
        {
            var query = _context.NhomXuongs
                .Include(x => x.DuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.PhuTrachXuong)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(x => x.TenNhomXuong.ToLower().Contains(search));
            }

            if (trangThai != null)
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            var totalItems = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.IdNhomXuong,
                    x.TenNhomXuong,
                    TenDuAn = x.DuAn != null ? x.DuAn.TenDuAn : "Không có",
                    MaBoMon = x.QuanLyBoMon != null ? x.QuanLyBoMon.MaBoMon : "Không có",
                    TenGiangVien = x.PhuTrachXuong != null ? x.PhuTrachXuong.TenNhanVien : "Không có",
                    x.MoTa,
                    x.TrangThai,
                    x.NgayTao,
                    x.NgayCapNhat
                })
                .ToListAsync();

            return Ok(new
            {
                data,
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    totalItems,
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                }
            });
        }

        // GET: api/NhomXuongs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _context.NhomXuongs
                .Include(x => x.DuAn)
                .Include(x => x.QuanLyBoMon)
                .Include(x => x.PhuTrachXuong)
                .FirstOrDefaultAsync(x => x.IdNhomXuong == id);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        // POST: api/NhomXuongs
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] NhomXuong model)
        {
            model.IdNhomXuong = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.TrangThai = 1;
            _context.NhomXuongs.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // PUT: api/NhomXuongs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, NhomXuong model)
        {
            var exist = await _context.NhomXuongs.FindAsync(id);
            if (exist == null) return NotFound();

            exist.TenNhomXuong = model.TenNhomXuong;
            exist.IdDuAn = model.IdDuAn;
            exist.IdBoMon = model.IdBoMon;
            exist.IdPhuTrachXuong = model.IdPhuTrachXuong;
            exist.MoTa = model.MoTa;
            exist.TrangThai = model.TrangThai;
            exist.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(exist);
        }

        // PUT: api/NhomXuongs/toggle-status/{id}
        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var nx = await _context.NhomXuongs.FindAsync(id);
            if (nx == null) return NotFound();

            nx.TrangThai = nx.TrangThai == 1 ? 0 : 1;
            nx.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(nx);
        }


    }
}