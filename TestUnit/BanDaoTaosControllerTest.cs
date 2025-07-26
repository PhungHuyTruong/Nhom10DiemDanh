using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestUnit
{
    [TestFixture]
    public class BanDaoTaoControllerTest
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
        public async Task TC001_GetByID_ReturnsOk_WhenFound()
        {
            var bdt = new BanDaoTao
            {
                IdBanDaoTao = Guid.NewGuid(),
                MaBanDaoTao = "BDT01",
                TenBanDaoTao = "Ban A",
                Email = "bdt01@edu.vn",
                TrangThai = true
            };

            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);
            var result = await controller.GetByID(bdt.IdBanDaoTao);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TC002_GetByID_ReturnsNotFound_WhenMissing()
        {
            var controller = new BanDaoTaoController(_context);
            var result = await controller.GetByID(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC003_Create_AddsNewBanDaoTao()
        {
            var controller = new BanDaoTaoController(_context);
            var bdt = new BanDaoTao { MaBanDaoTao = "BDT02", TenBanDaoTao = "Ban B", Email = "b@edu.vn" };

            var result = await controller.Create(bdt);
            Assert.IsTrue(_context.BanDaoTaos.Any(x => x.MaBanDaoTao == "BDT02"));
        }

        [Test]
        public async Task TC004_Create_InvalidModel_ReturnsBadRequest()
        {
            var controller = new BanDaoTaoController(_context);
            controller.ModelState.AddModelError("MaBanDaoTao", "Required");
            var result = await controller.Create(new BanDaoTao());

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public async Task TC005_Update_ValidId_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();

            // Thêm bản ghi gốc với Email hợp lệ
            _context.BanDaoTaos.Add(new BanDaoTao
            {
                IdBanDaoTao = id,
                MaBanDaoTao = "BDT03",
                TenBanDaoTao = "Ban C",
                Email = "original@example.com",
                TrangThai = true
            });
            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);

            // Tạo đối tượng update có Email hợp lệ
            var updated = new BanDaoTao
            {
                MaBanDaoTao = "BDT03U",
                TenBanDaoTao = "Ban C Updated",
                TrangThai = false,
                Email = "updated@example.com" // thêm email để tránh lỗi
            };

            var result = await controller.Update(id, updated);

            Assert.IsInstanceOf<OkObjectResult>(result);

            // Kiểm tra dữ liệu đã được cập nhật
            var updatedEntity = await _context.BanDaoTaos.FindAsync(id);
            Assert.AreEqual("BDT03U", updatedEntity.MaBanDaoTao);
            Assert.AreEqual("Ban C Updated", updatedEntity.TenBanDaoTao);
            Assert.AreEqual("updated@example.com", updatedEntity.Email);
            Assert.IsFalse(updatedEntity.TrangThai);
        }


        [Test]
        public async Task TC006_Update_InvalidId_ReturnsNotFound()
        {
            var controller = new BanDaoTaoController(_context);
            var result = await controller.Update(Guid.NewGuid(), new BanDaoTao());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC007_Delete_RemovesItem()
        {
            var id = Guid.NewGuid();

            _context.BanDaoTaos.Add(new BanDaoTao
            {
                IdBanDaoTao = id,
                MaBanDaoTao = "BDT_DEL",
                TenBanDaoTao = "Xóa Test",
                Email = "delete@edu.vn",
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);
            var result = await controller.Delete(id);

            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.IsNull(await _context.BanDaoTaos.FindAsync(id));
        }


        [Test]
        public async Task TC008_Delete_InvalidId_ReturnsNotFound()
        {
            var controller = new BanDaoTaoController(_context);
            var result = await controller.Delete(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC009_ChangeStatus_TogglesValue()
        {
            var id = Guid.NewGuid();

            _context.BanDaoTaos.Add(new BanDaoTao
            {
                IdBanDaoTao = id,
                MaBanDaoTao = "BDT_CHANGE",
                TenBanDaoTao = "Test Toggle",
                Email = "toggle@edu.vn",
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);
            var result = await controller.ChangeStatus(id);
            var updated = await _context.BanDaoTaos.FindAsync(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsFalse(updated.TrangThai); // Toggle từ true -> false
        }


        [Test]
        public async Task TC011_GetPaged_ReturnsPagination()
        {
            for (int i = 0; i < 10; i++)
            {
                _context.BanDaoTaos.Add(new BanDaoTao
                {
                    MaBanDaoTao = "BDT" + i,
                    TenBanDaoTao = "Ban " + i,
                    Email = $"bdt{i}@edu.vn",   // ✅ Bắt buộc phải có
                    TrangThai = true
                });
            }

            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);
            var result = await controller.GetPaged(1, 5) as OkObjectResult;

            Assert.IsNotNull(result);
        }


        [Test]
        public async Task TC012_GetPaged_SearchByKeyword()
        {
            _context.BanDaoTaos.Add(new BanDaoTao
            {
                MaBanDaoTao = "SEARCH01",
                TenBanDaoTao = "Find me",
                Email = "search01@edu.vn", // ✅ required
                TrangThai = true
            });

            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);
            var result = await controller.GetPaged(search: "search01") as OkObjectResult;

            Assert.IsNotNull(result);
        }


        [Test]
        public async Task TC013_GetPaged_FilterStatusActive()
        {
            // Arrange
            _context.BanDaoTaos.Add(new BanDaoTao
            {
                MaBanDaoTao = "BDT1",
                TenBanDaoTao = "A",
                Email = "bdt1@edu.vn",
                TrangThai = true
            });

            _context.BanDaoTaos.Add(new BanDaoTao
            {
                MaBanDaoTao = "BDT2",
                TenBanDaoTao = "B",
                Email = "bdt2@edu.vn",
                TrangThai = false
            });

            await _context.SaveChangesAsync();
            var controller = new BanDaoTaoController(_context);

            // Act
            var result = await controller.GetPaged(status: "active") as OkObjectResult;

            // Parse kết quả JSON từ anonymous object
            var json = JsonConvert.SerializeObject(result.Value);
            var parsed = JObject.Parse(json);
            var dataArray = parsed["data"].ToObject<List<BanDaoTao>>();

            // Assert
            Assert.IsNotNull(dataArray);
            Assert.IsTrue(dataArray.Any(x => x.MaBanDaoTao == "BDT1"));
            Assert.IsTrue(dataArray.All(x => x.TrangThai == true));
        }


        [Test]
        public async Task TC014_Create_SetsNgayTao()
        {
            var controller = new BanDaoTaoController(_context);

            var bdt = new BanDaoTao
            {
                MaBanDaoTao = "BDT11",
                TenBanDaoTao = "Ban Now",
                Email = "now@edu.vn"
            };

            await controller.Create(bdt);

            var saved = _context.BanDaoTaos.FirstOrDefault(x => x.MaBanDaoTao == "BDT11");

            Assert.IsNotNull(saved);
            Assert.That(saved.NgayTao, Is.Not.EqualTo(default(DateTime)));
        }


        [Test]
        public async Task TC015_Update_SetsNgayCapNhat()
        {
            var id = Guid.NewGuid();

            _context.BanDaoTaos.Add(new BanDaoTao
            {
                IdBanDaoTao = id,
                MaBanDaoTao = "UPD1",
                TenBanDaoTao = "Old",
                Email = "old@edu.vn",
                TrangThai = true,
                NgayTao = DateTime.Now
            });

            await _context.SaveChangesAsync();

            var controller = new BanDaoTaoController(_context);

            var update = new BanDaoTao
            {
                MaBanDaoTao = "UPD1",
                TenBanDaoTao = "Updated",
                Email = "updated@edu.vn",
                TrangThai = false
            };

            await controller.Update(id, update);

            var saved = await _context.BanDaoTaos.FindAsync(id);

            Assert.That(saved.NgayCapNhat, Is.Not.EqualTo(null));
            Assert.That(saved.TenBanDaoTao, Is.EqualTo("Updated"));
            Assert.That(saved.Email, Is.EqualTo("updated@edu.vn"));
        }


        [TearDown]
        public void TearDown() => _context.Dispose();
    }
}
