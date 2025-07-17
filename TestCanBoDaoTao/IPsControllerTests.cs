using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCanBoDaoTao
{
    [TestFixture]
    public class IPsControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private IPsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new IPsController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây

        //1. Test tạo mới IP thành công
        [Test]
        public async Task Post_ValidIP_ReturnsCreated()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@ip.com"
            });
            await _context.SaveChangesAsync();

            var ip = new IP
            {
                KieuIP = "ip1",
                IP_DaiIP = "192.168.1.1",
                IdCoSo = coSoId
            };

            var result = await _controller.Post(ip);
            var created = result as CreatedAtActionResult;
            Assert.IsNotNull(created);

            var saved = _context.IPs.FirstOrDefault(i => i.KieuIP == "ip1");
            Assert.IsNotNull(saved);
        }

        //2. Test tạo mới IP thiếu trường bắt buộc
        [Test]
        public async Task Post_MissingKieuIP_ReturnsBadRequest()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@ip.com"
            });
            await _context.SaveChangesAsync();

            var ip = new IP
            {
                // KieuIP = null,
                IP_DaiIP = "192.168.1.1",
                IdCoSo = coSoId
            };

            _controller.ModelState.AddModelError("KieuIP", "Kiểu IP không được để trống");

            var result = await _controller.Post(ip);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //3. Test lấy IP theo ID thành công
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var coSoId = Guid.NewGuid();
            var ip = new IP
            {
                KieuIP = "ip2",
                IP_DaiIP = "192.168.1.2",
                IdCoSo = coSoId
            };
            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(ip.IdIP);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as IP;
            Assert.AreEqual("ip2", returned.KieuIP);
        }

        //4. Test lấy IP theo ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //5. Test lấy danh sách IP theo cơ sở
        [Test]
        public async Task GetByCoSo_ReturnsCorrectList()
        {
            var coSoId = Guid.NewGuid();
            _context.IPs.Add(new IP
            {
                KieuIP = "ip3",
                IP_DaiIP = "192.168.1.3",
                IdCoSo = coSoId
            });
            _context.IPs.Add(new IP
            {
                KieuIP = "ip4",
                IP_DaiIP = "192.168.1.4",
                IdCoSo = coSoId
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetByCoSo(coSoId);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as System.Collections.Generic.List<IP>;
            Assert.AreEqual(2, list.Count);
        }

        //6. Test cập nhật IP thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var coSoId = Guid.NewGuid();
            var ip = new IP
            {
                KieuIP = "ip5",
                IP_DaiIP = "192.168.1.5",
                IdCoSo = coSoId
            };
            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var updated = new IP
            {
                IdIP = ip.IdIP,
                KieuIP = "ip5-updated",
                IP_DaiIP = "192.168.1.55",
                TrangThai = false,
                IdCoSo = coSoId
            };

            var result = await _controller.Update(ip.IdIP, updated);
            Assert.IsInstanceOf<NoContentResult>(result);

            var ipDb = _context.IPs.Find(ip.IdIP);
            Assert.AreEqual("ip5-updated", ipDb.KieuIP);
            Assert.AreEqual("192.168.1.55", ipDb.IP_DaiIP);
            Assert.IsFalse(ipDb.TrangThai);
        }

        //7. Test cập nhật IP với ID không khớp
        [Test]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                KieuIP = "ip6",
                IP_DaiIP = "192.168.1.6",
                IdCoSo = Guid.NewGuid()
            };

            var result = await _controller.Update(Guid.NewGuid(), ip);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        //8. Test cập nhật IP với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                KieuIP = "ip7",
                IP_DaiIP = "192.168.1.7",
                IdCoSo = Guid.NewGuid()
            };

            var result = await _controller.Update(ip.IdIP, ip);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //9. Test tạo mới IP với IdCoSo không tồn tại
        [Test]
        public async Task Post_IdCoSoNotExist_AllowsOrRejects()
        {
            var ip = new IP
            {
                KieuIP = "ip8",
                IP_DaiIP = "192.168.1.8",
                IdCoSo = Guid.NewGuid() // Không có CoSo này
            };

            var result = await _controller.Post(ip);
            // Nếu bạn kiểm tra ràng buộc FK, có thể trả về lỗi
            // Nếu không kiểm tra, sẽ cho phép tạo
            // Assert phù hợp với logic của bạn
        }

        //10. Test tạo mới IP với KieuIP trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Post_DuplicateKieuIP_AllowsOrRejects()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@ip.com"
            });
            await _context.SaveChangesAsync();

            var ip1 = new IP
            {
                KieuIP = "ip9",
                IP_DaiIP = "192.168.1.9",
                IdCoSo = coSoId
            };
            var ip2 = new IP
            {
                KieuIP = "ip9",
                IP_DaiIP = "192.168.1.99",
                IdCoSo = coSoId
            };

            await _controller.Post(ip1);
            var result = await _controller.Post(ip2);

            // Nếu có kiểm tra trùng, nên trả về BadRequest
            // Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //11. Test lấy danh sách IP khi không có dữ liệu
        [Test]
        public async Task GetByCoSo_Empty_ReturnsEmptyList()
        {
            var coSoId = Guid.NewGuid();
            var result = await _controller.GetByCoSo(coSoId);
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<IP>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        //12. Test dữ liệu lớn (stress test)
        [Test]
        public async Task GetByCoSo_LargeData_ReturnsAll()
        {
            var coSoId = Guid.NewGuid();
            for (int i = 0; i < 500; i++)
            {
                _context.IPs.Add(new IP
                {
                    KieuIP = $"ip{i}",
                    IP_DaiIP = $"192.168.1.{i}",
                    IdCoSo = coSoId
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetByCoSo(coSoId);
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<IP>;
            Assert.AreEqual(500, list.Count);
        }

        //13. Test cập nhật trạng thái IP
        [Test]
        public async Task Update_ToggleTrangThai_UpdatesSuccessfully()
        {
            var coSoId = Guid.NewGuid();
            var ip = new IP
            {
                KieuIP = "ipToggle",
                IP_DaiIP = "192.168.1.100",
                IdCoSo = coSoId,
                TrangThai = true
            };
            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            ip.TrangThai = false;
            var result = await _controller.Update(ip.IdIP, ip);
            Assert.IsInstanceOf<NoContentResult>(result);

            var ipDb = _context.IPs.Find(ip.IdIP);
            Assert.IsFalse(ipDb.TrangThai);
        }

        //14. Test tạo mới IP với IP_DaiIP trống
        [Test]
        public async Task Post_MissingIP_DaiIP_ReturnsBadRequest()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@ip.com"
            });
            await _context.SaveChangesAsync();

            var ip = new IP
            {
                KieuIP = "ip10",
                // IP_DaiIP = null,
                IdCoSo = coSoId
            };

            _controller.ModelState.AddModelError("IP_DaiIP", "Dải IP không được để trống");

            var result = await _controller.Post(ip);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //15. Test cập nhật IP chỉ thay đổi một trường
        [Test]
        public async Task Update_OnlyKieuIP_UpdatesSuccessfully()
        {
            var coSoId = Guid.NewGuid();
            var ip = new IP
            {
                KieuIP = "ip11",
                IP_DaiIP = "192.168.1.11",
                IdCoSo = coSoId
            };
            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            var updated = new IP
            {
                IdIP = ip.IdIP,
                KieuIP = "ip11-updated",
                IP_DaiIP = ip.IP_DaiIP,
                TrangThai = ip.TrangThai,
                IdCoSo = coSoId
            };

            var result = await _controller.Update(ip.IdIP, updated);
            Assert.IsInstanceOf<NoContentResult>(result);

            var ipDb = _context.IPs.Find(ip.IdIP);
            Assert.AreEqual("ip11-updated", ipDb.KieuIP);
            Assert.AreEqual("192.168.1.11", ipDb.IP_DaiIP);
        }

        

        //16. Test lấy IP theo cơ sở với nhiều cơ sở khác nhau
        [Test]
        public async Task GetByCoSo_MultipleCoSo_ReturnsOnlyCorrectIPs()
        {
            var coSoId1 = Guid.NewGuid();
            var coSoId2 = Guid.NewGuid();

            _context.IPs.Add(new IP { KieuIP = "ip14", IP_DaiIP = "192.168.1.14", IdCoSo = coSoId1 });
            _context.IPs.Add(new IP { KieuIP = "ip15", IP_DaiIP = "192.168.1.15", IdCoSo = coSoId2 });
            await _context.SaveChangesAsync();

            var result1 = await _controller.GetByCoSo(coSoId1);
            var okResult1 = result1 as OkObjectResult;
            var list1 = okResult1.Value as List<IP>;
            Assert.AreEqual(1, list1.Count);
            Assert.AreEqual("ip14", list1[0].KieuIP);

            var result2 = await _controller.GetByCoSo(coSoId2);
            var okResult2 = result2 as OkObjectResult;
            var list2 = okResult2.Value as List<IP>;
            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual("ip15", list2[0].KieuIP);
        }

        //17. Test tạo mới IP với trạng thái mặc định
        [Test]
        public async Task Post_ValidIP_DefaultTrangThaiIsTrue()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@ip.com"
            });
            await _context.SaveChangesAsync();

            var ip = new IP
            {
                KieuIP = "ip16",
                IP_DaiIP = "192.168.1.16",
                IdCoSo = coSoId
                // Không set TrangThai
            };

            var result = await _controller.Post(ip);
            var created = result as CreatedAtActionResult;
            Assert.IsNotNull(created);

            var saved = _context.IPs.FirstOrDefault(i => i.KieuIP == "ip16");
            Assert.IsTrue(saved.TrangThai);
        }

    }
}
