using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using System.ComponentModel.DataAnnotations;

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
            Console.WriteLine($"Fetching schedules for email: {email}, date: {today:yyyy-MM-dd HH:mm:ss.fff}");

            var schedules = await (from khnxCaHoc in _context.KHNXCaHocs
                                   join caHoc in _context.CaHocs on khnxCaHoc.IdCaHoc equals caHoc.IdCaHoc
                                   join keHoachNhomXuong in _context.KeHoachNhomXuongs on khnxCaHoc.IdKHNX equals keHoachNhomXuong.IdKHNX
                                   join nhomXuong in _context.NhomXuongs on keHoachNhomXuong.IdNhomXuong equals nhomXuong.IdNhomXuong
                                   join sinhVien in _context.SinhViens on nhomXuong.IdNhomXuong equals sinhVien.IdNhomXuong
                                   join phuTrachXuong in _context.PhuTrachXuongs on nhomXuong.IdPhuTrachXuong equals phuTrachXuong.IdNhanVien
                                   where sinhVien.Email == email
                                   && khnxCaHoc.NgayHoc.Date == today
                                   && khnxCaHoc.TrangThai == 1
                                   let hasCheckedIn = _context.LichSuDiemDanhs
                                       .Any(ls => ls.IdNXCH == khnxCaHoc.IdNXCH
                                               && ls.IdDiemDanh == (_context.DiemDanhs
                                                   .Where(d => d.IdSinhVien == sinhVien.IdSinhVien
                                                           && d.IdCaHoc == khnxCaHoc.IdCaHoc)
                                                   .Select(d => d.IdDiemDanh)
                                                   .FirstOrDefault())
                                               && ls.ThoiGianDiemDanh.Date == today
                                               && ls.TrangThai >= 3 && ls.TrangThai <= 4)
                                   let hasCheckedOut = _context.LichSuDiemDanhs
                                       .Any(ls => ls.IdNXCH == khnxCaHoc.IdNXCH
                                               && ls.IdDiemDanh == (_context.DiemDanhs
                                                   .Where(d => d.IdSinhVien == sinhVien.IdSinhVien
                                                           && d.IdCaHoc == khnxCaHoc.IdCaHoc)
                                                   .Select(d => d.IdDiemDanh)
                                                   .FirstOrDefault())
                                               && ls.ThoiGianDiemDanh.Date == today
                                               && ls.TrangThai >= 5 && ls.TrangThai <= 6)
                                   let checkInTime = caHoc.ThoiGianBatDau.HasValue ? caHoc.ThoiGianBatDau.Value.Add(TimeSpan.FromMinutes(-5)) : (TimeSpan?)null
                                   let lateCheckInTime = caHoc.ThoiGianBatDau.HasValue ? caHoc.ThoiGianBatDau.Value.Add(TimeSpan.FromMinutes(double.Parse(khnxCaHoc.DiemDanhTre ?? "0"))) : (TimeSpan?)null
                                   let isBeforeCheckIn = caHoc.ThoiGianBatDau.HasValue && DateTime.Now.TimeOfDay < checkInTime
                                   let isAfterCheckIn = caHoc.ThoiGianBatDau.HasValue && DateTime.Now.TimeOfDay > lateCheckInTime
                                   select new ScheduleViewModel
                                   {
                                       ThoiGian = $"{caHoc.ThoiGianBatDau} - {caHoc.ThoiGianKetThuc}",
                                       CaHoc = caHoc.TenCaHoc,
                                       LopHoc = nhomXuong.TenNhomXuong,
                                       GiangVien = phuTrachXuong.TenNhanVien,
                                       DiemDanhTre = khnxCaHoc.DiemDanhTre,
                                       CheckInTime = checkInTime,
                                       CheckOutTime = caHoc.ThoiGianKetThuc,
                                       IdNXCH = khnxCaHoc.IdNXCH,
                                       CanCheckIn = caHoc.ThoiGianBatDau.HasValue && caHoc.ThoiGianKetThuc.HasValue
                                                   && DateTime.Now.TimeOfDay >= checkInTime
                                                   && DateTime.Now.TimeOfDay <= lateCheckInTime
                                                   && !hasCheckedIn,
                                       CanCheckOut = caHoc.ThoiGianKetThuc.HasValue
                                                   && DateTime.Now.TimeOfDay >= caHoc.ThoiGianKetThuc.Value
                                                   && !hasCheckedOut,
                                       Status = hasCheckedOut ? "Đã check-out" :
                                                hasCheckedIn ? "Đã check-in" :
                                                isBeforeCheckIn ? "Chưa đến thời gian check-in" :
                                                isAfterCheckIn ? "Đã quá thời gian check-in" :
                                                "Chưa điểm danh",
                                       HasCheckedIn = hasCheckedIn,
                                       HasCheckedOut = hasCheckedOut
                                   }).ToListAsync();

            Console.WriteLine($"Found {schedules.Count} schedules for email: {email}");
            foreach (var schedule in schedules)
            {
                Console.WriteLine($"Schedule: IdNXCH={schedule.IdNXCH}, CanCheckIn={schedule.CanCheckIn}, CanCheckOut={schedule.CanCheckOut}, Status={schedule.Status}, HasCheckedIn={schedule.HasCheckedIn}, HasCheckedOut={schedule.HasCheckedOut}");
            }

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
                .FirstOrDefaultAsync(k => k.IdNXCH == dto.IdNXCH && k.TrangThai == 1);

            if (khnxCaHoc == null || khnxCaHoc.CaHoc == null)
                return NotFound("Session not found");

            // Get location information for this session
            var locationInfo = await (from khnx in _context.KHNXCaHocs
                                      join keHoachNhomXuong in _context.KeHoachNhomXuongs on khnx.IdKHNX equals keHoachNhomXuong.IdKHNX
                                      join nhomXuong in _context.NhomXuongs on keHoachNhomXuong.IdNhomXuong equals nhomXuong.IdNhomXuong
                                      join phuTrachXuong in _context.PhuTrachXuongs on nhomXuong.IdPhuTrachXuong equals phuTrachXuong.IdNhanVien
                                      join coSoEntity in _context.CoSos on phuTrachXuong.IdCoSo equals coSoEntity.IdCoSo
                                      join diaDiemEntity in _context.DiaDiems on coSoEntity.IdCoSo equals diaDiemEntity.IdCoSo
                                      where khnx.IdNXCH == dto.IdNXCH && diaDiemEntity.TrangThai == true
                                      select new { diaDiem = diaDiemEntity, coSo = coSoEntity }).FirstOrDefaultAsync();

            if (locationInfo?.diaDiem == null)
                return BadRequest("Location information not found for this session");

            var diaDiem = locationInfo.diaDiem;
            var coSo = locationInfo.coSo;

            // Validate IP address (only if provided)
            if (!string.IsNullOrEmpty(dto.IPAddress))
            {
                var validIPs = await _context.IPs
                    .Where(ip => ip.IdCoSo == coSo.IdCoSo && ip.TrangThai)
                    .Select(ip => ip.IP_DaiIP)
                    .ToListAsync();

                if (!validIPs.Any())
                    return BadRequest("No valid IP addresses configured for this location");

                if (!validIPs.Contains(dto.IPAddress))
                    return BadRequest($"IP address {dto.IPAddress} is not authorized for this location. Valid IPs: {string.Join(", ", validIPs)}");
            }

            // Validate location (coordinates) - only if provided
            if (dto.Latitude.HasValue && dto.Longitude.HasValue)
            {
                if (!diaDiem.ViDo.HasValue || !diaDiem.KinhDo.HasValue || !diaDiem.BanKinh.HasValue)
                    return BadRequest("Location coordinates or radius not configured for this venue");

                var distance = CalculateDistance(dto.Latitude.Value, dto.Longitude.Value, diaDiem.ViDo.Value, diaDiem.KinhDo.Value);

                if (distance > diaDiem.BanKinh.Value)
                    return BadRequest($"You are too far from the venue. Distance: {distance:F2}m, Allowed radius: {diaDiem.BanKinh:F2}m");
            }

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

                if (now.TimeOfDay > lateTime)
                    return BadRequest("Too late to check-in");

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

            // Get IdNhomXuong from KHNXCaHocs and KeHoachNhomXuongs
            var nhomXuongId = await (from khnx in _context.KHNXCaHocs
                                     join keHoachNhomXuong in _context.KeHoachNhomXuongs on khnx.IdKHNX equals keHoachNhomXuong.IdKHNX
                                     where khnx.IdNXCH == dto.IdNXCH
                                     select keHoachNhomXuong.IdNhomXuong).FirstOrDefaultAsync();

            if (nhomXuongId == Guid.Empty)
                return BadRequest("Invalid group assignment");

            // Save attendance record
            var diemDanh = await _context.DiemDanhs
                .FirstOrDefaultAsync(d => d.IdSinhVien == student.IdSinhVien
                    && d.IdCaHoc == khnxCaHoc.IdCaHoc
                    && d.IdNhomXuong == nhomXuongId);

            if (diemDanh == null)
            {
                diemDanh = new DiemDanh
                {
                    IdSinhVien = student.IdSinhVien,
                    IdCaHoc = (Guid)khnxCaHoc.IdCaHoc,
                    IdNhomXuong = nhomXuongId,
                    IdNhanVien = await (from nx in _context.NhomXuongs
                                        where nx.IdNhomXuong == nhomXuongId
                                        select nx.IdPhuTrachXuong).FirstOrDefaultAsync()
                };
                _context.DiemDanhs.Add(diemDanh);
            }

            var lichSuDiemDanh = new LichSuDiemDanh
            {
                IdDiemDanh = diemDanh.IdDiemDanh,
                IdNXCH = khnxCaHoc.IdNXCH,
                ThoiGianDiemDanh = now,
                NoiDungBuoiHoc = "Buổi học thực hành",
                HinhThuc = "Online",
                DiaDiem = diaDiem.TenDiaDiem,
                GhiChu = isLate ? "Late" : "On-time",
                TrangThai = status,
                NgayTao = now,
                TrangThaiDuyet = 1
            };

            _context.LichSuDiemDanhs.Add(lichSuDiemDanh);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Saved LichSuDiemDanh: IdNXCH={khnxCaHoc.IdNXCH}, IdDiemDanh={diemDanh.IdDiemDanh}, Status={status}, Time={now:yyyy-MM-dd HH:mm:ss.fff}");

            return Ok(new { Status = status, Message = "Attendance recorded successfully" });
        }

        /// <summary>
        /// Calculate distance between two points using Haversine formula
        /// </summary>
        /// <param name="lat1">Latitude of point1</param>
        /// <param name="lon1">Longitude of point1</param>
        /// <param name="lat2">Latitude of point2</param>
        /// <param name="lon2">Longitude of point2</param>
        /// <returns>Distance in meters</returns>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth's radius in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
    public class ScheduleViewModel
    {
        public string ThoiGian { get; set; }
        public string CaHoc { get; set; }
        public string LopHoc { get; set; }
        public string GiangVien { get; set; }
        public string DiemDanhTre { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public Guid IdNXCH { get; set; }
        public bool CanCheckIn { get; set; }
        public bool CanCheckOut { get; set; }
        public string Status { get; set; }
        public bool HasCheckedIn { get; set; }
        public bool HasCheckedOut { get; set; }
    }
}