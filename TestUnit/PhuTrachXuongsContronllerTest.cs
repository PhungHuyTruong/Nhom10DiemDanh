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

namespace TestUnit
{
    [TestFixture]
    public class PhuTrachXuongsContronllerTest
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
            _controller = new PhuTrachXuongsController(_context);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task TC001_GetAll_ReturnsOk()
        {
            _context.PhuTrachXuongs.Add(new PhuTrachXuong
            {
                IdNhanVien = Guid.NewGuid(),
                TenNhanVien = "Test NV",
                MaNhanVien = "NV001",
                EmailFE = "testfe@example.com",
                EmailFPT = "testfpt@example.com",
                IdCoSo = Guid.NewGuid(),
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC002_GetById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();

            _context.PhuTrachXuongs.Add(new PhuTrachXuong
            {
                IdNhanVien = id,
                TenNhanVien = "Nguyễn Văn B",
                MaNhanVien = "NVB001",
                EmailFE = "nvb.fe@example.com",
                EmailFPT = "nvb.fpt@example.com",
                IdCoSo = Guid.NewGuid(),
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var result = await _controller.GetById(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC003_GetById_ReturnsNotFound_WhenNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC004_Create_ReturnsRedirect()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở CS1",
                DiaChi = "123 Đường ABC",
                Email = "coso1@example.com",
                MaCoSo = "CS001",
                SDT = "0123456789",
                TrangThai = true
            });

            var roleId = Guid.NewGuid();
            _context.VaiTros.Add(new VaiTro
            {
                IdVaiTro = roleId,
                TenVaiTro = "Role1",
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "NV1",
                MaNhanVien = "MN01",
                EmailFE = "fe@example.com",
                EmailFPT = "fpt@example.com",
                IdCoSo = coSoId,
                IdVaiTros = new List<Guid> { roleId }
            };

            var result = await _controller.Create(model);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }


        [Test]
        public async Task TC005_Create_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.Create(null);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task TC006_Update_ReturnsNotFound_WhenNotExist()
        {
            var result = await _controller.Update(Guid.NewGuid(), new PhuTrachXuongDto());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC007_Update_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var coSoId = Guid.NewGuid();

            _context.PhuTrachXuongs.Add(new PhuTrachXuong
            {
                IdNhanVien = id,
                TenNhanVien = "Old",
                MaNhanVien = "MN001",
                EmailFE = "oldfe@example.com",
                EmailFPT = "oldfpt@example.com",
                IdCoSo = coSoId,
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "Updated",
                MaNhanVien = "MN002",
                EmailFE = "updatedfe@example.com",
                EmailFPT = "updatedfpt@example.com",
                IdCoSo = coSoId,
                TrangThai = true,
                IdVaiTros = new List<Guid>()
            };

            var result = await _controller.Update(id, model);
            Assert.IsInstanceOf<OkResult>(result);
        }


        [Test]
        public async Task TC008_ToggleStatus_TogglesAndRedirects()
        {
            var id = Guid.NewGuid();

            _context.PhuTrachXuongs.Add(new PhuTrachXuong
            {
                IdNhanVien = id,
                TenNhanVien = "Test NV",
                MaNhanVien = "NV999",
                EmailFE = "fe@example.com",
                EmailFPT = "fpt@example.com",
                IdCoSo = Guid.NewGuid(),
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var result = await _controller.ToggleStatus(id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }


        [Test]
        public async Task TC009_ToggleStatus_ReturnsNotFound_WhenNotExist()
        {
            var result = await _controller.ToggleStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC010_GetCoSoList_ReturnsOk()
        {
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = Guid.NewGuid(),
                TenCoSo = "Cơ Sở Test",
                DiaChi = "789 Nguyễn Huệ",
                Email = "coso_test@example.com",
                MaCoSo = "CS003",
                SDT = "0912345678",
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var result = await _controller.GetCoSoList();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC011_GetVaiTroList_ReturnsOk()
        {
            _context.VaiTros.Add(new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "TestRole" });
            await _context.SaveChangesAsync();
            var result = await _controller.GetVaiTroList();
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC012_Create_AddsVaiTrosCorrectly()
        {
            var coSoId = Guid.NewGuid();
            var vaiTroId = Guid.NewGuid();

            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "CoSo Test",
                DiaChi = "456 Trần Hưng Đạo",
                Email = "testcoso@example.com",
                MaCoSo = "CS002",
                SDT = "0987654321",
                TrangThai = true
            });

            _context.VaiTros.Add(new VaiTro
            {
                IdVaiTro = vaiTroId,
                TenVaiTro = "VaiTro Test",
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "Test NV",
                MaNhanVien = "NV123",
                EmailFE = "testfe@example.com",
                EmailFPT = "testfpt@example.com",
                IdCoSo = coSoId,
                IdVaiTros = new List<Guid> { vaiTroId }
            };

            await _controller.Create(model);

            var count = _context.VaiTroNhanViens.Count();
            Assert.AreEqual(1, count);
        }


        [Test]
        public async Task TC013_Update_ReplaceVaiTrosCorrectly()
        {
            var nvId = Guid.NewGuid();
            var oldRoleId = Guid.NewGuid();
            var newRoleId = Guid.NewGuid();
            var coSoId = Guid.NewGuid();

            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "CoSo A",
                DiaChi = "123 Test St",
                Email = "coso@example.com",
                MaCoSo = "CSUPD",
                SDT = "0909090909",
                TrangThai = true
            });

            _context.VaiTros.Add(new VaiTro
            {
                IdVaiTro = oldRoleId,
                TenVaiTro = "Vai Trò Cũ",
                TrangThai = true
            });

            _context.VaiTros.Add(new VaiTro
            {
                IdVaiTro = newRoleId,
                TenVaiTro = "Vai Trò Mới",
                TrangThai = true
            });

            _context.PhuTrachXuongs.Add(new PhuTrachXuong
            {
                IdNhanVien = nvId,
                TenNhanVien = "Test User",
                MaNhanVien = "NV000",
                EmailFE = "oldfe@example.com",
                EmailFPT = "oldfpt@example.com",
                IdCoSo = coSoId,
                TrangThai = true
            });

            _context.VaiTroNhanViens.Add(new VaiTroNhanVien
            {
                IdVTNV = Guid.NewGuid(),
                IdNhanVien = nvId,
                IdVaiTro = oldRoleId,
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "Updated",
                MaNhanVien = "NV000",
                EmailFE = "updatedfe@example.com",
                EmailFPT = "updatedfpt@example.com",
                IdCoSo = coSoId,
                TrangThai = true,
                IdVaiTros = new List<Guid> { newRoleId }
            };

            await _controller.Update(nvId, model);

            var newCount = _context.VaiTroNhanViens.Count(v => v.IdVaiTro == newRoleId);
            Assert.AreEqual(1, newCount);
        }


        [Test]
        public async Task TC014_Create_SavesPhuTrachXuong()
        {
            var coSoId = Guid.NewGuid();

            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "CoSo A",
                DiaChi = "123 Lê Lợi",
                Email = "coso@example.com",
                MaCoSo = "CS001",
                SDT = "0123456789",
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "Nguyen Van A",
                MaNhanVien = "NV001",
                EmailFE = "nv.a@fe.com",
                EmailFPT = "nv.a@fpt.com",
                IdCoSo = coSoId,
                IdVaiTros = new List<Guid>()
            };

            await _controller.Create(model);

            var exists = _context.PhuTrachXuongs.Any(x => x.MaNhanVien == "NV001");
            Assert.IsTrue(exists);
        }


        [Test]
        public async Task TC015_Update_ChangesEmailFields()
        {
            var id = Guid.NewGuid();
            var coSoId = Guid.NewGuid();

            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "Cơ sở cập nhật",
                DiaChi = "123 Test St",
                Email = "coso@example.com",
                MaCoSo = "CSUPDATE",
                SDT = "0909090909",
                TrangThai = true
            });

            _context.PhuTrachXuongs.Add(new PhuTrachXuong
            {
                IdNhanVien = id,
                MaNhanVien = "NV999",
                TenNhanVien = "Nguyễn Văn Cũ",
                EmailFPT = "old@fpt.com",
                EmailFE = "old@fe.com",
                IdCoSo = coSoId,
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "Nguyễn Văn Mới",
                MaNhanVien = "NV999", // giữ nguyên
                EmailFE = "new@fe.com",
                EmailFPT = "new@fpt.com",
                IdCoSo = coSoId,
                IdVaiTros = new List<Guid>(),
                TrangThai = true
            };

            await _controller.Update(id, model);

            var updated = _context.PhuTrachXuongs.Find(id);
            Assert.AreEqual("new@fpt.com", updated.EmailFPT);
            Assert.AreEqual("new@fe.com", updated.EmailFE);
            Assert.AreEqual("Nguyễn Văn Mới", updated.TenNhanVien);
        }

    }
}
