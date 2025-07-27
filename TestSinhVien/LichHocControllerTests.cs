using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSinhVien
{
    [TestFixture]
    public class LichHocControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private LichHocViewsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new LichHocViewsController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // 1. Sinh viên không tồn tại
        [Test]
        public async Task GetLichHocTheoSinhVien_StudentNotFound_ReturnsNotFound()
        {
            var result = await _controller.GetLichHocTheoSinhVien(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        // 2. Sinh viên chưa thuộc nhóm xưởng
        [Test]
        public async Task GetLichHocTheoSinhVien_StudentNoGroup_ReturnsNotFound()
        {
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = null,
                Email = "test@student.com",      // BẮT BUỘC
                MaSinhVien = "SV001",            // BẮT BUỘC
                TrangThai = true                 // Nên có nếu model yêu cầu
            };
            _context.SinhViens.Add(sv);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        // 3. Sinh viên hợp lệ, không có lịch học
        [Test]
        public async Task GetLichHocTheoSinhVien_StudentNoSchedule_ReturnsEmptyList()
        {
            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test" // BẮT BUỘC
            };
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",   // BẮT BUỘC
                MaSinhVien = "SV001",         // BẮT BUỘC
                TrangThai = true              // Nên có nếu model yêu cầu
            };
            _context.NhomXuongs.Add(nx);
            _context.SinhViens.Add(sv);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            Assert.IsNotNull(result);
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;
            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.Count);
        }

        

        //4. Sinh viên có lịch học nhưng thiếu kế hoạch nhóm xưởng
        [Test]
        public async Task GetLichHocTheoSinhVien_MissingKeHoachNhomXuong_ReturnsEmpty()
        {
            var nx = new NhomXuong { IdNhomXuong = Guid.NewGuid(), TenNhomXuong = "NX1", MoTa = "Nhóm test" };
            var sv = new SinhVien { IdSinhVien = Guid.NewGuid(), TenSinhVien = "Test", IdNhomXuong = nx.IdNhomXuong, Email = "test@student.com", MaSinhVien = "SV001", TrangThai = true };
            // Không seed KeHoachNhomXuong
            _context.NhomXuongs.Add(nx); _context.SinhViens.Add(sv); _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;
            Assert.AreEqual(0, data.Count);
        }


        //5. Sinh viên có lịch học hợp lệ (đầy đủ các liên kết)
        [Test]
        public async Task GetLichHocTheoSinhVien_ValidStudentWithSchedule_ReturnsSchedule()
        {
            var coSoId = Guid.NewGuid();

            // 1. Tạo DuAn (bổ sung MoTa bắt buộc)
            var duAn = new DuAn
            {
                IdDuAn = Guid.NewGuid(),
                TenDuAn = "Dự án test",
                MoTa = "Mô tả dự án test"   // ✅ Bổ sung required
            };

            // 2. Tạo KeHoach (gắn với DuAn, bổ sung MoTa nếu có required)
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "Kế hoạch test",
                NoiDung = "Nội dung kế hoạch test"
            };

            // 3. Tạo Giảng viên phụ trách
            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            // 4. Tạo CoSo
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC, Quận 1",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            // 5. Tạo DiaDiem
            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                TenDiaDiem = "Phòng 101"
            };

            // 6. Tạo NhomXuong (có MoTa required)
            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            // 7. SinhVien
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            // 8. KeHoachNhomXuong
            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            // 9. CaHoc (bổ sung MoTa nếu cần)
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            // 10. KHNXCaHoc
            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today.AddDays(1),
                Buoi = "Sáng",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com",
                NoiDung = "Ôn tập",
                ThoiGian = "07:00 - 11:00"
            };

            // ✅ Add FULL
            _context.DuAns.Add(duAn);
            _context.KeHoachs.Add(keHoach);
            _context.PhuTrachXuongs.Add(giangVien);
            _context.CoSos.Add(coSo);
            _context.DiaDiems.Add(diaDiem);
            _context.NhomXuongs.Add(nx);
            _context.SinhViens.Add(sv);
            _context.KeHoachNhomXuongs.Add(khnx);
            _context.CaHocs.Add(caHoc);
            _context.KHNXCaHocs.Add(khnxCaHoc);

            _context.SaveChanges();

            // Act
            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            // Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("Ca 1", data[0].TenCaHoc);
            Assert.AreEqual("GV Test", data[0].GiangVienPhuTrach);
        }






        //6. Lịch học có nhiều ca trong ngày
        [Test]
        public async Task GetLichHocTheoSinhVien_MultipleSchedulesInOneDay_ReturnsAll()
        {
            var coSoId = Guid.NewGuid();

            // 1. DuAn
            var duAn = new DuAn
            {
                IdDuAn = Guid.NewGuid(),
                TenDuAn = "Dự án test",
                MoTa = "Mô tả dự án test"
            };

            // 2. KeHoach
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "Kế hoạch test",
                NoiDung = "Nội dung kế hoạch test"
            };

            // 3. Giảng viên phụ trách (đầy đủ required)
            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            // 4. CoSo (đầy đủ required)
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC, Quận 1",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            // 5. DiaDiem
            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                TenDiaDiem = "Phòng 101"
            };

            // 6. NhomXuong
            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            // 7. SinhVien
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            // 8. KeHoachNhomXuong
            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            // 9. Hai ca học trong cùng 1 ngày
            var ca1 = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(9, 0, 0)
            };
            var ca2 = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 2",
                ThoiGianBatDau = new TimeSpan(9, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            // 10. Hai KHNXCaHoc trong cùng ngày
            var khnxCaHoc1 = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = ca1.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com/ca1",
                NoiDung = "Ôn tập Ca 1",
                ThoiGian = "07:00 - 09:00"
            };
            var khnxCaHoc2 = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = ca2.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com/ca2",
                NoiDung = "Ôn tập Ca 2",
                ThoiGian = "09:00 - 11:00"
            };

            // ✅ Add FULL
            _context.DuAns.Add(duAn);
            _context.KeHoachs.Add(keHoach);
            _context.PhuTrachXuongs.Add(giangVien);
            _context.CoSos.Add(coSo);
            _context.DiaDiems.Add(diaDiem);
            _context.NhomXuongs.Add(nx);
            _context.SinhViens.Add(sv);
            _context.KeHoachNhomXuongs.Add(khnx);
            _context.CaHocs.AddRange(ca1, ca2);
            _context.KHNXCaHocs.AddRange(khnxCaHoc1, khnxCaHoc2);

            _context.SaveChanges();

            // Act
            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            // Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Count);   // ✅ có 2 lịch
            var tenCaHocList = data.Select(d => d.TenCaHoc).ToList();
            CollectionAssert.Contains(tenCaHocList, "Ca 1");
            CollectionAssert.Contains(tenCaHocList, "Ca 2");
        }



        //7. Lịch học ở ngày trong quá khứ
        [Test]
        public async Task GetLichHocTheoSinhVien_ScheduleInPast_ReturnsCorrect()
        {
            var coSoId = Guid.NewGuid();

            // 1. DuAn (MoTa required)
            var duAn = new DuAn
            {
                IdDuAn = Guid.NewGuid(),
                TenDuAn = "Dự án test",
                MoTa = "Mô tả dự án test"
            };

            // 2. KeHoach (TenKeHoach + NoiDung required)
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "Kế hoạch test",
                NoiDung = "Nội dung kế hoạch test"
            };

            // 3. Giảng viên phụ trách (EmailFE, EmailFPT, MaNhanVien required)
            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            // 4. CoSo (DiaChi, Email, MaCoSo, SDT required)
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC, Quận 1",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            // 5. DiaDiem
            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                TenDiaDiem = "Phòng 101"
            };

            // 6. NhomXuong (MoTa required)
            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            // 7. SinhVien
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            // 8. KeHoachNhomXuong
            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            // 9. CaHoc
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            // 10. KHNXCaHoc (Ngày trong quá khứ)
            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today.AddDays(-1), // quá khứ
                Buoi = "Sáng",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com",
                NoiDung = "Ôn tập",
                ThoiGian = "07:00 - 11:00"
            };

            // ✅ Add FULL
            _context.DuAns.Add(duAn);
            _context.KeHoachs.Add(keHoach);
            _context.PhuTrachXuongs.Add(giangVien);
            _context.CoSos.Add(coSo);
            _context.DiaDiems.Add(diaDiem);
            _context.NhomXuongs.Add(nx);
            _context.SinhViens.Add(sv);
            _context.KeHoachNhomXuongs.Add(khnx);
            _context.CaHocs.Add(caHoc);
            _context.KHNXCaHocs.Add(khnxCaHoc);

            _context.SaveChanges();

            // Act
            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            // Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(DateTime.Today.AddDays(-1), data[0].NgayHoc);
        }



        //8. Lịch học ở ngày trong tương lai
        [Test]
        public async Task GetLichHocTheoSinhVien_ScheduleInFuture_ReturnsCorrect()
        {
            var coSoId = Guid.NewGuid();

            // 1. DuAn (MoTa required)
            var duAn = new DuAn
            {
                IdDuAn = Guid.NewGuid(),
                TenDuAn = "Dự án test",
                MoTa = "Mô tả dự án test"
            };

            // 2. KeHoach (TenKeHoach + NoiDung required)
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "Kế hoạch test",
                NoiDung = "Nội dung kế hoạch test"
            };

            // 3. Giảng viên phụ trách (EmailFE, EmailFPT, MaNhanVien required)
            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            // 4. CoSo (DiaChi, Email, MaCoSo, SDT required)
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC, Quận 1",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            // 5. DiaDiem
            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = coSo.IdCoSo,
                TenDiaDiem = "Phòng 101"
            };

            // 6. NhomXuong (MoTa required)
            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            // 7. SinhVien
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            // 8. KeHoachNhomXuong
            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            // 9. CaHoc
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            // 10. KHNXCaHoc (Ngày tương lai)
            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today.AddDays(1), // tương lai
                Buoi = "Sáng",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com",
                NoiDung = "Ôn tập",
                ThoiGian = "07:00 - 11:00"
            };

            // ✅ Add FULL
            _context.DuAns.Add(duAn);
            _context.KeHoachs.Add(keHoach);
            _context.PhuTrachXuongs.Add(giangVien);
            _context.CoSos.Add(coSo);
            _context.DiaDiems.Add(diaDiem);
            _context.NhomXuongs.Add(nx);
            _context.SinhViens.Add(sv);
            _context.KeHoachNhomXuongs.Add(khnx);
            _context.CaHocs.Add(caHoc);
            _context.KHNXCaHocs.Add(khnxCaHoc);

            _context.SaveChanges();

            // Act
            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            // Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(DateTime.Today.AddDays(1), data[0].NgayHoc);
        }



        //9. Lịch học có thông tin ca học đầy đủ
        [Test]
        public async Task GetLichHocTheoSinhVien_ScheduleHasFullCaHocInfo()
        {
            var coSoId = Guid.NewGuid();

            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "Dự án test", MoTa = "Mô tả dự án test" };
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "Kế hoạch test",
                NoiDung = "Nội dung kế hoạch test"
            };

            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            var diaDiem = new DiaDiem { IdDiaDiem = Guid.NewGuid(), IdCoSo = coSo.IdCoSo, TenDiaDiem = "Phòng 101" };

            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                DiemDanhTre = "10 phút",
                LinkOnline = "https://example.com",
                NoiDung = "Ôn tập",
                ThoiGian = "07:00 - 11:00"
            };

            _context.AddRange(duAn, keHoach, giangVien, coSo, diaDiem, nx, sv, khnx, caHoc, khnxCaHoc);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            Assert.IsNotNull(data);
            Assert.AreEqual("Ca 1", data[0].TenCaHoc);
        }


        //10. Sinh viên có nhiều lịch học ở các nhóm xưởng khác nhau (chỉ lấy đúng nhóm)
        [Test]
        public async Task GetLichHocTheoSinhVien_StudentInMultipleGroups_OnlyOwnGroupSchedule()
        {
            var coSoId = Guid.NewGuid();

            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "Dự án test", MoTa = "Mô tả dự án test" };
            var keHoach1 = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "KH Nhóm 1",
                NoiDung = "Nội dung KH1"
            };
            var keHoach2 = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "KH Nhóm 2",
                NoiDung = "Nội dung KH2"
            };

            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            var diaDiem = new DiaDiem { IdDiaDiem = Guid.NewGuid(), IdCoSo = coSo.IdCoSo, TenDiaDiem = "Phòng 101" };

            var nx1 = new NhomXuong { IdNhomXuong = Guid.NewGuid(), TenNhomXuong = "NX1", MoTa = "Nhóm 1", IdPhuTrachXuong = giangVien.IdNhanVien };
            var nx2 = new NhomXuong { IdNhomXuong = Guid.NewGuid(), TenNhomXuong = "NX2", MoTa = "Nhóm 2", IdPhuTrachXuong = giangVien.IdNhanVien };

            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx1.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            var khnx1 = new KeHoachNhomXuong { IdKHNX = Guid.NewGuid(), IdKeHoach = keHoach1.IdKeHoach, IdNhomXuong = nx1.IdNhomXuong };
            var khnx2 = new KeHoachNhomXuong { IdKHNX = Guid.NewGuid(), IdKeHoach = keHoach2.IdKeHoach, IdNhomXuong = nx2.IdNhomXuong };

            var ca1 = new CaHoc { IdCaHoc = Guid.NewGuid(), TenCaHoc = "Ca 1", ThoiGianBatDau = new TimeSpan(7, 0, 0), ThoiGianKetThuc = new TimeSpan(11, 0, 0) };
            var ca2 = new CaHoc { IdCaHoc = Guid.NewGuid(), TenCaHoc = "Ca 2", ThoiGianBatDau = new TimeSpan(13, 0, 0), ThoiGianKetThuc = new TimeSpan(17, 0, 0) };

            var khnxCaHoc1 = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx1.IdKHNX,
                IdCaHoc = ca1.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com/ca1",
                NoiDung = "Ôn tập Ca 1",
                ThoiGian = "07:00 - 11:00"
            };
            var khnxCaHoc2 = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx2.IdKHNX,
                IdCaHoc = ca2.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Chiều",
                DiemDanhTre = "15 phút",
                LinkOnline = "https://example.com/ca2",
                NoiDung = "Ôn tập Ca 2",
                ThoiGian = "13:00 - 17:00"
            };

            _context.AddRange(duAn, keHoach1, keHoach2, giangVien, coSo, diaDiem, nx1, nx2, sv, khnx1, khnx2, ca1, ca2, khnxCaHoc1, khnxCaHoc2);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("Ca 1", data[0].TenCaHoc);
        }


        //11. Lịch học có thông tin bổ sung (ví dụ: Buổi, Nội dung)
        [Test]
        public async Task GetLichHocTheoSinhVien_ScheduleWithExtraInfo_ReturnsCorrect()
        {
            var coSoId = Guid.NewGuid();

            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "Dự án test", MoTa = "Mô tả dự án test" };
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "Kế hoạch test",
                NoiDung = "Nội dung kế hoạch test"
            };

            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv.test@fe.edu.vn",
                EmailFPT = "gv.test@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "123 Đường ABC",
                Email = "coso.a@fpt.edu.vn",
                MaCoSo = "CS001",
                SDT = "0901234567"
            };

            var diaDiem = new DiaDiem { IdDiaDiem = Guid.NewGuid(), IdCoSo = coSo.IdCoSo, TenDiaDiem = "Phòng 101" };

            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                NoiDung = "Ôn tập chi tiết",
                DiemDanhTre = "10 phút",
                LinkOnline = "https://example.com",
                ThoiGian = "07:00 - 11:00"
            };

            _context.AddRange(duAn, keHoach, giangVien, coSo, diaDiem, nx, sv, khnx, caHoc, khnxCaHoc);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            Assert.IsNotNull(data);
            Assert.AreEqual("Sáng", data[0].Buoi);
        }


        //12. Sinh viên hợp lệ nhưng tất cả KHNXCaHoc ở nhóm khác → trả về rỗng
        [Test]
        public async Task GetLichHocTheoSinhVien_ValidStudentButAllSchedulesInOtherGroup_ReturnsEmpty()
        {
            // Tạo đầy đủ dữ liệu nhóm khác
            var coSoId = Guid.NewGuid();

            var otherGroupId = Guid.NewGuid();
            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "DuAn Other", MoTa = "Khác" };
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "KH Other",
                NoiDung = "KH Nhóm khác"
            };
            var giangVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Other",
                IdCoSo = coSoId,
                EmailFE = "gv.other@fe.edu.vn",
                EmailFPT = "gv.other@fpt.edu.vn",
                MaNhanVien = "GV002"
            };

            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở B",
                DiaChi = "Địa chỉ B",
                Email = "coso.b@fpt.edu.vn",
                MaCoSo = "CS002",
                SDT = "0909876543"
            };
            var diaDiem = new DiaDiem { IdDiaDiem = Guid.NewGuid(), IdCoSo = coSo.IdCoSo, TenDiaDiem = "Phòng B" };

            // Nhóm xưởng khác
            var nxOther = new NhomXuong
            {
                IdNhomXuong = otherGroupId,
                TenNhomXuong = "NX_OTHER",
                MoTa = "Nhóm khác",
                IdPhuTrachXuong = giangVien.IdNhanVien
            };

            // Sinh viên thuộc nhóm KHÔNG có lịch
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = Guid.NewGuid(), // nhóm khác, ko có lịch
                Email = "test@student.com",
                MaSinhVien = "SV123",
                TrangThai = true
            };

            // Lịch học của nhóm khác
            var khnx = new KeHoachNhomXuong { IdKHNX = Guid.NewGuid(), IdKeHoach = keHoach.IdKeHoach, IdNhomXuong = otherGroupId };
            var caHoc = new CaHoc { IdCaHoc = Guid.NewGuid(), TenCaHoc = "Ca Other", ThoiGianBatDau = new TimeSpan(7, 0, 0), ThoiGianKetThuc = new TimeSpan(11, 0, 0) };
            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                NoiDung = "Lịch nhóm khác",
                DiemDanhTre = "5 phút",
                LinkOnline = "https://example.com",
                ThoiGian = "07:00-11:00"
            };

            _context.AddRange(duAn, keHoach, giangVien, coSo, diaDiem, nxOther, sv, khnx, caHoc, khnxCaHoc);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.Count);
        }

        //13. Có lịch nhưng thiếu DiaDiem → trả về “Chưa cập nhật”
        [Test]
        public async Task GetLichHocTheoSinhVien_ScheduleWithoutDiaDiem_ReturnsDefaultValue()
        {
            // Seed full, nhưng KHÔNG thêm DiaDiem
            var coSoId = Guid.NewGuid();
            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "DuAn Test", MoTa = "MoTa" };
            var keHoach = new KeHoach { IdKeHoach = Guid.NewGuid(), IdDuAn = duAn.IdDuAn, TenKeHoach = "KH", NoiDung = "ND" };
            var gv = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV",
                IdCoSo = coSoId,
                EmailFE = "gv@fe.edu.vn",
                EmailFPT = "gv@fpt.edu.vn",
                MaNhanVien = "GV003"
            };
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "CS",
                DiaChi = "ĐC",
                Email = "cs@fpt.edu.vn",
                MaCoSo = "CS03",
                SDT = "0123"
            };
            var nx = new NhomXuong { IdNhomXuong = Guid.NewGuid(), TenNhomXuong = "NX", MoTa = "MT", IdPhuTrachXuong = gv.IdNhanVien };
            var sv = new SinhVien { IdSinhVien = Guid.NewGuid(), TenSinhVien = "SV", IdNhomXuong = nx.IdNhomXuong, Email = "sv@test", MaSinhVien = "SV001", TrangThai = true };
            var khnx = new KeHoachNhomXuong { IdKHNX = Guid.NewGuid(), IdKeHoach = keHoach.IdKeHoach, IdNhomXuong = nx.IdNhomXuong };
            var ca = new CaHoc { IdCaHoc = Guid.NewGuid(), TenCaHoc = "Ca", ThoiGianBatDau = new TimeSpan(7, 0, 0), ThoiGianKetThuc = new TimeSpan(11, 0, 0) };
            var khnxCa = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = ca.IdCaHoc,
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                NoiDung = "ND",
                DiemDanhTre = "0",
                LinkOnline = "-",
                ThoiGian = "07-11"
            };

            _context.AddRange(duAn, keHoach, gv, coSo, nx, sv, khnx, ca, khnxCa); // ❌ không add DiaDiem
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;
            Assert.IsNotNull(data);
            Assert.AreEqual("Chưa cập nhật", data[0].DiaDiem);
        }

        //14. Có lịch nhưng thiếu CaHoc → trả về “Không rõ”
        [Test]
        public async Task GetLichHocTheoSinhVien_ScheduleWithoutCaHoc_ReturnsUnknownCa()
        {
            var coSoId = Guid.NewGuid();
            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "DuAn", MoTa = "MoTa" };
            var keHoach = new KeHoach { IdKeHoach = Guid.NewGuid(), IdDuAn = duAn.IdDuAn, TenKeHoach = "KH", NoiDung = "ND" };
            var gv = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV",
                IdCoSo = coSoId,
                EmailFE = "gv@fe.edu.vn",
                EmailFPT = "gv@fpt.edu.vn",
                MaNhanVien = "GV004"
            };
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "CS",
                DiaChi = "ĐC",
                Email = "cs@fpt.edu.vn",
                MaCoSo = "CS04",
                SDT = "0123"
            };
            var diaDiem = new DiaDiem { IdDiaDiem = Guid.NewGuid(), IdCoSo = coSoId, TenDiaDiem = "P404" };
            var nx = new NhomXuong { IdNhomXuong = Guid.NewGuid(), TenNhomXuong = "NX", MoTa = "MT", IdPhuTrachXuong = gv.IdNhanVien };
            var sv = new SinhVien { IdSinhVien = Guid.NewGuid(), TenSinhVien = "SV", IdNhomXuong = nx.IdNhomXuong, Email = "sv@test", MaSinhVien = "SV001", TrangThai = true };
            var khnx = new KeHoachNhomXuong { IdKHNX = Guid.NewGuid(), IdKeHoach = keHoach.IdKeHoach, IdNhomXuong = nx.IdNhomXuong };

            // KHNXCaHoc có IdCaHoc nhưng ko add vào context
            var khnxCa = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = khnx.IdKHNX,
                IdCaHoc = Guid.NewGuid(), // ❌ không seed CaHoc
                NgayHoc = DateTime.Today,
                Buoi = "Sáng",
                NoiDung = "ND",
                DiemDanhTre = "0",
                LinkOnline = "-",
                ThoiGian = "07-11"
            };

            _context.AddRange(duAn, keHoach, gv, coSo, diaDiem, nx, sv, khnx, khnxCa);
            _context.SaveChanges();

            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;
            Assert.IsNotNull(data);
            Assert.AreEqual("Không rõ", data[0].TenCaHoc);
        }


        //15. Sinh viên có 5 lịch khác ngày → sắp xếp đúng theo ngày
        [Test]
        public async Task GetLichHocTheoSinhVien_MultipleDays_SortedByNgayHoc()
        {
            var coSoId = Guid.NewGuid();

            // 1. DuAn
            var duAn = new DuAn
            {
                IdDuAn = Guid.NewGuid(),
                TenDuAn = "Dự án test",
                MoTa = "Mô tả dự án"
            };

            // 2. KeHoach
            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                IdDuAn = duAn.IdDuAn,
                TenKeHoach = "KH Test",
                NoiDung = "Nội dung"
            };

            // 3. Giảng viên
            var gv = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "GV Test",
                IdCoSo = coSoId,
                EmailFE = "gv@fe.edu.vn",
                EmailFPT = "gv@fpt.edu.vn",
                MaNhanVien = "GV001"
            };

            // 4. CoSo
            var coSo = new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở A",
                DiaChi = "ĐC",
                Email = "coso@fpt.edu.vn",
                MaCoSo = "CS01",
                SDT = "0901111111"
            };

            // 5. DiaDiem
            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                IdCoSo = coSoId,
                TenDiaDiem = "Phòng 101"
            };

            // 6. NhomXuong
            var nx = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "NX1",
                MoTa = "Nhóm test",
                IdPhuTrachXuong = gv.IdNhanVien
            };

            // 7. SinhVien
            var sv = new SinhVien
            {
                IdSinhVien = Guid.NewGuid(),
                TenSinhVien = "Test",
                IdNhomXuong = nx.IdNhomXuong,
                Email = "test@student.com",
                MaSinhVien = "SV001",
                TrangThai = true
            };

            // 8. KeHoachNhomXuong
            var khnx = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = nx.IdNhomXuong
            };

            // 9. CaHoc
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            // ✅ Lưu các entity cần thiết
            _context.AddRange(duAn, keHoach, gv, coSo, diaDiem, nx, sv, khnx, caHoc);

            // 10. Tạo 5 lịch với ngày khác nhau
            int[] days = { -2, 0, 3, 1, -1 };
            foreach (var offset in days)
            {
                _context.KHNXCaHocs.Add(new KHNXCaHoc
                {
                    IdNXCH = Guid.NewGuid(),
                    IdKHNX = khnx.IdKHNX,
                    IdCaHoc = caHoc.IdCaHoc,
                    NgayHoc = DateTime.Today.AddDays(offset),
                    Buoi = "Sáng",
                    NoiDung = $"Ôn tập {offset}",
                    ThoiGian = "07:00 - 11:00",
                    LinkOnline = "https://example.com",
                    DiemDanhTre = "15 phút" // ✅ Bổ sung bắt buộc
                });
            }

            _context.SaveChanges();

            // Act
            var result = await _controller.GetLichHocTheoSinhVien(sv.IdSinhVien) as OkObjectResult;
            var data = result.Value as List<LichHocViewsController.LichHocViewDto>;

            // Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(5, data.Count);

            // Kiểm tra thứ tự ngày tăng dần
            var sorted = data.Select(d => d.NgayHoc).OrderBy(d => d).ToList();
            CollectionAssert.AreEqual(sorted, data.Select(d => d.NgayHoc).ToList());
        }









    }
}
