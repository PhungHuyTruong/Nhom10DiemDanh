using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhomXuongCuaToiController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public NhomXuongCuaToiController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNhomXuongCuaToi([FromQuery] string? email)
        {
            var claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            email = string.IsNullOrEmpty(claimEmail) ? email : claimEmail;

            Console.WriteLine($"📧 Email API nhận được: {email}");

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("⚠ Không có email → Trả về tất cả nhóm xưởng");
                var all = await _context.NhomXuongs
                    .Include(n => n.DuAn)
                    .Include(n => n.QuanLyBoMon)
                    .ToListAsync();
                return Ok(all);
            }

            var giangVien = await _context.VaiTroNhanViens
                .Include(v => v.PhuTrachXuong)
              
                .FirstOrDefaultAsync(v =>
                    (v.PhuTrachXuong.EmailFE == email || v.PhuTrachXuong.EmailFPT == email) &&
                    v.TrangThai 
                  );

            if (giangVien == null)
            {
                Console.WriteLine($"❌ Không phải giảng viên: {email} → Trả tất cả");
                var all = await _context.NhomXuongs
                    .Include(n => n.DuAn)
                    .Include(n => n.QuanLyBoMon)
                    .ToListAsync();
                return Ok(all);
            }

            var idPhuTrach = giangVien.PhuTrachXuong.IdNhanVien;
            Console.WriteLine($"✅ Giảng viên {email}, ID phụ trách: {idPhuTrach}");

            var nhomXuongs = await _context.NhomXuongs
                .Include(n => n.DuAn)
                .Include(n => n.QuanLyBoMon)
                .Where(n => n.IdPhuTrachXuong == idPhuTrach)
                .ToListAsync();

            return Ok(nhomXuongs);
        }

        //[HttpPost("doi-trang-thai")]
        //public async Task<IActionResult> DoiTrangThai([FromBody] Guid idSinhVien)
        //{
        //    Console.WriteLine($"👉 Nhận yêu cầu đổi trạng thái sinh viên: {idSinhVien}");

        //    var sv = await _context.SinhViens.FindAsync(idSinhVien);
        //    if (sv == null)
        //    {
        //        Console.WriteLine("❌ Không tìm thấy sinh viên");
        //        return NotFound();
        //    }

        //    sv.TrangThai = !sv.TrangThai;
        //    sv.NgayCapNhat = DateTime.Now;
        //    await _context.SaveChangesAsync();

        //    Console.WriteLine($"✅ Trạng thái mới: {sv.TrangThai}");
        //    return Ok(new { success = true, newStatus = sv.TrangThai });
        //}

    }
}
