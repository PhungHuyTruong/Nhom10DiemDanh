using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public AttendanceController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // Get today's schedule for logged-in student
        [HttpGet("schedule")]
        public async Task<IActionResult> GetTodaySchedule([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required");

            var today = DateTime.Today;
            var schedules = await (from lichHoc in _context.LichHocs
                                   join khnxCaHoc in _context.KHNXCaHocs on lichHoc.IdNXCH equals khnxCaHoc.IdNXCH
                                   join caHoc in _context.CaHocs on khnxCaHoc.IdCaHoc equals caHoc.IdCaHoc
                                   join nhomXuong in _context.NhomXuongs on lichHoc.IdNhomXuong equals nhomXuong.IdNhomXuong
                                   join sinhVien in _context.SinhViens on nhomXuong.IdNhomXuong equals sinhVien.IdNhomXuong
                                   join phuTrachXuong in _context.PhuTrachXuongs on nhomXuong.IdPhuTrachXuong equals phuTrachXuong.IdNhanVien
                                   where sinhVien.Email == email
                                   && khnxCaHoc.NgayHoc.Date == today
                                   && lichHoc.TrangThai == 1
                                   && khnxCaHoc.TrangThai == 0
                                   select new
                                   {
                                       ThoiGian = $"{caHoc.ThoiGianBatDau} - {caHoc.ThoiGianKetThuc}",
                                       CaHoc = caHoc.TenCaHoc,
                                       LopHoc = nhomXuong.TenNhomXuong,
                                       GiangVien = phuTrachXuong.TenNhanVien,
                                       DiemDanhTre = khnxCaHoc.DiemDanhTre,
                                       CheckInTime = caHoc.ThoiGianBatDau.HasValue ? caHoc.ThoiGianBatDau.Value.Add(TimeSpan.FromMinutes(-5)) : (TimeSpan?)null,
                                       CheckOutTime = caHoc.ThoiGianKetThuc,
                                       IdNXCH = khnxCaHoc.IdNXCH,
                                       CanCheckIn = caHoc.ThoiGianBatDau.HasValue && caHoc.ThoiGianKetThuc.HasValue
                                                   && DateTime.Now.TimeOfDay >= caHoc.ThoiGianBatDau.Value.Add(TimeSpan.FromMinutes(-5))
                                                   && DateTime.Now.TimeOfDay <= caHoc.ThoiGianKetThuc.Value,
                                       CanCheckOut = caHoc.ThoiGianKetThuc.HasValue
                                                   && DateTime.Now.TimeOfDay >= caHoc.ThoiGianKetThuc.Value
                                   }).ToListAsync();

            return Ok(schedules);
        }

        // Handle attendance check-in/check-out
        [HttpPost("check")]
        public async Task<IActionResult> CheckAttendance([FromBody] AttendanceCheckDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _context.SinhViens
                .FirstOrDefaultAsync(s => s.Email == dto.Email && s.TrangThai);

            if (student == null)
                return NotFound("Student not found");

            var khnxCaHoc = await _context.KHNXCaHocs
                .Include(k => k.CaHoc)
                .FirstOrDefaultAsync(k => k.IdNXCH == dto.IdNXCH && k.TrangThai == 0);

            if (khnxCaHoc == null || khnxCaHoc.CaHoc == null)
                return NotFound("Session not found");

            var now = DateTime.Now;
            var isLate = false;
            var status = 0;

            if (dto.IsCheckIn)
            {
                // Check-in logic
                if (!khnxCaHoc.CaHoc.ThoiGianBatDau.HasValue || !khnxCaHoc.CaHoc.ThoiGianKetThuc.HasValue)
                    return BadRequest("Invalid session time configuration");

                var checkInTime = khnxCaHoc.CaHoc.ThoiGianBatDau.Value.Add(TimeSpan.FromMinutes(-5));
                var lateTime = khnxCaHoc.CaHoc.ThoiGianBatDau.Value.Add(TimeSpan.FromMinutes(double.Parse(khnxCaHoc.DiemDanhTre ?? "0")));

                if (now.TimeOfDay < checkInTime)
                    return BadRequest("Too early to check-in");

                status = now.TimeOfDay <= lateTime ? 3 : 4; // 3: Normal check-in, 4: Late check-in
                isLate = status == 4;
            }
            else
            {
                // Check-out logic
                if (!khnxCaHoc.CaHoc.ThoiGianKetThuc.HasValue)
                    return BadRequest("Invalid session end time");

                var checkOutTime = khnxCaHoc.CaHoc.ThoiGianKetThuc.Value;
                var lateTime = khnxCaHoc.CaHoc.ThoiGianKetThuc.Value.Add(TimeSpan.FromMinutes(double.Parse(khnxCaHoc.DiemDanhTre ?? "0")));

                if (now.TimeOfDay < checkOutTime)
                    return BadRequest("Too early to check-out");

                status = now.TimeOfDay <= lateTime ? 5 : 6; // 5: Normal check-out, 6: Late check-out
                isLate = status == 6;
            }

            //// Save attendance record
            //var diemDanh = await _context.DiemDanhs
            //    .FirstOrDefaultAsync(d => d.IdSinhVien == student.IdSinhVien
            //        && d.IdCaHoc == khnxCaHoc.IdCaHoc
            //        && d.IdNhomXuong == (
            //            from lichHoc in _context.LichHocs
            //            where lichHoc.IdNXCH == khnxCaHoc.IdNXCH
            //            select lichHoc.IdNhomXuong).FirstOrDefault());

            //if (diemDanh == null)
            //{
            //    var nhomXuongId = await (from lichHoc in _context.LichHocs
            //                             where lichHoc.IdNXCH == khnxCaHoc.IdNXCH
            //                             select lichHoc.IdNhomXuong).FirstOrDefaultAsync();

            //    if (nhomXuongId == Guid.Empty)
            //        return BadRequest("Invalid group assignment");

            //    diemDanh = new DiemDanh
            //    {
            //        IdSinhVien = student.IdSinhVien,
            //        IdCaHoc = (Guid)khnxCaHoc.IdCaHoc,
            //        IdNhomXuong = nhomXuongId,
            //        IdNhanVien = await (from nx in _context.NhomXuongs
            //                            where nx.IdNhomXuong == nhomXuongId
            //                            select nx.IdPhuTrachXuong).FirstOrDefaultAsync()
            //    };
            //    _context.DiemDanhs.Add(diemDanh);
            //}

            var lichSuDiemDanh = new LichSuDiemDanh
            {
                //IdDiemDanh = diemDanh.IdDiemDanh,
                //thêm trường này
                IdSinhVien = student.IdSinhVien, // Gán đúng trường mới

                IdNXCH = khnxCaHoc.IdNXCH,
                ThoiGianDiemDanh = now,
                NoiDungBuoiHoc = "Buổi học thực hành", // Thêm nội dung buổi học
                HinhThuc = "Online",
                DiaDiem = "Phòng Lab 1", // Thêm địa điểm
                GhiChu = isLate ? "Late" : "On-time",
                TrangThai = status,
                NgayTao = now,
                TrangThaiDuyet = 1 // Giả định trạng thái duyệt mặc định
            };

            _context.LichSuDiemDanhs.Add(lichSuDiemDanh);
            await _context.SaveChangesAsync();

            return Ok(new { Status = status, Message = "Attendance recorded" });
        }
    }

    public class AttendanceCheckDto
    {
        public Guid IdNXCH { get; set; }
        public string Email { get; set; }
        // Removed IPAddress since IP check is no longer needed
        public bool IsCheckIn { get; set; }
    }
}