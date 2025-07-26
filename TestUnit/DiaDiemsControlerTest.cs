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
    public class DiaDiemsControllerTest
    {
        private ModuleDiemDanhDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task TC001_GetAll_ReturnsAllItems()
        {
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = Guid.NewGuid(), TenDiaDiem = "Địa điểm 1" });
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = Guid.NewGuid(), TenDiaDiem = "Địa điểm 2" });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var result = await controller.GetAll() as OkObjectResult;

            Assert.IsNotNull(result);
            var list = result.Value as List<DiaDiem>;
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public async Task TC002_GetById_Found_ReturnsOk()
        {
            var id = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = id, TenDiaDiem = "A" });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var result = await controller.GetById(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC003_GetById_NotFound_ReturnsNotFound()
        {
            var controller = new DiaDiemController(_context);
            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC004_Create_AddsItemSuccessfully()
        {
            var controller = new DiaDiemController(_context);
            var diaDiem = new DiaDiem { TenDiaDiem = "Test", ViDo = 10, KinhDo = 20, BanKinh = 50, TrangThai = true };

            var result = await controller.Create(diaDiem);

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(1, _context.DiaDiems.Count());
        }

        [Test]
        public async Task TC005_Update_ValidId_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = id, TenDiaDiem = "Old", ViDo = 0, KinhDo = 0, BanKinh = 0, TrangThai = true });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var updated = new DiaDiem { TenDiaDiem = "New", ViDo = 1, KinhDo = 1, BanKinh = 1, TrangThai = false };
            var result = await controller.Update(id, updated);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var saved = await _context.DiaDiems.FindAsync(id);
            Assert.AreEqual("New", saved.TenDiaDiem);
        }

        [Test]
        public async Task TC006_Update_InvalidId_ReturnsNotFound()
        {
            var controller = new DiaDiemController(_context);
            var updated = new DiaDiem { TenDiaDiem = "Nope", ViDo = 1, KinhDo = 1, BanKinh = 1, TrangThai = false };
            var result = await controller.Update(Guid.NewGuid(), updated);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC007_Delete_DeletesSuccessfully()
        {
            var id = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = id, TenDiaDiem = "Del" });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var result = await controller.Delete(id);

            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.IsEmpty(_context.DiaDiems.ToList());
        }

        [Test]
        public async Task TC008_Delete_NotFound_ReturnsNotFound()
        {
            var controller = new DiaDiemController(_context);
            var result = await controller.Delete(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task TC009_DoiTrangThai_TogglesCorrectly()
        {
            var id = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem
            {
                IdDiaDiem = id,
                TenDiaDiem = "Địa điểm A", // 🟢 thêm dòng này để tránh lỗi
                ViDo = 10,
                KinhDo = 20,
                BanKinh = 30,
                TrangThai = true,
                NgayTao = DateTime.Now
            });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var result = await controller.DoiTrangThai(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsFalse(_context.DiaDiems.Find(id).TrangThai);
        }

        [Test]
        public async Task TC010_DoiTrangThai_InvalidId_ReturnsNotFound()
        {
            var controller = new DiaDiemController(_context);
            var result = await controller.DoiTrangThai(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC011_Create_SetsNgayTao()
        {
            var controller = new DiaDiemController(_context);
            var diaDiem = new DiaDiem { TenDiaDiem = "CheckDate" };

            await controller.Create(diaDiem);
            var saved = _context.DiaDiems.First();

            Assert.That(saved.NgayTao, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public async Task TC012_Update_SetsNgayCapNhat()
        {
            var id = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = id, TenDiaDiem = "A", TrangThai = true });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var updated = new DiaDiem { TenDiaDiem = "B", TrangThai = false };
            await controller.Update(id, updated);
            var saved = _context.DiaDiems.Find(id);

            Assert.That(saved.NgayCapNhat, Is.GreaterThan(DateTime.MinValue));
        }

        [Test]
        public async Task TC013_GetByCoSo_ReturnsList()
        {
            var coSoId = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem { IdDiaDiem = Guid.NewGuid(), IdCoSo = coSoId, TenDiaDiem = "Cơ sở A" });
            await _context.SaveChangesAsync();

            var controller = new DiaDiemController(_context);
            var result = await controller.GetByCoSo(coSoId) as OkObjectResult;

            Assert.IsNotNull(result);
            var list = result.Value as List<DiaDiem>;
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public async Task TC014_GetByCoSo_ReturnsEmpty()
        {
            var controller = new DiaDiemController(_context);
            var result = await controller.GetByCoSo(Guid.NewGuid()) as OkObjectResult;

            Assert.IsNotNull(result);
            var list = result.Value as List<DiaDiem>;
            Assert.IsEmpty(list);
        }

        [Test]
        public async Task TC015_Create_SavesAllFieldsCorrectly()
        {
            var controller = new DiaDiemController(_context);
            var idCoSo = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Full",
                ViDo = 1,
                KinhDo = 2,
                BanKinh = 3,
                TrangThai = true,
                IdCoSo = idCoSo
            };

            await controller.Create(diaDiem);
            var saved = _context.DiaDiems.FirstOrDefault(x => x.TenDiaDiem == "Full");

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, saved.ViDo);
                Assert.AreEqual(2, saved.KinhDo);
                Assert.AreEqual(3, saved.BanKinh);
                Assert.AreEqual(true, saved.TrangThai);
                Assert.AreEqual(idCoSo, saved.IdCoSo);
            });
        }
    }
}
