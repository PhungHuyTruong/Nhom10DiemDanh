using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom10ModuleDiemDanh.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnitTest
{
    [TestFixture]
    public class LichGiangDaysControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private LichGiangDaysController _controller;
        private Guid _testNhanVienId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);

            _testNhanVienId = Guid.NewGuid();
            var idCoSo = Guid.NewGuid();
            var idDuAn = Guid.NewGuid();

            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = TimeSpan.FromHours(8),
                ThoiGianKetThuc = TimeSpan.FromHours(10),
                TrangThai = 1
            };

            var coSo = new CoSo
            {
                IdCoSo = idCoSo,
                TenCoSo = "Cơ sở A",
                MaCoSo = "CSHN01",
                DiaChi = "Hà Nội",
                Email = "test@example.com",
                SDT = "0987654321"
            };

            var diaDiem = new DiaDiem
            {
                IdDiaDiem = Guid.NewGuid(),
                TenDiaDiem = "Phòng A",
                IdCoSo = idCoSo
            };

            var duAn = new DuAn
            {
                IdDuAn = idDuAn,
                TenDuAn = "Dự án A",
                MoTa = "Mô tả cho dự án A"
            };

            var nhanVien = new PhuTrachXuong
            {
                IdNhanVien = _testNhanVienId,
                IdCoSo = idCoSo,
                MaNhanVien = "NV001",
                TenNhanVien = "Nguyễn Văn Test",
                EmailFE = "testnv.fe@fpt.edu.vn",
                EmailFPT = "testnv@fpt.com"
            };

            var nhomXuong = new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                IdPhuTrachXuong = _testNhanVienId,
                TenNhomXuong = "Nhóm A",
                MoTa = "Mô tả cho Nhóm A"
            };

            // ✅ SỬA Ở ĐÂY: Bổ sung ĐẦY ĐỦ các thuộc tính bắt buộc cho KeHoach
            var keHoach = new KeHoach
            {
                IdDuAn = idDuAn,
                TenKeHoach = "Kế hoạch Test",
                NoiDung = "Nội dung cho kế hoạch Test",
                ThoiGianBatDau = DateTime.Today, // Bắt buộc
                ThoiGianKetThuc = DateTime.Today.AddMonths(3) // Bắt buộc
            };

            var keHoachNhomXuong = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdNhomXuong = nhomXuong.IdNhomXuong,
                KeHoach = keHoach
            };

            var caHocEntry = new KHNXCaHoc
            {
                IdKHNX = keHoachNhomXuong.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Today,
                ThoiGian = "08:00 - 10:00",
                DiemDanhTre = "false",
                LinkOnline = "",
                Buoi = "Sáng", // Bắt buộc
                NoiDung = "Nội dung buổi học test" // Bắt buộc
            };

            _context.CaHocs.Add(caHoc);
            _context.CoSos.Add(coSo);
            _context.DiaDiems.Add(diaDiem);
            _context.DuAns.Add(duAn);
            _context.PhuTrachXuongs.Add(nhanVien);
            _context.NhomXuongs.Add(nhomXuong);
            _context.KeHoachNhomXuongs.Add(keHoachNhomXuong);
            _context.KHNXCaHocs.Add(caHocEntry);
            _context.SaveChanges();

            _controller = new LichGiangDaysController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // ... (Toàn bộ các hàm [Test] giữ nguyên như cũ) ...

        [Test]
        public async Task Return_Ok_With_ValidId()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task Return_Empty_With_InvalidId()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(Guid.NewGuid());
            var data = (result.Result as OkObjectResult)?.Value as List<LichGiangDayViewModel>;
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task Return_One_Record_Correctly()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            var data = (result.Result as OkObjectResult)?.Value as List<LichGiangDayViewModel>;
            Assert.AreEqual(1, data.Count);
        }

        [Test]
        public async Task Return_Type_List_Of_LichGiangDayViewModel()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            Assert.IsInstanceOf<List<LichGiangDayViewModel>>((result.Result as OkObjectResult)?.Value);
        }

        [Test]
        public async Task Check_TenNhomXuong_NotEmpty()
        {
            var data = await GetData();
            Assert.IsTrue(data.All(x => !string.IsNullOrEmpty(x.TenNhomXuong)));
        }

        [Test]
        public async Task Check_DuAn_Equals()
        {
            var data = await GetData();
            Assert.AreEqual("Dự án A", data[0].DuAn);
        }

        [Test]
        public async Task Check_Ca_Name()
        {
            var data = await GetData();
            Assert.AreEqual("Ca 1", data[0].Ca);
        }

        [Test]
        public async Task Check_ThoiGian()
        {
            var data = await GetData();
            Assert.AreEqual("08:00 - 10:00", data[0].ThoiGian);
        }

        [Test]
        public async Task HinhThuc_ShouldBe_Offline_When_LinkEmpty()
        {
            var data = await GetData();
            Assert.AreEqual("Offline", data[0].HinhThuc);
        }

        [Test]
        public async Task DiaDiem_ShouldBe_PhongA()
        {
            var data = await GetData();
            Assert.AreEqual("Phòng A", data[0].DiaDiem);
        }

        [Test]
        public async Task NgayHoc_ShouldBe_Today()
        {
            var data = await GetData();
            Assert.AreEqual(DateTime.Today, data[0].NgayHoc.Date);
        }

        [Test]
        public async Task Return_NotNull_When_Empty()
        {
            var controller = new LichGiangDaysController(new ModuleDiemDanhDbContext(
                new DbContextOptionsBuilder<ModuleDiemDanhDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options));

            var result = await controller.GetLichGiangDayTheoNhanVien(Guid.NewGuid());
            Assert.IsNotNull((result.Result as OkObjectResult)?.Value);
        }

        [Test]
        public async Task Return_Multiple_Records()
        {
            var ca = _context.KHNXCaHocs.First();
            _context.KHNXCaHocs.Add(new KHNXCaHoc
            {
                IdKHNX = ca.IdKHNX,
                IdCaHoc = ca.IdCaHoc,
                NgayHoc = DateTime.Today,
                ThoiGian = "10:00 - 12:00",
                LinkOnline = "http://zoom.com",

                // Thêm các trường bắt buộc còn thiếu
                Buoi = "Chiều",
                NoiDung = "Nội dung buổi học chiều",
                DiemDanhTre = "false"
            });
            await _context.SaveChangesAsync();

            var data = await GetData();
            Assert.AreEqual(2, data.Count);
        }

        [Test]
        public async Task Should_Be_Sorted_By_NgayHoc_And_Ca()
        {
            var data = await GetData();
            var sorted = data.OrderBy(x => x.NgayHoc).ThenBy(x => x.Ca).ToList();
            CollectionAssert.AreEqual(sorted, data);
        }

        [Test]
        public async Task HinhThuc_ShouldBe_Online_When_LinkExists()
        {
            var ca = _context.KHNXCaHocs.First();
            ca.LinkOnline = "http://meet.com";
            await _context.SaveChangesAsync();

            var data = await GetData();
            Assert.AreEqual("Online", data[0].HinhThuc);
        }

        [Test]
        public async Task DiemDanhTre_ShouldBe_False()
        {
            var data = await GetData();
            Assert.AreEqual("false", data[0].DiemDanhTre?.ToLower());
        }

        [Test]
        public async Task DiaDiem_ShouldBe_Minus_When_NotFound()
        {
            _context.DiaDiems.RemoveRange(_context.DiaDiems);
            await _context.SaveChangesAsync();

            var data = await GetData();
            Assert.AreEqual("-", data[0].DiaDiem);
        }

        [Test]
        public async Task ActionResult_ShouldBe_Valid()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            Assert.IsInstanceOf<ActionResult<List<LichGiangDayViewModel>>>(result);
        }

        [Test]
        public async Task ShouldNot_Throw_When_NoCaHoc()
        {
            _context.KHNXCaHocs.RemoveRange(_context.KHNXCaHocs);
            await _context.SaveChangesAsync();

            var data = await GetData();
            Assert.IsEmpty(data);
        }

        // Helper
        private async Task<List<LichGiangDayViewModel>> GetData()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            return (result.Result as OkObjectResult)?.Value as List<LichGiangDayViewModel>;
        }
    }
}