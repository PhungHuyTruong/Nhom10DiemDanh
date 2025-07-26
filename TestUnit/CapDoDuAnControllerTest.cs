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
    public class CapDoDuAnControllerTest
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
        public async Task TC001_GetAll_ReturnsList()
        {
            var entity = new CapDoDuAn
            {
                IdCDDA = Guid.NewGuid(),
                MaCapDoDuAn = "CD01",
                TenCapDoDuAn = "Cấp độ 1",
                MoTa = "Mô tả cấp độ 1", // ✅ Bổ sung trường bắt buộc
                TrangThai = true,
                NgayTao = DateTime.Now
            };
            _context.CapDoDuAns.Add(entity);
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);
            var result = await controller.GetAll() as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as List<CapDoDuAn>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }


        [Test]
        public async Task TC002_GetById_ReturnsItem()
        {
            // Arrange
            var id = Guid.NewGuid();
            var capDo = new CapDoDuAn
            {
                IdCDDA = id,
                MaCapDoDuAn = "CD02",
                TenCapDoDuAn = "Cấp độ 2",
                MoTa = "Mô tả cấp độ 2", // ✅ Thêm dòng này
                TrangThai = true,
                NgayTao = DateTime.Now
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);

            // Act
            var result = await controller.GetById(id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var returnValue = result.Value as CapDoDuAn;
            Assert.AreEqual("CD02", returnValue.MaCapDoDuAn);
            Assert.AreEqual("Cấp độ 2", returnValue.TenCapDoDuAn);
            Assert.AreEqual("Mô tả cấp độ 2", returnValue.MoTa);
        }


        [Test]
        public async Task TC003_GetById_NotFound()
        {
            var controller = new CapDoDuAnController(_context);
            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC004_Create_Success()
        {
            var controller = new CapDoDuAnController(_context);
            var model = new CapDoDuAn { TenCapDoDuAn = "Cấp C", MaCapDoDuAn = "C01", MoTa = "Mô tả C" };

            var result = await controller.Create(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task TC005_Create_ShouldSetTrangThaiTrue()
        {
            var controller = new CapDoDuAnController(_context);
            var model = new CapDoDuAn { TenCapDoDuAn = "Cấp D", MaCapDoDuAn = "D01", MoTa = "Mô tả D" };

            await controller.Create(model);
            var entity = _context.CapDoDuAns.FirstOrDefault(x => x.MaCapDoDuAn == "D01");

            Assert.IsTrue(entity.TrangThai);
        }

        [Test]
        public async Task TC006_Update_Success()
        {
            var id = Guid.NewGuid();
            _context.CapDoDuAns.Add(new CapDoDuAn
            {
                IdCDDA = id,
                TenCapDoDuAn = "Old",
                MaCapDoDuAn = "OLD",
                MoTa = "Old Desc",
                TrangThai = true
            });
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);
            var update = new CapDoDuAn
            {
                TenCapDoDuAn = "New",
                MaCapDoDuAn = "NEW",
                MoTa = "New Desc",
                TrangThai = false
            };

            var result = await controller.Update(id, update);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC007_Update_NotFound()
        {
            var controller = new CapDoDuAnController(_context);
            var update = new CapDoDuAn { TenCapDoDuAn = "X", MaCapDoDuAn = "X01", TrangThai = true };

            var result = await controller.Update(Guid.NewGuid(), update);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC008_DoiTrangThai_Success()
        {
            // Arrange
            var id = Guid.NewGuid();
            var capDo = new CapDoDuAn
            {
                IdCDDA = id,
                MaCapDoDuAn = "CD08",
                TenCapDoDuAn = "Cấp độ 08",
                MoTa = "Mô tả cho cấp độ 08",
                TrangThai = true,
                NgayTao = DateTime.Now
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);

            // Act
            var result = await controller.DoiTrangThai(id);
            var updated = await _context.CapDoDuAns.FindAsync(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsNotNull(updated);
            Assert.IsFalse(updated.TrangThai); // Đã toggle từ true => false
        }


        [Test]
        public async Task TC009_DoiTrangThai_NotFound()
        {
            var controller = new CapDoDuAnController(_context);
            var result = await controller.DoiTrangThai(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC010_Create_InvalidModelState()
        {
            var controller = new CapDoDuAnController(_context);
            controller.ModelState.AddModelError("TenCapDoDuAn", "Required");

            var model = new CapDoDuAn { MaCapDoDuAn = "ERR01" };
            var result = await controller.Create(model);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task TC011_Update_OnlyPartialFields()
        {
            // Arrange
            var id = Guid.NewGuid();
            var original = new CapDoDuAn
            {
                IdCDDA = id,
                MaCapDoDuAn = "CD11",
                TenCapDoDuAn = "Cấp độ ban đầu",
                MoTa = "Mô tả ban đầu",
                TrangThai = true,
                NgayTao = DateTime.Now
            };
            _context.CapDoDuAns.Add(original);
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);

            // Act
            var updateModel = new CapDoDuAn
            {
                MaCapDoDuAn = "CD11-New",
                TenCapDoDuAn = "Tên mới",
                MoTa = "Mô tả mới", // Bắt buộc phải có
                TrangThai = true
            };

            var result = await controller.Update(id, updateModel);
            var updated = await _context.CapDoDuAns.FindAsync(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("CD11-New", updated.MaCapDoDuAn);
            Assert.AreEqual("Tên mới", updated.TenCapDoDuAn);
            Assert.AreEqual("Mô tả mới", updated.MoTa);
        }


        [Test]
        public async Task TC012_Update_ShouldChangeNgayCapNhat()
        {
            var id = Guid.NewGuid();
            var oldDate = DateTime.Now.AddDays(-10);

            var capDo = new CapDoDuAn
            {
                IdCDDA = id,
                MaCapDoDuAn = "CD09",
                TenCapDoDuAn = "Old",
                MoTa = "Mô tả cũ", // ✅ thêm trường bắt buộc
                TrangThai = true,
                NgayTao = oldDate,
                NgayCapNhat = oldDate
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);

            var updateModel = new CapDoDuAn
            {
                MaCapDoDuAn = "CD09U",
                TenCapDoDuAn = "Updated",
                MoTa = "Mô tả mới", // ✅ cũng cần đủ field trong model update
                TrangThai = false
            };

            var result = await controller.Update(id, updateModel);
            var updated = await _context.CapDoDuAns.FindAsync(id);

            Assert.AreNotEqual(oldDate, updated.NgayCapNhat);
        }


        [Test]
        public async Task TC013_DoiTrangThai_ShouldUpdateNgayCapNhat()
        {
            var id = Guid.NewGuid();
            _context.CapDoDuAns.Add(new CapDoDuAn
            {
                IdCDDA = id,
                TenCapDoDuAn = "CD",
                MaCapDoDuAn = "CD01",
                TrangThai = true,
                MoTa = "Mô tả test" // ✅ FIX: Bổ sung trường bắt buộc
            });
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);
            var before = DateTime.Now;
            await controller.DoiTrangThai(id);
            var updated = await _context.CapDoDuAns.FindAsync(id);

            Assert.Greater(updated.NgayCapNhat, before);
        }


        [Test]
        public async Task TC014_GetAll_EmptyList()
        {
            var controller = new CapDoDuAnController(_context);
            var result = await controller.GetAll() as OkObjectResult;
            var list = result.Value as System.Collections.IEnumerable;

            Assert.IsNotNull(list);
        }

        [Test]
        public async Task TC015_GetById_CorrectDataReturned()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new CapDoDuAn
            {
                IdCDDA = id,
                TenCapDoDuAn = "Cap Z",
                MaCapDoDuAn = "Z01",
                MoTa = "Mô tả test",
                TrangThai = true,
                NgayTao = DateTime.Now
            };
            _context.CapDoDuAns.Add(entity);
            await _context.SaveChangesAsync();

            var controller = new CapDoDuAnController(_context);

            // Act
            var result = await controller.GetById(id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var returnedData = result.Value as CapDoDuAn;
            Assert.IsNotNull(returnedData);
            Assert.AreEqual(entity.MaCapDoDuAn, returnedData.MaCapDoDuAn);
            Assert.AreEqual(entity.TenCapDoDuAn, returnedData.TenCapDoDuAn);
            Assert.AreEqual(entity.MoTa, returnedData.MoTa);
            Assert.AreEqual(entity.TrangThai, returnedData.TrangThai);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
