using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestSinhVien
{
    [TestFixture]
    public class DiemDanhControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private AttendanceController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new AttendanceController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #region Helper tạo dữ liệu đầy đủ

        private SinhVien SeedSinhVien(string email = "student@fpt.edu.vn")
        {
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                Email = email,
                TrangThai = true,
                MaSinhVien = "SV001",
                TenSinhVien = "Nguyen Van A"
            };
            _context.SinhViens.Add(sv);
            return sv;
        }

        private CoSo SeedCoSo()
        {
            var coSo = new CoSo
            {
                IdCoSo = Guid.NewGuid(),
                TenCoSo = "Cơ sở FPT",
                DiaChi = "123 FPT Street",
                Email = "coso@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0123456789"
            };
            _context.CoSos.Add(coSo);
            return coSo;
        }

        private DiaDiem SeedDiaDiem(Guid idCoSo)
        {
            var dd = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = idCoSo,
                TrangThai = true,
                TenDiaDiem = "A1",
                ViDo = 10.123,
                KinhDo = 106.123,
                BanKinh = 200
            };
            _context.DiaDiems.Add(dd);
            return dd;
        }

        private CaHoc SeedCaHoc(TimeSpan? batDau = null, TimeSpan? ketThuc = null)
        {
            var ca = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca1",
                ThoiGianBatDau = batDau ?? DateTime.Now.TimeOfDay,
                ThoiGianKetThuc = ketThuc ?? DateTime.Now.AddHours(2).TimeOfDay
            };
            _context.CaHocs.Add(ca);
            return ca;
        }

        private KHNXCaHoc SeedKHNXCaHoc(Guid? idKHNX = null, Guid? idCaHoc = null)
        {
            var kh = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = idKHNX.HasValue ? idKHNX.Value : Guid.NewGuid(),
                IdCaHoc = idCaHoc.HasValue ? idCaHoc.Value : Guid.NewGuid(),
                TrangThai = 1,
                Buoi = "Buổi 1",
                DiemDanhTre = "15",
                LinkOnline = "https://zoom.us/test",
                NoiDung = "Bài học ASP.NET Core",
                ThoiGian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")  // ✅ FIX kiểu string
            };
            _context.KHNXCaHocs.Add(kh);
            return kh;
        }


        private void SaveAll()
        {
            _context.SaveChanges();
        }

        #endregion

        // ✅ 1. Thiếu email => BadRequest
        [Test]
        public async Task GetTodaySchedule_MissingEmail_ReturnsBadRequest()
        {
            var result = await _controller.GetTodaySchedule(null);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // ✅ 2. Không có lịch => trả về list rỗng
        [Test]
        public async Task GetTodaySchedule_NoSchedule_ReturnsEmptyList()
        {
            var result = await _controller.GetTodaySchedule("test@student.com") as OkObjectResult;
            Assert.IsNotNull(result);
            var schedules = result.Value as List<API.Controllers.ScheduleViewModel>;
            Assert.IsNotNull(schedules);
            Assert.AreEqual(0, schedules.Count);
        }

        // ✅ 3. ModelState invalid
        [Test]
        public async Task CheckAttendance_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Email", "Required");
            var dto = new AttendanceCheckDto();
            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // ✅ 4. Không tìm thấy sinh viên
        [Test]
        public async Task CheckAttendance_StudentNotFound_ReturnsNotFound()
        {
            var dto = new AttendanceCheckDto
            {
                Email = "notfound@student.com",
                IdNXCH = Guid.NewGuid(),
                IsCheckIn = true
            };
            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        // ✅ 5. Không có location hợp lệ
        [Test]
        public async Task CheckAttendance_NoLocation_ReturnsBadRequest()
        {
            var sv = SeedSinhVien();
            var kh = SeedKHNXCaHoc(); // không gán DiaDiem
            SaveAll();

            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // ✅ 6. Không có IP hợp lệ
        [Test]
        public async Task CheckAttendance_NoValidIP_ReturnsBadRequest()
        {
            var sv = SeedSinhVien();
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);
            var caHoc = SeedCaHoc();
            var kh = SeedKHNXCaHoc(idCaHoc: caHoc.IdCaHoc);

            SaveAll();

            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4" // chưa nằm trong IP hợp lệ
            };
            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // ✅ 7. IP không hợp lệ
        [Test]
        public async Task CheckAttendance_InvalidIP_ReturnsBadRequest()
        {
            // 1. Seed sinh viên
            var sv = SeedSinhVien();

            // 2. Seed cơ sở + địa điểm
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);

            // 3. Seed IP hợp lệ (sẽ khác với IP test)
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "10.0.0.1",
                KieuIP = "LAN",        // ✅ Bắt buộc
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // 4. Seed CaHoc & KHNXCaHoc
            var caHoc = SeedCaHoc();
            var kh = SeedKHNXCaHoc(idCaHoc: caHoc.IdCaHoc);

            SaveAll();

            // 5. DTO có IP sai
            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4",  // sai => BadRequest
                Latitude = 10.123,      // hợp lệ
                Longitude = 106.123     // hợp lệ
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        // ✅ 8. Tọa độ không hợp lệ
        [Test]
        public async Task CheckAttendance_InvalidLocation_ReturnsBadRequest()
        {
            // 1. Seed sinh viên
            var sv = SeedSinhVien();

            // 2. Seed cơ sở + địa điểm
            var coSo = SeedCoSo();
            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                TrangThai = true,
                TenDiaDiem = "A1",
                ViDo = 10,       // vị trí hợp lệ
                KinhDo = 10,
                BanKinh = 1      // bán kính nhỏ => dễ fail
            };
            _context.DiaDiems.Add(diaDiem);

            // 3. Seed IP hợp lệ
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // 4. Seed CaHoc & KHNXCaHoc
            var caHoc = SeedCaHoc();
            var kh = SeedKHNXCaHoc(idCaHoc: caHoc.IdCaHoc);

            SaveAll();

            // 5. DTO với tọa độ sai (cách quá xa => fail)
            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4",   // hợp lệ
                Latitude = 20,           // sai vị trí
                Longitude = 20
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        // ✅ 9. Check-in quá sớm
        [Test]
        public async Task CheckAttendance_TooEarlyCheckIn_ReturnsBadRequest()
        {
            // 1. Seed sinh viên
            var sv = SeedSinhVien();

            // 2. Seed cơ sở + địa điểm
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);

            // 3. Seed IP hợp lệ
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // 4. Seed CaHoc bắt đầu 2h sau (để check-in quá sớm)
            var ca = SeedCaHoc(DateTime.Now.AddHours(2).TimeOfDay, DateTime.Now.AddHours(4).TimeOfDay);

            // 5. Seed KHNXCaHoc gắn CaHoc
            var kh = SeedKHNXCaHoc(idCaHoc: ca.IdCaHoc);

            SaveAll();

            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4",  // hợp lệ
                Latitude = 10.123,      // hợp lệ
                Longitude = 106.123     // hợp lệ
            };

            // Expect BadRequest vì check-in quá sớm
            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        // ✅ 10. Check-in quá muộn
        [Test]
        public async Task CheckAttendance_TooLateCheckIn_ReturnsBadRequest()
        {
            // 1. Sinh viên
            var sv = SeedSinhVien();

            // 2. CoSo + DiaDiem
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);

            // 3. IP hợp lệ
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // 4. CaHoc bắt đầu 2h trước => quá muộn để check-in
            var ca = SeedCaHoc(DateTime.Now.AddHours(-2).TimeOfDay, DateTime.Now.AddHours(1).TimeOfDay);

            // 5. KHNXCaHoc gắn CaHoc & DiemDanhTre = 0 (không cho đi trễ)
            var kh = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = Guid.NewGuid(),
                IdCaHoc = ca.IdCaHoc,
                TrangThai = 1,
                Buoi = "Buổi 1",
                DiemDanhTre = "0", // không cho đi trễ
                LinkOnline = "https://zoom.us",
                NoiDung = "Bài học muộn",
                ThoiGian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            _context.KHNXCaHocs.Add(kh);
            SaveAll();

            // 6. DTO hợp lệ, nhưng thời gian quá muộn
            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4",
                Latitude = 10.123,
                Longitude = 106.123
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        // ✅ 11. Check-out quá sớm
        [Test]
        public async Task CheckAttendance_TooEarlyCheckOut_ReturnsBadRequest()
        {
            var sv = SeedSinhVien();
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // CaHoc vẫn còn 2h nữa mới kết thúc
            var ca = SeedCaHoc(DateTime.Now.AddHours(-2).TimeOfDay, DateTime.Now.AddHours(2).TimeOfDay);

            var kh = SeedKHNXCaHoc(idCaHoc: ca.IdCaHoc);
            SaveAll();

            // DTO check-out quá sớm
            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = false,    // check-out
                IPAddress = "1.2.3.4",
                Latitude = 10.123,
                Longitude = 106.123
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        // ✅ 12. Không có group assignment
        [Test]
        public async Task CheckAttendance_NoGroupAssignment_ReturnsBadRequest()
        {
            var sv = SeedSinhVien();
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // CaHoc hợp lệ nhưng KHNXCaHoc không gán đúng group assignment
            var ca = SeedCaHoc();
            var kh = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = Guid.NewGuid(), // không match bất kỳ KeHoachNhomXuong nào
                IdCaHoc = ca.IdCaHoc,
                TrangThai = 1,
                Buoi = "Buổi 1",
                DiemDanhTre = "15",
                LinkOnline = "https://zoom.us",
                NoiDung = "Thiếu group",
                ThoiGian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            _context.KHNXCaHocs.Add(kh);
            SaveAll();

            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4",
                Latitude = 10.123,
                Longitude = 106.123
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }



       

        //13. Test Check-in bằng IP hợp lệ nhưng sai cơ sở → BadRequest
        [Test]
        public async Task CheckAttendance_WrongCampusIP_ReturnsBadRequest()
        {
            var sv = SeedSinhVien();
            var coSo1 = SeedCoSo();   // cơ sở 1
            var coSo2 = SeedCoSo();   // cơ sở 2 khác

            var diaDiem = SeedDiaDiem(coSo2.IdCoSo); // KHNXCaHoc ở cơ sở khác

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo1.IdCoSo, // IP thuộc cơ sở khác
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            var ca = SeedCaHoc(DateTime.Now.AddMinutes(-10).TimeOfDay, DateTime.Now.AddHours(1).TimeOfDay);
            var kh = SeedKHNXCaHoc(idCaHoc: ca.IdCaHoc);
            SaveAll();

            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4", // hợp lệ nhưng sai campus
                Latitude = 10.123,
                Longitude = 106.123
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        //14. est Check-in đúng nhưng ngoài giờ học → BadRequest
        [Test]
        public async Task CheckAttendance_OutOfClassTime_ReturnsBadRequest()
        {
            var sv = SeedSinhVien();
            var coSo = SeedCoSo();
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // CaHoc kết thúc 1h trước → hiện tại đã hết giờ
            var ca = SeedCaHoc(DateTime.Now.AddHours(-3).TimeOfDay, DateTime.Now.AddHours(-1).TimeOfDay);
            var kh = SeedKHNXCaHoc(idCaHoc: ca.IdCaHoc);
            SaveAll();

            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = "1.2.3.4",
                Latitude = 10.123,
                Longitude = 106.123
            };

            var result = await _controller.CheckAttendance(dto);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        // ✅ 15. Thiếu IPAddress => BadRequest
        [Test]
        public async Task CheckAttendance_MissingIPAddress_ReturnsBadRequest()
        {
            // 1️⃣ Seed dữ liệu cần thiết
            var sv = SeedSinhVien();                 // Sinh viên hợp lệ
            var coSo = SeedCoSo();                   // Cơ sở
            var diaDiem = SeedDiaDiem(coSo.IdCoSo);  // Địa điểm trong campus

            // IP hợp lệ (nhưng DTO sẽ thiếu)
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                IP_DaiIP = "1.2.3.4",
                KieuIP = "LAN",
                TrangThai = true
            };
            _context.IPs.Add(ip);

            // Ca học đang diễn ra → đúng giờ
            var ca = SeedCaHoc(DateTime.Now.AddMinutes(-10).TimeOfDay, DateTime.Now.AddHours(1).TimeOfDay);

            // Kế hoạch ca học hợp lệ
            var kh = SeedKHNXCaHoc(idCaHoc: ca.IdCaHoc);
            SaveAll();

            // 2️⃣ DTO bỏ trống IPAddress
            var dto = new AttendanceCheckDto
            {
                Email = sv.Email,
                IdNXCH = kh.IdNXCH,
                IsCheckIn = true,
                IPAddress = null,       // ❌ Thiếu IP
                Latitude = 10.123,      // Đúng tọa độ
                Longitude = 106.123
            };

            // 3️⃣ Thực thi controller
            var result = await _controller.CheckAttendance(dto);

            // 4️⃣ Kết quả phải là BadRequest
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }








    }
}
