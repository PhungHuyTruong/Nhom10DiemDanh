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
        private Guid _testCaHoc1Id;
        private Guid _testKhnxId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);

            _testNhanVienId = Guid.NewGuid();
            _testCaHoc1Id = Guid.NewGuid();
            _testKhnxId = Guid.NewGuid();
            var idCoSo = Guid.NewGuid();
            var idDuAn = Guid.NewGuid();
            var idNhomXuong = Guid.NewGuid();
            var idKeHoach = Guid.NewGuid();

            var caHoc = new CaHoc { IdCaHoc = _testCaHoc1Id, TenCaHoc = "Ca 1", ThoiGianBatDau = TimeSpan.FromHours(8), ThoiGianKetThuc = TimeSpan.FromHours(10), TrangThai = 1 };
            var coSo = new CoSo { IdCoSo = idCoSo, TenCoSo = "Cơ sở A", MaCoSo = "CSHN01", DiaChi = "Hà Nội", Email = "test@example.com", SDT = "0987654321" };
            var diaDiem = new DiaDiem { IdDiaDiem = Guid.NewGuid(), TenDiaDiem = "Phòng A", IdCoSo = idCoSo };
            var duAn = new DuAn { IdDuAn = idDuAn, TenDuAn = "Dự án A", MoTa = "Mô tả cho dự án A" };
            var nhanVien = new PhuTrachXuong { IdNhanVien = _testNhanVienId, IdCoSo = idCoSo, MaNhanVien = "NV001", TenNhanVien = "Nguyễn Văn Test", EmailFE = "testnv.fe@fpt.edu.vn", EmailFPT = "testnv@fpt.com" };
            var nhomXuong = new NhomXuong { IdNhomXuong = idNhomXuong, IdPhuTrachXuong = _testNhanVienId, TenNhomXuong = "Nhóm A", MoTa = "Mô tả cho Nhóm A" };
            var keHoach = new KeHoach { IdKeHoach = idKeHoach, IdDuAn = idDuAn, TenKeHoach = "Kế hoạch Test", NoiDung = "Nội dung cho kế hoạch Test", ThoiGianBatDau = DateTime.Today, ThoiGianKetThuc = DateTime.Today.AddMonths(3) };
            var keHoachNhomXuong = new KeHoachNhomXuong { IdKHNX = _testKhnxId, IdNhomXuong = nhomXuong.IdNhomXuong, IdKeHoach = keHoach.IdKeHoach, SoSinhVien = 20, TrangThai = 1 };
            var caHocEntry = new KHNXCaHoc { IdKHNX = keHoachNhomXuong.IdKHNX, IdCaHoc = caHoc.IdCaHoc, NgayHoc = DateTime.Today, ThoiGian = "08:00 - 10:00", DiemDanhTre = "false", LinkOnline = "", Buoi = "Sáng", NoiDung = "Nội dung buổi học test" };

            _context.CaHocs.Add(caHoc);
            _context.CoSos.Add(coSo);
            _context.DiaDiems.Add(diaDiem);
            _context.DuAns.Add(duAn);
            _context.PhuTrachXuongs.Add(nhanVien);
            _context.NhomXuongs.Add(nhomXuong);
            _context.KeHoachs.Add(keHoach);
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

        private async Task<List<LichGiangDayViewModel>> GetData()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            return (result.Result as OkObjectResult)?.Value as List<LichGiangDayViewModel>;
        }

        #region Test Cases (20)

        [Test]
        public async Task Get_WithValidId_ReturnsOkObjectResult()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task Get_WithInvalidId_ReturnsEmptyList()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(Guid.NewGuid());
            var data = (result.Result as OkObjectResult)?.Value as List<LichGiangDayViewModel>;
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task Get_ReturnsOneRecord_Correctly()
        {
            var data = await GetData();
            Assert.AreEqual(1, data.Count);
        }

        [Test]
        public async Task Get_ReturnsCorrectViewModelType()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            Assert.IsInstanceOf<List<LichGiangDayViewModel>>((result.Result as OkObjectResult)?.Value);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectNhomXuongName()
        {
            var data = await GetData();
            Assert.AreEqual("Nhóm A", data[0].TenNhomXuong);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectDuAnName()
        {
            var data = await GetData();
            Assert.AreEqual("Dự án A", data[0].DuAn);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectCaHocName()
        {
            var data = await GetData();
            Assert.AreEqual("Ca 1", data[0].Ca);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectThoiGian()
        {
            var data = await GetData();
            Assert.AreEqual("08:00 - 10:00", data[0].ThoiGian);
        }

        [Test]
        public async Task Get_HinhThucIsOffline_WhenLinkIsEmpty()
        {
            var data = await GetData();
            Assert.AreEqual("Offline", data[0].HinhThuc);
        }

        [Test]
        public async Task Get_HinhThucIsOnline_WhenLinkExists()
        {
            var ca = await _context.KHNXCaHocs.FirstAsync();
            ca.LinkOnline = "http://example.com";
            await _context.SaveChangesAsync();
            var data = await GetData();
            Assert.AreEqual("Online", data[0].HinhThuc);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectDiaDiem()
        {
            var data = await GetData();
            Assert.AreEqual("Phòng A", data[0].DiaDiem);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectNgayHoc()
        {
            var data = await GetData();
            Assert.AreEqual(DateTime.Today, data[0].NgayHoc.Date);
        }

        [Test]
        public async Task Get_ReturnsResultWithCorrectDiemDanhTre()
        {
            var data = await GetData();
            Assert.AreEqual("false", data[0].DiemDanhTre);
        }

        [Test]
        public async Task Get_ReturnsMultipleRecords_WhenDataExists()
        {
            _context.KHNXCaHocs.Add(new KHNXCaHoc { IdKHNX = _testKhnxId, IdCaHoc = _testCaHoc1Id, NgayHoc = DateTime.Today.AddDays(1), ThoiGian = "08:00 - 10:00", DiemDanhTre = "false", LinkOnline = "", Buoi = "Sáng", NoiDung = "Buổi 2" });
            await _context.SaveChangesAsync();
            var data = await GetData();
            Assert.AreEqual(2, data.Count);
        }

        [Test]
        public async Task Get_ResultIsSortedByNgayHocThenByCa()
        {
            _context.KHNXCaHocs.Add(new KHNXCaHoc { IdKHNX = _testKhnxId, IdCaHoc = _testCaHoc1Id, NgayHoc = DateTime.Today.AddDays(-1), ThoiGian = "10:00 - 12:00", DiemDanhTre = "false", LinkOnline = "", Buoi = "Chiều", NoiDung = "Buổi cũ" });
            await _context.SaveChangesAsync();
            var data = await GetData();
            Assert.AreEqual(DateTime.Today.AddDays(-1), data[0].NgayHoc.Date);
            Assert.AreEqual(DateTime.Today, data[1].NgayHoc.Date);
        }

        [Test]
        public async Task Get_WhenDiaDiemIsMissing_ReturnsMinusSymbol()
        {
            _context.DiaDiems.RemoveRange(await _context.DiaDiems.ToListAsync());
            await _context.SaveChangesAsync();
            var data = await GetData();
            Assert.AreEqual("-", data[0].DiaDiem);
        }

        [Test]
        public async Task Get_WhenNoCaHoc_ReturnsEmptyListWithoutError()
        {
            _context.KHNXCaHocs.RemoveRange(await _context.KHNXCaHocs.ToListAsync());
            await _context.SaveChangesAsync();
            var data = await GetData();
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task Get_ReturnsNotNullValue_WhenResultIsEmpty()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(Guid.NewGuid());
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task Get_ReturnsCorrectActionResultType()
        {
            var result = await _controller.GetLichGiangDayTheoNhanVien(_testNhanVienId);
            Assert.IsInstanceOf<ActionResult<List<LichGiangDayViewModel>>>(result);
        }

        [Test]
        public async Task Get_ReturnsDataForCorrectNhanVienOnly()
        {
            var otherNhanVienId = Guid.NewGuid();
            var otherNhomXuongId = Guid.NewGuid();
            _context.PhuTrachXuongs.Add(new PhuTrachXuong { IdNhanVien = otherNhanVienId, MaNhanVien = "NV002", TenNhanVien = "Người khác", EmailFE = "a@a.com", EmailFPT = "a@a.com" });
            _context.NhomXuongs.Add(new NhomXuong { IdNhomXuong = otherNhomXuongId, IdPhuTrachXuong = otherNhanVienId, TenNhomXuong = "Nhóm B", MoTa = "Mô tả" });
            var otherKhnx = new KeHoachNhomXuong { IdKHNX = Guid.NewGuid(), IdNhomXuong = otherNhomXuongId, IdKeHoach = Guid.NewGuid(), SoSinhVien = 1, TrangThai = 1 };
            _context.KeHoachNhomXuongs.Add(otherKhnx);
            _context.KHNXCaHocs.Add(new KHNXCaHoc { IdKHNX = otherKhnx.IdKHNX, IdCaHoc = _testCaHoc1Id, NgayHoc = DateTime.Today, ThoiGian = "08:00 - 10:00", DiemDanhTre = "false", LinkOnline = "", Buoi = "Sáng", NoiDung = "Buổi học khác" });
            await _context.SaveChangesAsync();

            var data = await GetData();
            Assert.AreEqual(1, data.Count, "Should only return data for the requested employee.");
            Assert.AreEqual("Nhóm A", data[0].TenNhomXuong);
        }

        #endregion
    }
}