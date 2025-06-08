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
    public class DuAnsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public DuAnsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/DuAns
        [HttpGet]
        public async Task<IActionResult> GetDuAns(string? search, string? capDo, string? hocKy, string? monHoc, string? trangThai)
        {
            var query = _context.DuAns
                .Include(d => d.QuanLyBoMon)
                .Include(d => d.HocKy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.TenDuAn.Contains(search));

            if (!string.IsNullOrWhiteSpace(capDo))
                query = query.Where(d => d.CapDoDuAn.MaCapDoDuAn == capDo);

            if (!string.IsNullOrWhiteSpace(hocKy))
                query = query.Where(d => d.HocKy.TenHocKy == hocKy);

            if (!string.IsNullOrWhiteSpace(monHoc))
                query = query.Where(d => d.QuanLyBoMon.TenBoMon == monHoc);

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                if (bool.TryParse(trangThai, out bool status))
                    query = query.Where(d => d.TrangThai == status);
            }

            var result = await query.ToListAsync();

            return Ok(new
            {
                success = true,
                message = $"Tìm thấy {result.Count} dự án.",
                data = result
            });
        }

        // GET: api/DuAns/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDuAn(Guid id)
        {
            var duAn = await _context.DuAns
                .Include(d => d.QuanLyBoMon)
                .Include(d => d.HocKy)
                .FirstOrDefaultAsync(x => x.IdDuAn == id);

            if (duAn == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Không tìm thấy dự án với ID {id}"
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Tìm thấy dự án",
                data = duAn
            });
        }

        // POST: api/DuAns
        [HttpPost]
        public async Task<IActionResult> PostDuAn([FromBody] DuAn duAn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            duAn.IdDuAn = Guid.NewGuid();
            duAn.NgayTao = DateTime.Now;

            _context.DuAns.Add(duAn);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDuAn), new { id = duAn.IdDuAn }, new
            {
                success = true,
                message = "Thêm dự án thành công",
                data = duAn
            });
        }

        // PUT: api/DuAns?id=xxx
        [HttpPut]
        public async Task<IActionResult> PutDuAn([FromQuery] Guid id, [FromBody] DuAn duAn)
        {
            if (id != duAn.IdDuAn)
                return BadRequest(new { success = false, message = "ID không khớp" });

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var existing = await _context.DuAns.FindAsync(id);
            if (existing == null)
                return NotFound(new { success = false, message = "Không tìm thấy dự án để cập nhật" });

            existing.TenDuAn = duAn.TenDuAn;
            existing.CapDoDuAn = duAn.CapDoDuAn;
            existing.MoTa = duAn.MoTa;
            existing.IdBoMon = duAn.IdBoMon;
            existing.IdCDDA = duAn.IdCDDA;
            existing.IdHocKy = duAn.IdHocKy;
            existing.TrangThai = duAn.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cập nhật dự án thành công",
                data = existing
            });
        }

        // DELETE: api/DuAns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDuAn(Guid id)
        {
            var duAn = await _context.DuAns.FindAsync(id);
            if (duAn == null)
                return NotFound(new { success = false, message = "Không tìm thấy dự án để xoá" });

            _context.DuAns.Remove(duAn);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Xóa dự án thành công",
                data = duAn
            });
        }

        // PATCH: api/DuAns/{id}/ToggleStatus
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var duAn = await _context.DuAns.FindAsync(id);
            if (duAn == null)
                return NotFound(new { success = false, message = "Không tìm thấy dự án để cập nhật trạng thái" });

            duAn.TrangThai = !duAn.TrangThai;
            duAn.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cập nhật trạng thái thành công",
                data = duAn
            });
        }
    }
}
