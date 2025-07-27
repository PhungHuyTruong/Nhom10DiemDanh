using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuleDiemDanhTest
{
    public class PhuTrachXuongsControllerTest
    {
        private ModuleDiemDanhDbContext _context;
        private PhuTrachXuongsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);

            // Seed dữ liệu Cơ sở và Vai trò
            var coSo = new CoSo { IdCoSo = Guid.NewGuid(), TenCoSo = "Cơ sở 1" };
            var vaiTro = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Quản lý" };

            _context.CoSos.Add(coSo);
            _context.VaiTros.Add(vaiTro);
            _context.SaveChanges();

            _controller = new PhuTrachXuongsController(_context);
        }

        [Test]
        public async Task Create_ReturnsOkResult_WhenValidModel()
        {
            var dto = new PhuTrachXuongDto
            {
                TenNhanVien = "Nguyen Van A",
                MaNhanVien = "NV01",
                EmailFE = "nv.a@fpt.edu.vn",
                EmailFPT = "nva@fpt.edu.vn",
                IdCoSo = _context.CoSos.First().IdCoSo,
                IdVaiTros = new List<Guid> { _context.VaiTros.First().IdVaiTro }
            };

            var result = await _controller.Create(dto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(_context.PhuTrachXuongs.Any(x => x.MaNhanVien == "NV01"));
        }

        [Test]
        public async Task GetAll_ReturnsListPhuTrachXuongDto()
        {
            // Tạo dữ liệu trước
            await Create_ReturnsOkResult_WhenValidModel();

            var result = await _controller.GetAll() as OkObjectResult;
            Assert.IsNotNull(result);

            var list = result.Value as List<PhuTrachXuongDto>;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public async Task GetById_ReturnsCorrectPhuTrachXuong()
        {
            // Tạo NV
            await Create_ReturnsOkResult_WhenValidModel();
            var nv = _context.PhuTrachXuongs.First();

            var result = await _controller.GetById(nv.IdNhanVien) as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as PhuTrachXuong;
            Assert.AreEqual(nv.IdNhanVien, data.IdNhanVien);
        }

        [Test]
        public async Task ToggleStatus_ShouldChangeTrangThai()
        {
            await Create_ReturnsOkResult_WhenValidModel();
            var nv = _context.PhuTrachXuongs.First();

            var oldTrangThai = nv.TrangThai;

            var result = await _controller.ToggleStatus(nv.IdNhanVien) as OkObjectResult;
            Assert.IsNotNull(result);

            var updated = await _context.PhuTrachXuongs.FindAsync(nv.IdNhanVien);
            Assert.AreNotEqual(oldTrangThai, updated.TrangThai);
        }

        [Test]
        public async Task Update_ShouldUpdatePhuTrachXuong()
        {
            await Create_ReturnsOkResult_WhenValidModel();
            var nv = _context.PhuTrachXuongs.First();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "Nguyen Van B",
                MaNhanVien = "NV02",
                EmailFE = "nv.b@fpt.edu.vn",
                EmailFPT = "nvb@fpt.edu.vn",
                IdCoSo = nv.IdCoSo,
                IdVaiTros = new List<Guid> { _context.VaiTros.First().IdVaiTro },
                TrangThai = false
            };

            var result = await _controller.Update(nv.IdNhanVien, model) as OkResult;
            Assert.IsNotNull(result);

            var updated = await _context.PhuTrachXuongs.FindAsync(nv.IdNhanVien);
            Assert.AreEqual("Nguyen Van B", updated.TenNhanVien);
            Assert.IsFalse(updated.TrangThai);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}