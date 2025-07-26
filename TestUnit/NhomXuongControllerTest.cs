using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestUnit
{
    [TestFixture]
    public class NhomXuongControllerTest
    {
        private DbContextOptions<ModuleDiemDanhDbContext> _options;
        private ModuleDiemDanhDbContext _context;
        private NhomXuongsController _controller;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(_options);
            _controller = new NhomXuongsController(_context);
        }

        [Test]
        public async Task TC001_Create_ReturnsRedirect()
        {
            var model = new NhomXuong
            {
                TenNhomXuong = "Nhóm 1",
                MoTa = "Mô tả",
                TrangThai = 1
            };

            var result = await _controller.Create(model);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task TC002_Create_Allows_EmptyIdDuAn()
        {
            var model = new NhomXuong
            {
                TenNhomXuong = "Test thiếu dự án",
                MoTa = "Mô tả",
                IdDuAn = Guid.Empty,
                IdBoMon = Guid.NewGuid(),
                IdPhuTrachXuong = Guid.NewGuid()
            };

            var result = await _controller.Create(model);
            var exists = _context.NhomXuongs.Any(x => x.TenNhomXuong == "Test thiếu dự án");

            Assert.IsTrue(exists); // Đổi lại để phản ánh đúng logic controller
        }





        [Test]
        public async Task TC003_Create_Fails_WhenMissingFields()
        {
            // Arrange
            var invalidModel = new NhomXuong
            {
                // Bỏ qua TenNhomXuong để gây lỗi
                MoTa = "Không có tên"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _controller.Create(invalidModel);
            });

            Assert.That(ex, Is.Not.Null);
        }


        [Test]
        public async Task TC004_Create_Fails_WhenMissingMoTa()
        {
            // Arrange
            var model = new NhomXuong
            {
                TenNhomXuong = "Nhóm A",
                // Bỏ qua MoTa để gây lỗi
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _controller.Create(model);
            });

            Assert.That(ex, Is.Not.Null);
        }



        [Test]
        public async Task TC005_GetById_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC006_Update_Success()
        {
            var id = Guid.NewGuid();
            _context.NhomXuongs.Add(new NhomXuong
            {
                IdNhomXuong = id,
                TenNhomXuong = "Old",
                MoTa = "Cũ",
                TrangThai = 1
            });
            await _context.SaveChangesAsync();

            var updated = new NhomXuong
            {
                TenNhomXuong = "Updated",
                MoTa = "Mô tả mới", // ✅ Bổ sung dòng này
                TrangThai = 1
            };

            var result = await _controller.Update(id, updated);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC007_Update_ReturnsNotFound()
        {
            var model = new NhomXuong { TenNhomXuong = "UpdateFail" };
            var result = await _controller.Update(Guid.NewGuid(), model);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC008_ToggleStatus_ActiveToInactive()
        {
            var id = Guid.NewGuid();
            _context.NhomXuongs.Add(new NhomXuong
            {
                IdNhomXuong = id,
                TenNhomXuong = "Toggle",
                MoTa = "Mô tả Toggle", // ✅ Bổ sung dòng này
                TrangThai = 1
            });
            await _context.SaveChangesAsync();

            var result = await _controller.ToggleStatus(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC009_ToggleStatus_NotFound()
        {
            var result = await _controller.ToggleStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TC010_DownloadData_ReturnsFile()
        {
            _context.NhomXuongs.Add(new NhomXuong
            {
                IdNhomXuong = Guid.NewGuid(),
                TenNhomXuong = "Download",
                MoTa = "Test mô tả",        // ✅ Bổ sung dòng này
                TrangThai = 1,
                NgayTao = DateTime.Now
            });
            _context.SaveChanges();

            var result = _controller.DownloadData();
            Assert.IsInstanceOf<FileContentResult>(result);
        }


        [Test]
        public async Task TC011_ImportExcel_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.ImportExcel(null);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TC012_ImportExcel_ReturnsBadRequest_WhenEmpty()
        {
            var emptyFile = new FormFile(Stream.Null, 0, 0, "Data", "fake.xlsx");
            var result = await _controller.ImportExcel(emptyFile);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TC013_ImportExcel_InvalidContent()
        {
            var content = new MemoryStream();
            await content.WriteAsync(new byte[] { 0x00 });
            content.Position = 0;

            var file = new FormFile(content, 0, content.Length, "file", "test.xlsx");

            // Cách 1: Bắt exception
            Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _controller.ImportExcel(file);
            });
        }


        [Test]
        public async Task TC014_Create_GenId_TrangThai1()
        {
            var model = new NhomXuong
            {
                TenNhomXuong = "CheckId",
                MoTa = "Mô tả"
            };

            var result = await _controller.Create(model);
            var redirect = result as RedirectToActionResult;

            var created = _context.NhomXuongs.FirstOrDefault(x => x.TenNhomXuong == "CheckId");

            Assert.IsNotNull(created);
            Assert.AreEqual(1, created.TrangThai);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}
