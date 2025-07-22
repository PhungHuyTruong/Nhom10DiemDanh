using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCanBoDaoTao
{
    [TestFixture]
    public class BanDaoTaoControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private BanDaoTaoController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new BanDaoTaoController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây

        //1. Test lấy tất cả ban đào tạo (paging mặc định)
        [Test]
        public async Task GetPaged_ReturnsAllBanDaoTao()
        {
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "BDT1", MaBanDaoTao = "BDT_1", Email = "a@email.com" });
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "BDT2", MaBanDaoTao = "BDT_2", Email = "b@email.com" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Parse anonymous object bằng Newtonsoft.Json
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = JObject.Parse(json);
            var list = jObj["data"].ToObject<List<BanDaoTao>>();

            Assert.IsTrue(list.Count >= 2);
        }


        //2. Test lấy ban đào tạo theo ID hợp lệ
        [Test]
        public async Task GetByID_ValidId_ReturnsOk()
        {
            var bdt = new BanDaoTao { TenBanDaoTao = "BDT3", MaBanDaoTao = "BDT_3", Email = "c@email.com" };
            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var result = await _controller.GetByID(bdt.IdBanDaoTao);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as BanDaoTao;
            Assert.AreEqual("BDT3", returned.TenBanDaoTao);
        }

        //3.Test lấy ban đào tạo với ID không tồn tại
        [Test]
        public async Task GetByID_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetByID(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //4. Test tạo mới ban đào tạo thành công
        [Test]
        public async Task Create_ValidBanDaoTao_ReturnsRedirect()
        {
            var bdt = new BanDaoTao
            {
                TenBanDaoTao = "BDT4",
                MaBanDaoTao = "BDT_4",
                Email = "d@email.com"
            };

            var result = await _controller.Create(bdt);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var saved = _context.BanDaoTaos.FirstOrDefault(x => x.MaBanDaoTao == "BDT_4");
            Assert.IsNotNull(saved);
        }

        //5. Test tạo mới thiếu trường bắt buộc (Tên ban đào tạo)
        [Test]
        public async Task Create_MissingTenBanDaoTao_ReturnsBadRequest()
        {
            var bdt = new BanDaoTao
            {
                TenBanDaoTao = null,
                MaBanDaoTao = "BDT_5",
                Email = "e@email.com"
            };
            _controller.ModelState.AddModelError("TenBanDaoTao", "Tên ban đào tạo là bắt buộc.");

            var result = await _controller.Create(bdt);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //6. Test tạo mới với email không hợp lệ
        [Test]
        public async Task Create_InvalidEmail_ReturnsBadRequest()
        {
            var bdt = new BanDaoTao
            {
                TenBanDaoTao = "BDT6",
                MaBanDaoTao = "BDT_6",
                Email = "not-an-email"
            };
            _controller.ModelState.AddModelError("Email", "Email không hợp lệ.");

            var result = await _controller.Create(bdt);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //7. Test cập nhật ban đào tạo thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var bdt = new BanDaoTao { TenBanDaoTao = "BDT7", MaBanDaoTao = "BDT_7", Email = "f@email.com" };
            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var updated = new BanDaoTao
            {
                TenBanDaoTao = "BDT7-updated",
                MaBanDaoTao = "BDT_7-updated",
                Email = "f-updated@email.com",
                TrangThai = false
            };

            var result = await _controller.Update(bdt.IdBanDaoTao, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var bdtDb = _context.BanDaoTaos.Find(bdt.IdBanDaoTao);
            Assert.AreEqual("BDT7-updated", bdtDb.TenBanDaoTao);
            Assert.IsFalse(bdtDb.TrangThai);
        }

        //8. Test cập nhật với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var updated = new BanDaoTao
            {
                TenBanDaoTao = "notfound",
                MaBanDaoTao = "notfound",
                Email = "notfound@email.com"
            };

            var result = await _controller.Update(Guid.NewGuid(), updated);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //9. Test xóa ban đào tạo thành công
        [Test]
        public async Task Delete_ValidId_DeletesSuccessfully()
        {
            var bdt = new BanDaoTao { TenBanDaoTao = "BDT8", MaBanDaoTao = "BDT_8", Email = "g@email.com" };
            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(bdt.IdBanDaoTao);
            Assert.IsInstanceOf<NoContentResult>(result);

            var deleted = _context.BanDaoTaos.Find(bdt.IdBanDaoTao);
            Assert.IsNull(deleted);
        }

        //10. Test xóa với ID không tồn tại
        [Test]
        public async Task Delete_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.Delete(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //11. Test đổi trạng thái ban đào tạo
        [Test]
        public async Task ChangeStatus_ChangesTrangThai()
        {
            var bdt = new BanDaoTao { TenBanDaoTao = "BDT9", MaBanDaoTao = "BDT_9", Email = "h@email.com", TrangThai = true };
            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeStatus(bdt.IdBanDaoTao);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var bdtDb = _context.BanDaoTaos.Find(bdt.IdBanDaoTao);
            Assert.IsFalse(bdtDb.TrangThai);
        }

        //12. Test đổi trạng thái với ID không tồn tại
        [Test]
        public async Task ChangeStatus_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.ChangeStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //13. Test lọc theo trạng thái (active)
        [Test]
        public async Task GetPaged_FilterByActiveStatus_ReturnsOnlyActive()
        {
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "BDT10", MaBanDaoTao = "BDT_10", Email = "i@email.com", TrangThai = true });
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "BDT11", MaBanDaoTao = "BDT_11", Email = "j@email.com", TrangThai = false });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(1, 10, null, "active");
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Parse anonymous object bằng Newtonsoft.Json
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = JObject.Parse(json);
            var list = jObj["data"].ToObject<List<BanDaoTao>>();

            Assert.IsTrue(list.All(x => x.TrangThai));
        }

        //14. Test lọc theo trạng thái (inactive)
        [Test]
        public async Task GetPaged_FilterByInactiveStatus_ReturnsOnlyInactive()
        {
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "BDT12", MaBanDaoTao = "BDT_12", Email = "k@email.com", TrangThai = false });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(1, 10, null, "inactive");
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Parse anonymous object bằng Newtonsoft.Json
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = JObject.Parse(json);
            var list = jObj["data"].ToObject<List<BanDaoTao>>();

            Assert.IsTrue(list.All(x => !x.TrangThai));
        }

        //15. Test lọc theo tên hoặc mã
        [Test]
        public async Task GetPaged_FilterBySearch_ReturnsCorrect()
        {
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "BDT13", MaBanDaoTao = "BDT_13", Email = "l@email.com" });
            _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = "Khác", MaBanDaoTao = "OTHER", Email = "m@email.com" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(1, 10, "BDT_13", null);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Parse anonymous object bằng Newtonsoft.Json
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = JObject.Parse(json);
            var list = jObj["data"].ToObject<List<BanDaoTao>>();

            Assert.IsTrue(list.All(x => x.MaBanDaoTao.Contains("BDT_13")));
        }

        //16. Test phân trang
        [Test]
        public async Task GetPaged_Paging_ReturnsCorrectPage()
        {
            for (int i = 0; i < 15; i++)
            {
                _context.BanDaoTaos.Add(new BanDaoTao { TenBanDaoTao = $"BDT{i}", MaBanDaoTao = $"BDT_{i}", Email = $"bdt{i}@email.com" });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(2, 10, null, null); // Trang 2, mỗi trang 10
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Parse anonymous object bằng Newtonsoft.Json
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = JObject.Parse(json);
            var list = jObj["data"].ToObject<List<BanDaoTao>>();

            Assert.AreEqual(5, list.Count);
        }

        //17. Test tạo mới với mã ban đào tạo trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Create_DuplicateMaBanDaoTao_AllowsOrRejects()
        {
            var bdt1 = new BanDaoTao { TenBanDaoTao = "BDT14", MaBanDaoTao = "BDT_DUP", Email = "n@email.com" };
            var bdt2 = new BanDaoTao { TenBanDaoTao = "BDT15", MaBanDaoTao = "BDT_DUP", Email = "o@email.com" };

            await _controller.Create(bdt1);
            var result = await _controller.Create(bdt2);

            // Nếu bạn kiểm tra trùng, nên trả về BadRequest hoặc ViewResult với lỗi
            // Assert.IsInstanceOf<BadRequestObjectResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        //18. Test cập nhật chỉ một trường (partial update)

        [Test]
        public async Task Update_OnlyTenBanDaoTao_UpdatesSuccessfully()
        {
            var bdt = new BanDaoTao { TenBanDaoTao = "BDT16", MaBanDaoTao = "BDT_16", Email = "p@email.com" };
            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var updated = new BanDaoTao
            {
                TenBanDaoTao = "BDT16-updated",
                MaBanDaoTao = bdt.MaBanDaoTao,
                Email = bdt.Email,
                TrangThai = bdt.TrangThai
            };

            var result = await _controller.Update(bdt.IdBanDaoTao, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var bdtDb = _context.BanDaoTaos.Find(bdt.IdBanDaoTao);
            Assert.AreEqual("BDT16-updated", bdtDb.TenBanDaoTao);
            Assert.AreEqual("BDT_16", bdtDb.MaBanDaoTao);
        }

        //19. Test cập nhật ngày cập nhật (NgayCapNhat) thay đổi
        [Test]
        public async Task Update_UpdatesNgayCapNhat()
        {
            var bdt = new BanDaoTao { TenBanDaoTao = "BDT17", MaBanDaoTao = "BDT_17", Email = "q@email.com" };
            _context.BanDaoTaos.Add(bdt);
            await _context.SaveChangesAsync();

            var oldNgayCapNhat = bdt.NgayCapNhat;

            var updated = new BanDaoTao
            {
                TenBanDaoTao = "BDT17-updated",
                MaBanDaoTao = "BDT_17-updated",
                Email = "q-updated@email.com",
                TrangThai = bdt.TrangThai
            };

            var result = await _controller.Update(bdt.IdBanDaoTao, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var bdtDb = _context.BanDaoTaos.Find(bdt.IdBanDaoTao);
            Assert.IsNotNull(bdtDb.NgayCapNhat);
            if (oldNgayCapNhat != null)
                Assert.IsTrue(bdtDb.NgayCapNhat >= oldNgayCapNhat);
        }

        //20. Test lấy tất cả khi không có dữ liệu
        [Test]
        public async Task GetPaged_Empty_ReturnsEmptyList()
        {
            var result = await _controller.GetPaged();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Parse anonymous object bằng Newtonsoft.Json
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = JObject.Parse(json);
            var list = jObj["data"].ToObject<List<BanDaoTao>>(); // hoặc List<object> nếu không cần kiểu cụ thể

            Assert.AreEqual(0, list.Count);
        }
    }
}
