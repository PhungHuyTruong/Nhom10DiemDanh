using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NUnitTest
{
    [TestFixture]
    public class NhomXuongCuaToiControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private NhomXuongCuaToiController _controller;

        // --- Dữ liệu test ---
        private Guid _giangVien1Id;
        private Guid _giangVien2Id;
        private const string GiangVien1EmailFE = "thaygv1@fe.edu.vn";
        private const string GiangVien1EmailFPT = "thaygv1@fpt.com";
        private const string NonExistentEmail = "nguoidungla@fe.edu.vn";

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);

            // Khởi tạo dữ liệu mẫu
            _giangVien1Id = Guid.NewGuid();
            _giangVien2Id = Guid.NewGuid();
            var duAn1Id = Guid.NewGuid();
            var boMon1Id = Guid.NewGuid();

            // 1. Giảng viên
            var gv1 = new PhuTrachXuong
            {
                IdNhanVien = _giangVien1Id,
                TenNhanVien = "Giảng Viên 1",
                MaNhanVien = "GV01",
                EmailFE = GiangVien1EmailFE,
                EmailFPT = GiangVien1EmailFPT
            };

            var gv2 = new PhuTrachXuong
            {
                IdNhanVien = _giangVien2Id,
                TenNhanVien = "Giảng Viên 2",
                MaNhanVien = "GV02",
                EmailFE = "thaygv2@fe.edu.vn",
                EmailFPT = "thaygv2@fpt.com"
            };

            _context.PhuTrachXuongs.AddRange(gv1, gv2);

            // 2. Vai trò
            _context.VaiTroNhanViens.AddRange(
                new VaiTroNhanVien
                {
                    IdVaiTro = Guid.NewGuid(),
                    IdNhanVien = gv1.IdNhanVien,
                    TrangThai = true
                },
                new VaiTroNhanVien
                {
                    IdVaiTro = Guid.NewGuid(),
                    IdNhanVien = gv2.IdNhanVien,
                    TrangThai = true
                }
            );


            // 3. Dự án và bộ môn
            var duAn = new DuAn { IdDuAn = duAn1Id, TenDuAn = "Dự Án Test", MoTa = "Mô tả dự án test" };
            var boMon = new QuanLyBoMon { IDBoMon = boMon1Id, TenBoMon = "Bộ Môn Test", MaBoMon = "IT", CoSoHoatDong = "Hòa Lạc" };
            _context.DuAns.Add(duAn);
            _context.QuanLyBoMons.Add(boMon);

            // 4. Nhóm xưởng với navigation đã gán
            var nhomA = new NhomXuong
            {
                TenNhomXuong = "Nhóm Xưởng A",
                IdPhuTrachXuong = _giangVien1Id,
                IdDuAn = duAn.IdDuAn,
                DuAn = duAn,
                IdBoMon = boMon.IDBoMon,
                QuanLyBoMon = boMon,
                MoTa = "Mô tả nhóm A"
            };

            var nhomB = new NhomXuong
            {
                TenNhomXuong = "Nhóm Xưởng B",
                IdPhuTrachXuong = _giangVien1Id,
                MoTa = "Mô tả nhóm B"
            };

            var nhomC = new NhomXuong
            {
                TenNhomXuong = "Nhóm Xưởng C",
                IdPhuTrachXuong = _giangVien2Id,
                MoTa = "Mô tả nhóm C"
            };

            _context.NhomXuongs.AddRange(nhomA, nhomB, nhomC);
            _context.SaveChanges();

            _controller = new NhomXuongCuaToiController(_context);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal()
                }
            };
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SetUserClaims(string email)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, email) }, "mock"));
            _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = user } };
        }

        #region Test Cases

        [Test]
        public async Task GetNhomXuong_WithValidQueryEmail_ReturnsCorrectGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(GiangVien1EmailFE) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(2, nhomXuongs.Count);
            Assert.IsTrue(nhomXuongs.All(n => n.IdPhuTrachXuong == _giangVien1Id));
        }

        [Test]
        public async Task GetNhomXuong_WithNonExistentQueryEmail_ReturnsAllGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(NonExistentEmail) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(3, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_WithNullQueryEmail_ReturnsAllGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(3, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_WithEmptyQueryEmail_ReturnsAllGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(string.Empty) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(3, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_WithValidClaimEmail_ReturnsCorrectGroups()
        {
            SetUserClaims(GiangVien1EmailFE);
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(2, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_WithClaimAndQueryEmail_PrioritizesClaimEmail()
        {
            SetUserClaims(GiangVien1EmailFE);
            var result = await _controller.GetNhomXuongCuaToi(NonExistentEmail) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(2, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_WithNonExistentClaimEmail_ReturnsAllGroups()
        {
            SetUserClaims(NonExistentEmail);
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(3, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_ForLecturerWithNoGroups_ReturnsEmptyList()
        {
            var gv3 = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "Giảng Viên 3",
                MaNhanVien = "GV03",
                EmailFE = "thaygv3@fe.edu.vn",
                EmailFPT = "thaygv3@fpt.com"
            };

            var vaiTroGv3 = new VaiTroNhanVien
            {
                IdVaiTro = Guid.NewGuid(), // 🔧 THÊM DÒNG NÀY
                IdNhanVien = gv3.IdNhanVien,
                TrangThai = true
            };

            _context.Add(gv3);
            _context.Add(vaiTroGv3);
            await _context.SaveChangesAsync();

            var result = await _controller.GetNhomXuongCuaToi("thaygv3@fe.edu.vn") as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;

            Assert.IsEmpty(nhomXuongs);
        }


        [Test]
        public async Task GetNhomXuong_Always_ReturnsOkObjectResult()
        {
            var result = await _controller.GetNhomXuongCuaToi(null);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetNhomXuong_Always_ReturnsValueAsListOfNhomXuong()
        {
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            Assert.IsInstanceOf<List<NhomXuong>>(result.Value);
        }

        [Test]
        public async Task GetNhomXuong_IncludesDuAnDataCorrectly()
        {
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            var nhomCoDuAn = nhomXuongs.FirstOrDefault(n => n.TenNhomXuong == "Nhóm Xưởng A");
            Assert.IsNotNull(nhomCoDuAn.DuAn);
            Assert.AreEqual("Dự Án Test", nhomCoDuAn.DuAn.TenDuAn);
        }

        [Test]
        public async Task GetNhomXuong_IncludesQuanLyBoMonDataCorrectly()
        {
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            var nhomCoBoMon = nhomXuongs.FirstOrDefault(n => n.TenNhomXuong == "Nhóm Xưởng A");
            Assert.IsNotNull(nhomCoBoMon.QuanLyBoMon);
            Assert.AreEqual("Bộ Môn Test", nhomCoBoMon.QuanLyBoMon.TenBoMon);
        }

        [Test]
        public async Task GetNhomXuong_WhenDatabaseIsEmpty_ReturnsEmptyList()
        {
            // Tạo context và controller mới với DB rỗng
            var emptyOptions = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            using var emptyContext = new ModuleDiemDanhDbContext(emptyOptions);
            var emptyController = new NhomXuongCuaToiController(emptyContext);

            // THÊM ĐOẠN NÀY:
            // Gán ControllerContext cho controller mới này để User không bị null
            emptyController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() }
            };

            var result = await emptyController.GetNhomXuongCuaToi("any@email.com") as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.IsEmpty(nhomXuongs);
        }

        [Test]
        public async Task GetNhomXuong_ForInactiveLecturerRole_ReturnsAllGroups()
        {
            var vaiTro = await _context.VaiTroNhanViens.FirstAsync(v => v.IdNhanVien == _giangVien1Id);
            vaiTro.TrangThai = false;
            await _context.SaveChangesAsync();

            var result = await _controller.GetNhomXuongCuaToi(GiangVien1EmailFE) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(3, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_ForSecondLecturer_ReturnsCorrectGroup()
        {
            var result = await _controller.GetNhomXuongCuaToi("thaygv2@fe.edu.vn") as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(1, nhomXuongs.Count);
            Assert.AreEqual("Nhóm Xưởng C", nhomXuongs.First().TenNhomXuong);
        }

        // Test case quan trọng: Chứng minh controller đã sửa lỗi
        //[Test]
        //public async Task GetNhomXuong_WithOrphanedVaiTroNhanVien_DoesNotCrashAndReturnsAll()
        //{
        //    // Tạo một vai trò "mồ côi" không có giảng viên
        //    _context.VaiTroNhanViens.Add(new VaiTroNhanVien { IdNhanVien = null, TrangThai = true });
        //    await _context.SaveChangesAsync();

        //    // Gọi API với một email không liên quan, để nó duyệt qua vai trò mồ côi
        //    var result = await _controller.GetNhomXuongCuaToi(NonExistentEmail) as OkObjectResult;
        //    var nhomXuongs = result.Value as List<NhomXuong>;

        //    // Mong đợi nó không bị crash và trả về tất cả nhóm xưởng
        //    Assert.AreEqual(3, nhomXuongs.Count);
        //}

        [Test]
        public async Task GetNhomXuong_WithValidFptEmail_ReturnsCorrectGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(GiangVien1EmailFPT) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(2, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_WithCaseInsensitiveEmail_ReturnsCorrectGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(GiangVien1EmailFE.ToUpper()) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(2, nhomXuongs.Count);
        }

        [Test]
        public async Task GetNhomXuong_ResultForLecturer1_DoesNotContainLecturer2Groups()
        {
            var result = await _controller.GetNhomXuongCuaToi(GiangVien1EmailFE) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.IsFalse(nhomXuongs.Any(n => n.TenNhomXuong == "Nhóm Xưởng C"));
        }

        [Test]
        public async Task GetNhomXuong_WithNoClaimsAndNoQueryEmail_ReturnsAllGroups()
        {
            var result = await _controller.GetNhomXuongCuaToi(null) as OkObjectResult;
            var nhomXuongs = result.Value as List<NhomXuong>;
            Assert.AreEqual(3, nhomXuongs.Count);
        }

        #endregion
    }
}