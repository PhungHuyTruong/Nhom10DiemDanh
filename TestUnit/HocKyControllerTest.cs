using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestUnit
{
    [TestFixture]
    public class HocKyControllerTest
    {
        private ModuleDiemDanhDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);
        }

        [Test]
        public async Task TC001_GetAll_ShouldReturnList()
        {
            _context.HocKys.Add(new HocKy { IdHocKy = Guid.NewGuid(), MaHocKy = "HK1", TenHocKy = "Học kỳ 1" });
            await _context.SaveChangesAsync();

            var controller = new HocKyController(_context);
            var result = await controller.GetAll() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC002_GetById_ShouldReturnItem()
        {
            var id = Guid.NewGuid();
            _context.HocKys.Add(new HocKy { IdHocKy = id, MaHocKy = "HK2", TenHocKy = "Học kỳ 2" });
            await _context.SaveChangesAsync();

            var controller = new HocKyController(_context);
            var result = await controller.GetById(id) as OkObjectResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task TC003_GetById_ShouldReturnNotFound()
        {
            var controller = new HocKyController(_context);
            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC004_Create_ShouldRedirect()
        {
            var controller = new HocKyController(_context);
            var model = new HocKy { MaHocKy = "HK3", TenHocKy = "Học kỳ 3" };

            var result = await controller.Create(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task TC005_Create_ShouldSetDefaultTrangThai()
        {
            var controller = new HocKyController(_context);
            var model = new HocKy { MaHocKy = "HK4", TenHocKy = "Học kỳ 4" };

            await controller.Create(model);

            var created = _context.HocKys.FirstOrDefault(x => x.MaHocKy == "HK4");
            Assert.IsTrue(created.TrangThai);
        }

        [Test]
        public async Task TC006_Update_ShouldSuccess()
        {
            var id = Guid.NewGuid();
            _context.HocKys.Add(new HocKy { IdHocKy = id, MaHocKy = "HK5", TenHocKy = "Old", TrangThai = true });
            await _context.SaveChangesAsync();

            var controller = new HocKyController(_context);
            var updateModel = new HocKy { MaHocKy = "HK5U", TenHocKy = "Updated", TrangThai = false };

            var result = await controller.Update(id, updateModel);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC007_Update_ShouldReturnNotFound()
        {
            var controller = new HocKyController(_context);
            var updateModel = new HocKy { MaHocKy = "HKX", TenHocKy = "Something", TrangThai = true };

            var result = await controller.Update(Guid.NewGuid(), updateModel);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC008_DoiTrangThai_ShouldToggle()
        {
            var id = Guid.NewGuid();
            _context.HocKys.Add(new HocKy { IdHocKy = id, MaHocKy = "HK6", TenHocKy = "Toggle", TrangThai = true });
            await _context.SaveChangesAsync();

            var controller = new HocKyController(_context);
            var result = await controller.DoiTrangThai(id);

            var updated = await _context.HocKys.FindAsync(id);
            Assert.IsFalse(updated.TrangThai);
        }

        [Test]
        public async Task TC009_DoiTrangThai_ShouldReturnNotFound()
        {
            var controller = new HocKyController(_context);
            var result = await controller.DoiTrangThai(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC010_Create_MissingTenHocKy_ShouldFailModelState()
        {
            var controller = new HocKyController(_context);
            var model = new HocKy { MaHocKy = "HK7" };

            controller.ModelState.AddModelError("TenHocKy", "Required");

            var result = await controller.Create(model);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}
