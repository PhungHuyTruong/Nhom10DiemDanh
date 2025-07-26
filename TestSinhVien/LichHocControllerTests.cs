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

        
    }
}
