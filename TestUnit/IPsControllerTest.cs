using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TestUnit
{
    [TestFixture]
    public class IPsControllerTest
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
        public async Task TC001_GetById_ReturnsOk()
        {
            var ip = new IP { IdIP = Guid.NewGuid(), KieuIP = "IPv4", IP_DaiIP = "192.168.0.1", TrangThai = true, IdCoSo = Guid.NewGuid() };
            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var controller = new IPsController(_context);
            var result = await controller.GetById(ip.IdIP);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TC002_GetById_NotFound()
        {
            var controller = new IPsController(_context);
            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC003_GetByCoSo_ReturnsList()
        {
            var coSoId = Guid.NewGuid();
            _context.IPs.Add(new IP { IdIP = Guid.NewGuid(), KieuIP = "IPv4", IP_DaiIP = "10.0.0.1", IdCoSo = coSoId });
            await _context.SaveChangesAsync();

            var controller = new IPsController(_context);
            var result = await controller.GetByCoSo(coSoId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<IP>>(result.Value);
        }

        [Test]
        public async Task TC004_Post_ValidData_CreatesIP()
        {
            var controller = new IPsController(_context);
            var ip = new IP { KieuIP = "IPv6", IP_DaiIP = "::1", IdCoSo = Guid.NewGuid(), TrangThai = true };

            var result = await controller.Post(ip);

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task TC005_Post_InvalidModel_ReturnsBadRequest()
        {
            var controller = new IPsController(_context);
            controller.ModelState.AddModelError("KieuIP", "Required");

            var ip = new IP { IP_DaiIP = "0.0.0.0" };
            var result = await controller.Post(ip);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TC006_Update_ValidId_UpdatesSuccessfully()
        {
            var idCoSo = Guid.NewGuid();
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                KieuIP = "IPv4",
                IP_DaiIP = "192.168.1.1",
                IdCoSo = idCoSo, // 🔴 BẮT BUỘC PHẢI CÓ
                TrangThai = true,
                NgayCapNhat = DateTime.Now.AddDays(-1)
            };

            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var controller = new IPsController(_context);
            var updatedIP = new IP
            {
                IdIP = ip.IdIP,
                KieuIP = "IPv6",
                IP_DaiIP = "::1",
                IdCoSo = idCoSo, // 🔴 PHẢI TRÙNG ID CƠ SỞ CŨ
                TrangThai = false
            };

            var result = await controller.Update(ip.IdIP, updatedIP);

            Assert.IsInstanceOf<NoContentResult>(result);

            var dbItem = await _context.IPs.FindAsync(ip.IdIP);
            Assert.AreEqual("IPv6", dbItem.KieuIP);
            Assert.AreEqual("::1", dbItem.IP_DaiIP);
            Assert.AreEqual(false, dbItem.TrangThai);
        }


        [Test]
        public async Task TC007_Update_IdMismatch_ReturnsBadRequest()
        {
            var controller = new IPsController(_context);
            var result = await controller.Update(Guid.NewGuid(), new IP { IdIP = Guid.NewGuid() });

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task TC008_Update_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            var controller = new IPsController(_context);

            var ip = new IP { IdIP = id, KieuIP = "IPv4", IP_DaiIP = "127.0.0.1", TrangThai = true };
            var result = await controller.Update(id, ip);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TC009_Post_SetsNgayCapNhat()
        {
            var controller = new IPsController(_context);
            var ip = new IP { KieuIP = "IPv4", IP_DaiIP = "8.8.8.8", TrangThai = true, IdCoSo = Guid.NewGuid() };

            await controller.Post(ip);
            var saved = _context.IPs.FirstOrDefault(x => x.IP_DaiIP == "8.8.8.8");

            Assert.That(saved.NgayCapNhat, Is.Not.Null);
        }

        [Test]
        public async Task TC010_Update_SetsNgayCapNhat()
        {
            var id = Guid.NewGuid();
            var coSoId = Guid.NewGuid(); // Thêm dòng này để có giá trị IdCoSo

            var ip = new IP
            {
                IdIP = id,
                KieuIP = "IPv4",
                IP_DaiIP = "8.8.4.4",
                TrangThai = true,
                NgayCapNhat = DateTime.MinValue,
                IdCoSo = coSoId // ✅ Cần thiết để tránh lỗi
            };

            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var update = new IP
            {
                IdIP = id,
                KieuIP = "IPv6",
                IP_DaiIP = "::2",
                TrangThai = false,
                IdCoSo = coSoId // ✅ Cũng nên truyền lại
            };

            var controller = new IPsController(_context);
            await controller.Update(id, update);

            var updated = await _context.IPs.FindAsync(id);
            Assert.That(updated.NgayCapNhat, Is.GreaterThan(DateTime.MinValue));
        }


        [Test]
        public async Task TC011_GetByCoSo_EmptyList()
        {
            var controller = new IPsController(_context);
            var result = await controller.GetByCoSo(Guid.NewGuid()) as OkObjectResult;
            var list = result.Value as List<IP>;

            Assert.IsNotNull(list);
            Assert.IsEmpty(list);
        }

        [Test]
        public async Task TC012_Post_GeneratesNewId()
        {
            var controller = new IPsController(_context);
            var ip = new IP { KieuIP = "Test", IP_DaiIP = "1.1.1.1", IdCoSo = Guid.NewGuid() };

            await controller.Post(ip);
            var created = _context.IPs.FirstOrDefault(x => x.IP_DaiIP == "1.1.1.1");

            Assert.That(created.IdIP, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task TC0013_Update_UpdatesNgayCapNhat()
        {
            var idCoSo = Guid.NewGuid();
            var id = Guid.NewGuid();
            var oldDate = DateTime.UtcNow.AddDays(-1);

            var ip = new IP
            {
                IdIP = id,
                KieuIP = "IPv4",
                IP_DaiIP = "192.168.1.10",
                IdCoSo = idCoSo,
                TrangThai = true,
                NgayCapNhat = oldDate
            };
            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var controller = new IPsController(_context);

            var before = DateTime.UtcNow.AddMilliseconds(-1); // 💡 Giảm 1ms để đảm bảo so sánh đúng

            var updateModel = new IP
            {
                IdIP = id,
                KieuIP = "IPv6",
                IP_DaiIP = "::1",
                IdCoSo = idCoSo,
                TrangThai = false
            };

            await controller.Update(id, updateModel);

            var updated = await _context.IPs.FindAsync(id);

            Assert.That(updated.NgayCapNhat, Is.GreaterThan(before));
        }



        [Test]
        public async Task TC014_Update_KeepsCoSoId_Unchanged()
        {
            var id = Guid.NewGuid();
            var coSoId = Guid.NewGuid();
            _context.IPs.Add(new IP { IdIP = id, KieuIP = "A", IP_DaiIP = "1.1.1.1", IdCoSo = coSoId, TrangThai = true });
            await _context.SaveChangesAsync();

            var controller = new IPsController(_context);
            var update = new IP { IdIP = id, KieuIP = "B", IP_DaiIP = "2.2.2.2", TrangThai = false };

            await controller.Update(id, update);
            var updated = await _context.IPs.FindAsync(id);

            Assert.AreEqual(coSoId, updated.IdCoSo);
        }

        [Test]
        public async Task TC015_Post_SavesAllFieldsCorrectly()
        {
            var controller = new IPsController(_context);
            var idCoSo = Guid.NewGuid();
            var ip = new IP { KieuIP = "TestFull", IP_DaiIP = "3.3.3.3", TrangThai = true, IdCoSo = idCoSo };

            await controller.Post(ip);
            var saved = _context.IPs.FirstOrDefault(x => x.IP_DaiIP == "3.3.3.3");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("TestFull", saved.KieuIP);
                Assert.AreEqual(true, saved.TrangThai);
                Assert.AreEqual(idCoSo, saved.IdCoSo);
            });
        }

        [TearDown]
        public void TearDown() => _context.Dispose();
    }
}
