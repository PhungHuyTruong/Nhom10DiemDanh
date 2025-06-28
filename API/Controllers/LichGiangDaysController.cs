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
    public class LichGiangDaysController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public LichGiangDaysController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/LichGiangDays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichGiangDayDTO>>> GetLichGiangDays()
        {
            var list = await _context.LichGiangDays
                .Include(l => l.KHNXCaHoc)
                    .ThenInclude(k => k.CaHoc)
                .Include(l => l.NhomXuong)
                .Include(l => l.DuAn)
                .Include(l => l.DiaDiem)
                .Select(l => new LichGiangDayDTO
                {
                    IdLichDay = l.IdLichDay,
                    IdNXCH = l.IdNXCH,
                    IdNhomXuong = l.IdNhomXuong,
                    IdDuAn = l.IdDuAn,
                    IdDiaDiem = l.IdDiaDiem,
                    HTGiangDay = l.HTGiangDay,
                    TTDiemDanh = l.TTDiemDanh,
                    TTDiemDanhMuon = l.TTDiemDanhMuon,
                    NgayTao = l.NgayTao,
                    NgayCapNhat = l.NgayCapNhat,
                    TrangThai = l.TrangThai,
                    TenNhomXuong = l.NhomXuong.TenNhomXuong,
                    TenDiaDiem = l.DiaDiem.TenDiaDiem,
                    TenDuAn = l.DuAn != null ? l.DuAn.TenDuAn : null,
                    TenCaHoc = l.KHNXCaHoc.CaHoc != null ? l.KHNXCaHoc.CaHoc.TenCaHoc : null
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/LichGiangDays/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LichGiangDayDTO>> GetLichGiangDay(Guid id)
        {
            var l = await _context.LichGiangDays
                .Include(x => x.KHNXCaHoc)
                    .ThenInclude(k => k.CaHoc)
                .Include(x => x.NhomXuong)
                .Include(x => x.DuAn)
                .Include(x => x.DiaDiem)
                .Where(x => x.IdLichDay == id)
                .Select(l => new LichGiangDayDTO
                {
                    IdLichDay = l.IdLichDay,
                    IdNXCH = l.IdNXCH,
                    IdNhomXuong = l.IdNhomXuong,
                    IdDuAn = l.IdDuAn,
                    IdDiaDiem = l.IdDiaDiem,
                    HTGiangDay = l.HTGiangDay,
                    TTDiemDanh = l.TTDiemDanh,
                    TTDiemDanhMuon = l.TTDiemDanhMuon,
                    NgayTao = l.NgayTao,
                    NgayCapNhat = l.NgayCapNhat,
                    TrangThai = l.TrangThai,
                    TenNhomXuong = l.NhomXuong.TenNhomXuong,
                    TenDiaDiem = l.DiaDiem.TenDiaDiem,
                    TenDuAn = l.DuAn != null ? l.DuAn.TenDuAn : null,
                    TenCaHoc = l.KHNXCaHoc.CaHoc != null ? l.KHNXCaHoc.CaHoc.TenCaHoc : null
                })
                .FirstOrDefaultAsync();

            if (l == null) return NotFound();
            return Ok(l);
        }

        // POST: api/LichGiangDays
        [HttpPost]
        public async Task<ActionResult> PostLichGiangDay(LichGiangDay lich)
        {
            lich.IdLichDay = Guid.NewGuid();
            lich.NgayTao = DateTime.Now;

            _context.LichGiangDays.Add(lich);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo lịch giảng dạy thành công", id = lich.IdLichDay });
        }

        // PUT: api/LichGiangDays/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLichGiangDay(Guid id, LichGiangDay lich)
        {
            if (id != lich.IdLichDay) return BadRequest();

            var existing = await _context.LichGiangDays.FindAsync(id);
            if (existing == null) return NotFound();

            existing.IdNXCH = lich.IdNXCH;
            existing.IdNhomXuong = lich.IdNhomXuong;
            existing.IdDuAn = lich.IdDuAn;
            existing.IdDiaDiem = lich.IdDiaDiem;
            existing.HTGiangDay = lich.HTGiangDay;
            existing.TTDiemDanh = lich.TTDiemDanh;
            existing.TTDiemDanhMuon = lich.TTDiemDanhMuon;
            existing.TrangThai = lich.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật lịch giảng dạy thành công" });
        }

        // DELETE: api/LichGiangDays/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLichGiangDay(Guid id)
        {
            var lich = await _context.LichGiangDays.FindAsync(id);
            if (lich == null) return NotFound();

            _context.LichGiangDays.Remove(lich);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
    }
}
