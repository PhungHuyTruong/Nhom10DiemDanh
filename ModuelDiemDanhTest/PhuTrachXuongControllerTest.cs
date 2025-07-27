using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DtoApi = API.Models.PhuTrachXuongDto;

namespace ModuelDiemDanhTest
{
    [TestFixture]
    internal class PhuTrachXuongControllerTest
    {
        private ModuleDiemDanhDbContext _context;
        private PhuTrachXuongsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);

            // Seed dữ liệu
            var coSo = new CoSo { IdCoSo = Guid.NewGuid(), TenCoSo = "Cơ sở 1" };
            var vaiTro = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Quản lý" };

            var nhanVien = new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "Nguyễn Văn A",
                MaNhanVien = "NV001",
                EmailFE = "a@example.com",
                EmailFPT = "a@fpt.edu.vn",
                IdCoSo = coSo.IdCoSo,
                CoSo = coSo,
                NgayTao = DateTime.Now,
                TrangThai = true,
                VaiTroNhanViens = new List<VaiTroNhanVien>()
            };

            var vtnv = new VaiTroNhanVien
            {
                IdVTNV = Guid.NewGuid(),
                IdNhanVien = nhanVien.IdNhanVien,
                IdVaiTro = vaiTro.IdVaiTro,
                VaiTro = vaiTro,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            nhanVien.VaiTroNhanViens.Add(vtnv);

            _context.CoSos.Add(coSo);
            _context.VaiTros.Add(vaiTro);
            _context.PhuTrachXuongs.Add(nhanVien);
            _context.VaiTroNhanViens.Add(vtnv);
            _context.SaveChanges();

            _controller = new PhuTrachXuongsController(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsCorrectData()
        {
            var result = await _controller.GetAll() as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as List<DtoApi>;
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count);

            var dto = data.First();
            Assert.AreEqual("NV001", dto.MaNhanVien);
            Assert.AreEqual("Nguyễn Văn A", dto.TenNhanVien);
            Assert.AreEqual("Cơ sở 1", dto.TenCoSo);
            Assert.IsTrue(dto.TenVaiTros.Contains("Quản lý"));
        }

        [Test]
        public async Task GetById_ValidId_ReturnsPhuTrachXuong()
        {
            var existing = _context.PhuTrachXuongs.First();

            var result = await _controller.GetById(existing.IdNhanVien) as OkObjectResult;

            Assert.IsNotNull(result);
            var entity = result.Value as PhuTrachXuong;
            Assert.AreEqual("Nguyễn Văn A", entity.TenNhanVien);
        }

        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_ValidModel_AddsNewPhuTrachXuong()
        {
            var dto = new DtoApi
            {
                TenNhanVien = "Nguyễn Văn B",
                MaNhanVien = "NV002",
                EmailFE = "b@example.com",
                EmailFPT = "b@fpt.edu.vn",
                IdCoSo = _context.CoSos.First().IdCoSo,
                IdVaiTros = new List<Guid> { _context.VaiTros.First().IdVaiTro }
            };

            var result = await _controller.Create(dto);
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var created = _context.PhuTrachXuongs.FirstOrDefault(x => x.MaNhanVien == "NV002");
            Assert.IsNotNull(created);
            Assert.AreEqual("Nguyễn Văn B", created.TenNhanVien);
        }

        [Test]
        public async Task Update_ValidId_UpdatesPhuTrachXuong()
        {
            var existing = _context.PhuTrachXuongs.Include(x => x.VaiTroNhanViens).First();

            var dto = new DtoApi
            {
                TenNhanVien = "Cập nhật",
                MaNhanVien = "UPDATED001",
                EmailFE = "updated@example.com",
                EmailFPT = "updated@fpt.edu.vn",
                IdCoSo = _context.CoSos.First().IdCoSo,
                IdVaiTros = new List<Guid> { _context.VaiTros.First().IdVaiTro },
                TrangThai = false
            };

            var result = await _controller.Update(existing.IdNhanVien, dto);
            Assert.IsInstanceOf<OkResult>(result);

            var updated = await _context.PhuTrachXuongs.FindAsync(existing.IdNhanVien);
            Assert.AreEqual("Cập nhật", updated.TenNhanVien);
            Assert.IsFalse(updated.TrangThai);
        }

        [Test]
        public async Task ToggleStatus_ValidId_ChangesTrangThai()
        {
            var nv = _context.PhuTrachXuongs.First();
            bool originalStatus = nv.TrangThai;

            var result = await _controller.ToggleStatus(nv.IdNhanVien);
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var updated = await _context.PhuTrachXuongs.FindAsync(nv.IdNhanVien);
            Assert.AreEqual(!originalStatus, updated.TrangThai);
        }

        [Test]
        public async Task GetCoSoList_ReturnsList()
        {
            var result = await _controller.GetCoSoList() as OkObjectResult;
            Assert.IsNotNull(result);
            var list = result.Value as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public async Task GetVaiTroList_ReturnsList()
        {
            var result = await _controller.GetVaiTroList() as OkObjectResult;
            Assert.IsNotNull(result);
            var list = result.Value as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
        }
    }
}
