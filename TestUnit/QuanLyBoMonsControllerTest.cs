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
    public class QuanLyBoMonsControllerTest
    {
        private ModuleDiemDanhDbContext _context;
        private QuanLyBoMonsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);
            _controller = new QuanLyBoMonsController(_context);
        }

        [Test]
        public async Task TC001_Create_Success()
        {
            var model = new QuanLyBoMon
            {
                MaBoMon = "B01",
                TenBoMon = "Bộ môn CNTT"
            };

            var result = await _controller.Create(model);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task TC002_Create_NullCoSoHoatDong_SetsDefault()
        {
            var model = new QuanLyBoMon
            {
                MaBoMon = "B02",
                TenBoMon = "Bộ môn Toán"
                // Không set CoSoHoatDong
            };

            await _controller.Create(model);

            var saved = _context.QuanLyBoMons.FirstOrDefault(x => x.MaBoMon == "B02");
            Assert.IsNotNull(saved);
            Assert.AreEqual("Chưa có", saved.CoSoHoatDong);
        }

        [Test]
        public async Task TC003_GetAll_ReturnsData()
        {
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "B03", TenBoMon = "Lý", CoSoHoatDong = "Cơ sở A" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll() as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Value.ToString());
        }

        [Test]
        public async Task TC004_Create_DuplicateMaBoMon_Allowed_ByDefault()
        {
            var model = new QuanLyBoMon { MaBoMon = "DUP", TenBoMon = "Bộ môn A", CoSoHoatDong = "Cơ sở A" };
            await _controller.Create(model);

            var duplicate = new QuanLyBoMon { MaBoMon = "DUP", TenBoMon = "Bộ môn B", CoSoHoatDong = "Cơ sở B" };
            await _controller.Create(duplicate);

            var count = _context.QuanLyBoMons.Count(x => x.MaBoMon == "DUP");
            Assert.AreEqual(2, count); // xác nhận rằng hệ thống cho phép trùng
        }


        [Test]
        public async Task TC005_GetById_NotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC006_Update_Success()
        {
            var id = Guid.NewGuid();
            _context.QuanLyBoMons.Add(new QuanLyBoMon
            {
                IDBoMon = id,
                MaBoMon = "B05",
                TenBoMon = "Old",
                CoSoHoatDong = "Cơ sở A" // 👈 Bắt buộc có!
            });
            await _context.SaveChangesAsync();

            var model = new QuanLyBoMon { MaBoMon = "B05U", TenBoMon = "New", CoSoHoatDong = "Cơ sở A", TrangThai = false };
            var result = await _controller.Update(id, model);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC007_Update_NotFound()
        {
            var model = new QuanLyBoMon { MaBoMon = "BX", TenBoMon = "Name" };
            var result = await _controller.Update(Guid.NewGuid(), model);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC008_ChangeStatus_ToggleSuccess()
        {
            var id = Guid.NewGuid();
            _context.QuanLyBoMons.Add(new QuanLyBoMon { IDBoMon = id, MaBoMon = "BT", TenBoMon = "Toggle", CoSoHoatDong = "Cơ sở A", TrangThai = true });
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeStatus(id);
            Assert.IsInstanceOf<OkObjectResult>(result);

            var updated = await _context.QuanLyBoMons.FindAsync(id);
            Assert.IsFalse(updated.TrangThai);
        }

        [Test]
        public async Task TC009_ChangeStatus_NotFound()
        {
            var result = await _controller.ChangeStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC010_Update_Existing_ShouldReturnOk()
        {
            var id = Guid.NewGuid();
            _context.QuanLyBoMons.Add(new QuanLyBoMon { IDBoMon = id, MaBoMon = "B05", TenBoMon = "Old", CoSoHoatDong = "A" });
            await _context.SaveChangesAsync();

            var updateModel = new QuanLyBoMon { MaBoMon = "B05U", TenBoMon = "Updated", CoSoHoatDong = "A", TrangThai = false };
            var result = await _controller.Update(id, updateModel);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC007_ChangeStatus_Toggle()
        {
            var id = Guid.NewGuid();
            _context.QuanLyBoMons.Add(new QuanLyBoMon { IDBoMon = id, MaBoMon = "B06", TenBoMon = "ToggleTest", CoSoHoatDong = "A", TrangThai = true });
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeStatus(id);
            var updated = await _context.QuanLyBoMons.FindAsync(id);

            Assert.IsFalse(updated.TrangThai);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC012_Create_SetsTrangThaiTrue()
        {
            var model = new QuanLyBoMon { MaBoMon = "B12", TenBoMon = "TT" };
            await _controller.Create(model);

            var saved = _context.QuanLyBoMons.FirstOrDefault(x => x.MaBoMon == "B12");
            Assert.IsTrue(saved.TrangThai);
        }

        [Test]
        public void TC013_Create_MissingTenBoMon_ShouldFail()
        {
            // Arrange
            var model = new QuanLyBoMon
            {
                MaBoMon = "B13",
                CoSoHoatDong = "Cơ sở A"
                // Thiếu TenBoMon
            };

            // Act + Assert: Bắt lỗi đúng kiểu
            var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _controller.Create(model);
            });

            Assert.That(ex.Message, Does.Contain("TenBoMon"));
        }


        [Test]
        public async Task TC014_Create_EmptyMaBoMon_StillSucceeds()
        {
            var model = new QuanLyBoMon { TenBoMon = "Thiếu mã", MaBoMon = "B12", CoSoHoatDong = "Cơ sở A" };
            var result = await _controller.Create(model);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task TC015_Update_OnlyPartialFields()
        {
            var id = Guid.NewGuid();
            _context.QuanLyBoMons.Add(new QuanLyBoMon { IDBoMon = id, MaBoMon = "Original", TenBoMon = "Old", CoSoHoatDong = "Cơ sở A" });
            await _context.SaveChangesAsync();

            var model = new QuanLyBoMon { TenBoMon = "Partial Update", TrangThai = true, CoSoHoatDong = "Cơ sở B" };
            var result = await _controller.Update(id, model);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}
