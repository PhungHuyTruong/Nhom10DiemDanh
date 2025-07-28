using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore.InMemory;
using ClosedXML.Excel;

namespace TestSinhVien
{
    [TestFixture]
    public class LichSuDiemDanhControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private LichSuDiemDanhController _controller;
        private Guid _sinhVienId;
        private Guid _hocKyId;
        private Guid _nhomXuongId;

        [SetUp]
        public void Setup()
        {
            // Tạo database in-memory cho testing
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);
            _controller = new LichSuDiemDanhController(_context);

            // Khởi tạo test data
            _sinhVienId = Guid.NewGuid();
            _hocKyId = Guid.NewGuid();
            _nhomXuongId = Guid.NewGuid();

            // Tạo dữ liệu mẫu
            SeedTestData();
        }

        private void SeedTestData()
        {
            var hocKy = new HocKy { IdHocKy = _hocKyId, TenHocKy = "HK1 2023-2024" };
            var duAn = new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "Dự án test", IdHocKy = _hocKyId, HocKy = hocKy };
            var keHoach = new KeHoach { IdKeHoach = Guid.NewGuid(), TenKeHoach = "KH Test", IdDuAn = duAn.IdDuAn, DuAn = duAn };
            var nhomXuong = new NhomXuong { IdNhomXuong = _nhomXuongId, TenNhomXuong = "Nhóm 1" };
            var keHoachNhomXuong = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = _nhomXuongId,
                KeHoach = keHoach,
                NhomXuong = nhomXuong
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
                IdKHNX = keHoachNhomXuong.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Now.Date,
                NoiDung = "Bài test",
                DiemDanhTre = "15",
                CaHoc = caHoc,
                KeHoachNhomXuong = keHoachNhomXuong
            };

            var diemDanh = new DiemDanh
            {
                IdDiemDanh = Guid.NewGuid(),
                IdSinhVien = _sinhVienId,
                IdCaHoc = caHoc.IdCaHoc,
                IdNhomXuong = _nhomXuongId
            };

            var lichSuDiemDanh = new LichSuDiemDanh
            {
                IdLSDD = Guid.NewGuid(),
                IdDiemDanh = diemDanh.IdDiemDanh,
                IdNXCH = khnxCaHoc.IdNXCH,
                ThoiGianDiemDanh = DateTime.Now,
                NoiDungBuoiHoc = "Test nội dung",
                HinhThuc = "Trực tiếp",
                DiaDiem = "Phòng A1",
                GhiChu = "Ghi chú test",
                TrangThai = 3,
                TrangThaiDuyet = 1,
                DiemDanh = diemDanh,
                KHNXCaHoc = khnxCaHoc
            };

            _context.HocKys.Add(hocKy);
            _context.DuAns.Add(duAn);
            _context.KeHoachs.Add(keHoach);
            _context.NhomXuongs.Add(nhomXuong);
            _context.KeHoachNhomXuongs.Add(keHoachNhomXuong);
            _context.CaHocs.Add(caHoc);
            _context.KHNXCaHocs.Add(khnxCaHoc);
            _context.DiemDanhs.Add(diemDanh);
            _context.LichSuDiemDanhs.Add(lichSuDiemDanh);
            _context.SaveChanges();
        }

        //[Test]
        //public async Task GetLichSuDiemDanh_WithValidData_ReturnsOkResult()
        //{
        //    // Act
        //    var result = await _controller.GetLichSuDiemDanh(_sinhVienId, _hocKyId, _nhomXuongId);

        //    // Assert
        //    Assert.That(result, Is.TypeOf<OkObjectResult>());
        //    var okResult = result as OkObjectResult;
        //    Assert.That(okResult.Value, Is.Not.Null);
        //    var items = okResult.Value as IEnumerable<object>;
        //    Assert.That(items, Is.Not.Empty);
        //}

        //[Test]
        //public async Task GetLichSuDiemDanh_WithInvalidSinhVienId_ReturnsEmptyList()
        //{
        //    // Arrange
        //    var invalidSinhVienId = Guid.NewGuid();

        //    // Act
        //    var result = await _controller.GetLichSuDiemDanh(invalidSinhVienId, _hocKyId, _nhomXuongId);

        //    // Assert
        //    Assert.That(result, Is.TypeOf<OkObjectResult>());
        //    var okResult = result as OkObjectResult;
        //    var items = okResult.Value as IEnumerable<object>;
        //    Assert.That(items, Is.Empty);
        //}

        //[Test]
        //public async Task GetLichSuDiemDanh_WithoutFilters_ReturnsAllRecords()
        //{
        //    // Act
        //    var result = await _controller.GetLichSuDiemDanh(_sinhVienId, null, null);

        //    // Assert
        //    Assert.That(result, Is.TypeOf<OkObjectResult>());
        //    var okResult = result as OkObjectResult;
        //    Assert.That(okResult.Value, Is.Not.Null);
        //    var items = okResult.Value as IEnumerable<object>;
        //    Assert.That(items, Is.Not.Empty);
        //}

        //[Test]
        //public async Task DownloadTemplate_WithValidData_ReturnsFileResult()
        //{
        //    // Act
        //    var result = await _controller.DownloadTemplate(_sinhVienId, _hocKyId, _nhomXuongId);

        //    // Assert
        //    Assert.That(result, Is.TypeOf<FileContentResult>());
        //    var fileResult = result as FileContentResult;
        //    Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        //    Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        //}

        //[Test]
        //public async Task DownloadTemplate_WithInvalidData_ReturnsEmptyFile()
        //{
        //    // Arrange
        //    var invalidSinhVienId = Guid.NewGuid();

        //    // Act
        //    var result = await _controller.DownloadTemplate(invalidSinhVienId, _hocKyId, _nhomXuongId);

        //    // Assert
        //    Assert.That(result, Is.TypeOf<FileContentResult>());
        //    var fileResult = result as FileContentResult;
        //    Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        //    Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        //}

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}