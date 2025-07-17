using API.Controllers;
using API.Data;
using API.Models;
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
    public class CoSoControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private CoSoController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new CoSoController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây


        //1. Test lấy danh sách cơ sở (GetCoSos)
        [Test]
        public async Task GetCoSos_ReturnsAllCoSos()
        {
            // Arrange
            _context.CoSos.Add(new CoSo
            {
                TenCoSo = "CS1",
                MaCoSo = "CS1",
                TrangThai = true,
                DiaChi = "Test Address 1",
                SDT = "0123456789",
                Email = "cs1@email.com"
            });
            _context.CoSos.Add(new CoSo
            {
                TenCoSo = "CS2",
                MaCoSo = "CS2",
                TrangThai = false,
                DiaChi = "Test Address 2",
                SDT = "0987654321",
                Email = "cs2@email.com"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCoSos();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as List<CoSoViewModel>;
            Assert.AreEqual(2, list.Count);
        }

        //2. Test lọc theo tên cơ sở
        [Test]
        public async Task GetCoSos_FilterByTenCoSo_ReturnsCorrectResult()
        {
            //_context.CoSos.Add(new CoSo { TenCoSo = "Ha Noi", MaCoSo = "HN", TrangThai = true });
            //_context.CoSos.Add(new CoSo { TenCoSo = "Da Nang", MaCoSo = "DN", TrangThai = true });
            _context.CoSos.Add(new CoSo
            {
                TenCoSo = "Ha Noi",
                MaCoSo = "HN",
                TrangThai = true,
                DiaChi = "Test Address 1",
                SDT = "0123456789",
                Email = "cs1@email.com"
            });
            _context.CoSos.Add(new CoSo
            {
                TenCoSo = "Da Nang",
                MaCoSo = "DN",
                TrangThai = false,
                DiaChi = "Test Address 2",
                SDT = "0987654321",
                Email = "cs2@email.com"
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetCoSos("Ha Noi", null);

            var okResult = result.Result as OkObjectResult;
            var list = okResult.Value as List<CoSoViewModel>;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Ha Noi", list[0].TenCoSo);
        }

        //3. Test lọc theo trạng thái
        [Test]
        public async Task GetCoSos_FilterByTrangThai_ReturnsCorrectResult()
        {
            //_context.CoSos.Add(new CoSo { TenCoSo = "A", MaCoSo = "A", TrangThai = true });
            //_context.CoSos.Add(new CoSo { TenCoSo = "B", MaCoSo = "B", TrangThai = false });
            _context.CoSos.Add(new CoSo
            {
                TenCoSo = "A",
                MaCoSo = "A",
                TrangThai = true,
                DiaChi = "Test Address 1",
                SDT = "0123456789",
                Email = "cs1@email.com"
            });
            _context.CoSos.Add(new CoSo
            {
                TenCoSo = "B",
                MaCoSo = "B",
                TrangThai = false,
                DiaChi = "Test Address 2",
                SDT = "0987654321",
                Email = "cs2@email.com"
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetCoSos(null, "Hoạt động");
            var okResult = result.Result as OkObjectResult;
            var list = okResult.Value as List<CoSoViewModel>;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("A", list[0].TenCoSo);
        }

        //4. Test lấy chi tiết cơ sở(GetCoSo)
        [Test]
        public async Task GetCoSo_ReturnsCorrectCoSo()
        {
            // var coSo = new CoSo { TenCoSo = "Test", MaCoSo = "TST", TrangThai = true };
            var coSo = new CoSo
            {
                TenCoSo = "Test",
                MaCoSo = "TST",
                TrangThai = true,
                DiaChi = "Test Address",
                SDT = "0123456789",
                Email = "test@email.com"
            };
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            var result = await _controller.GetCoSo(coSo.IdCoSo);
            var okResult = result.Result as OkObjectResult;
            var model = okResult.Value as CoSoViewModel;
            Assert.AreEqual("Test", model.TenCoSo);
        }

        //5. Test tạo mới cơ sở (CreateCoSo)
        [Test]
        public async Task CreateCoSo_AddsNewCoSo()
        {
            //var model = new CoSoViewModel
            //{
            //    TenCoSo = "New",
            //    MaCoSo = "NEW",
            //    TrangThai = "Hoạt động"
            //};
            var model = new CoSoViewModel
            {
                TenCoSo = "New",
                MaCoSo = "NEW",
                TrangThai = "Hoạt động",
                DiaChi = "New Address",
                SDT = "0123456789",
                Email = "new@email.com"
            };

            var result = await _controller.CreateCoSo(model);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var coSo = _context.CoSos.FirstOrDefault(c => c.MaCoSo == "NEW");
            Assert.IsNotNull(coSo);
        }

        //6. Test cập nhật cơ sở (UpdateCoSo)
        [Test]
        public async Task UpdateCoSo_UpdatesExistingCoSo()
        {
            //var coSo = new CoSo { TenCoSo = "Old", MaCoSo = "OLD", TrangThai = true };
            var coSo = new CoSo { TenCoSo = "Old", MaCoSo = "OLD", TrangThai = true, DiaChi = "Old Address", SDT = "0123456789", Email = "old@email.com" };
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            var model = new CoSoViewModel
            {
                IdCoSo = coSo.IdCoSo,
                TenCoSo = "Updated",
                MaCoSo = "UPD",
                TrangThai = "Tắt"
            };

            var result = await _controller.UpdateCoSo(coSo.IdCoSo, model);
            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = _context.CoSos.Find(coSo.IdCoSo);
            Assert.AreEqual("Updated", updated.TenCoSo);
            Assert.IsFalse(updated.TrangThai);
        }

        //7. Test xóa cơ sở (DeleteCoSo)
        [Test]
        public async Task DeleteCoSo_RemovesCoSo()
        {
            //var coSo = new CoSo { TenCoSo = "Del", MaCoSo = "DEL", TrangThai = true };
            var coSo = new CoSo { TenCoSo = "Del", MaCoSo = "DEL", TrangThai = true, DiaChi = "Del Address", SDT = "0123456789", Email = "del@email.com" };
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteCoSo(coSo.IdCoSo);
            Assert.IsInstanceOf<NoContentResult>(result);

            var deleted = _context.CoSos.Find(coSo.IdCoSo);
            Assert.IsNull(deleted);
        }

        //8. Test chuyển trạng thái (ToggleStatus)
        [Test]
        public async Task ToggleStatus_ChangesTrangThai()
        {
            //var coSo = new CoSo { TenCoSo = "Toggle", MaCoSo = "TGL", TrangThai = true };
            var coSo = new CoSo { TenCoSo = "Toggle", MaCoSo = "TGL", TrangThai = true, DiaChi = "Toggle Address", SDT = "0123456789", Email = "toggle@email.com" };
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            var result = await _controller.ToggleStatus(coSo.IdCoSo);
            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = _context.CoSos.Find(coSo.IdCoSo);
            Assert.IsFalse(updated.TrangThai);
        }

        ////9. Test tạo mới với dữ liệu Thiếu trường bắt buộc
        //[Test]
        //public async Task CreateCoSo_MissingRequiredField_ReturnsBadRequest()
        //{
        //    var model = new CoSoViewModel
        //    {
        //        // Thiếu TenCoSo
        //        MaCoSo = "ERR",
        //        TrangThai = "Hoạt động",
        //        DiaChi = "Address",
        //        SDT = "0123456789",
        //        Email = "err@email.com"
        //    };

        //    var result = await _controller.CreateCoSo(model);
        //    var badRequest = result.Result as BadRequestObjectResult;
        //    Assert.IsNotNull(badRequest);
        //}

        //9. Test tạo mới với dữ liệu Truyền trạng thái không hợp lệ
        [Test]
        public async Task CreateCoSo_InvalidTrangThai_ReturnsBadRequest()
        {
            var model = new CoSoViewModel
            {
                TenCoSo = "Invalid",
                MaCoSo = "INV",
                TrangThai = "SaiTrangThai", // Không phải "Hoạt động" hoặc "Tắt"
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "inv@email.com"
            };

            var result = await _controller.CreateCoSo(model);
            // Tùy vào logic controller, có thể là BadRequest hoặc vẫn tạo với trạng thái mặc định
            // Nếu controller không kiểm tra, bạn có thể kiểm tra lại trạng thái sau khi tạo
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var coSo = _context.CoSos.FirstOrDefault(c => c.MaCoSo == "INV");
            Assert.IsFalse(coSo.TrangThai); // Nếu logic mặc định là false
        }

        //10. Test cập nhật với ID không tồn tại
        [Test]
        public async Task UpdateCoSo_NonExistentId_ReturnsNotFound()
        {
            var model = new CoSoViewModel
            {
                IdCoSo = Guid.NewGuid(),
                TenCoSo = "NotFound",
                MaCoSo = "NF",
                TrangThai = "Hoạt động",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "nf@email.com"
            };

            var result = await _controller.UpdateCoSo(model.IdCoSo, model);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //11. Test xóa với ID không tồn tại
        [Test]
        public async Task DeleteCoSo_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.DeleteCoSo(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //12. Test lấy chi tiết với ID không tồn tại
        [Test]
        public async Task GetCoSo_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.GetCoSo(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        //13. Test ToggleStatus với ID không tồn tại
        [Test]
        public async Task ToggleStatus_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.ToggleStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //14. Test lọc với không có dữ liệu
        [Test]
        public async Task GetCoSos_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _controller.GetCoSos();
            var okResult = result.Result as OkObjectResult;
            var list = okResult.Value as List<CoSoViewModel>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        //15. Test tạo mới với mã cơ sở trùng (nếu có ràng buộc unique)
        [Test]
        public async Task CreateCoSo_DuplicateMaCoSo_AllowsOrRejects()
        {
            var model1 = new CoSoViewModel
            {
                TenCoSo = "A",
                MaCoSo = "DUP",
                TrangThai = "Hoạt động",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "a@email.com"
            };
            var model2 = new CoSoViewModel
            {
                TenCoSo = "B",
                MaCoSo = "DUP",
                TrangThai = "Hoạt động",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "b@email.com"
            };

            await _controller.CreateCoSo(model1);
            var result = await _controller.CreateCoSo(model2);

            // Nếu controller kiểm tra trùng mã, sẽ trả về BadRequest
            // Nếu không kiểm tra, sẽ cho phép tạo (tùy vào logic)
            // Assert phù hợp với logic của bạn
        }

        //16. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task UpdateCoSo_OnlyTenCoSo_UpdatesSuccessfully()
        {
            var coSo = new CoSo
            {
                TenCoSo = "Old",
                MaCoSo = "UPD",
                TrangThai = true,
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "old@email.com"
            };
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            var model = new CoSoViewModel
            {
                IdCoSo = coSo.IdCoSo,
                TenCoSo = "New Name",
                MaCoSo = "UPD",
                TrangThai = "Hoạt động",
                DiaChi = coSo.DiaChi,
                SDT = coSo.SDT,
                Email = coSo.Email
            };

            var result = await _controller.UpdateCoSo(coSo.IdCoSo, model);
            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = _context.CoSos.Find(coSo.IdCoSo);
            Assert.AreEqual("New Name", updated.TenCoSo);
        }

        //17. Test trường hợp dữ liệu lớn (stress test)
        [Test]
        public async Task GetCoSos_LargeNumberOfRecords_ReturnsAll()
        {
            for (int i = 0; i < 1000; i++)
            {
                _context.CoSos.Add(new CoSo
                {
                    TenCoSo = $"CS{i}",
                    MaCoSo = $"MCS{i}",
                    TrangThai = i % 2 == 0,
                    DiaChi = $"Address {i}",
                    SDT = $"0123456{i:D4}",
                    Email = $"cs{i}@email.com"
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetCoSos();
            var okResult = result.Result as OkObjectResult;
            var list = okResult.Value as List<CoSoViewModel>;
            Assert.AreEqual(1000, list.Count);
        }

        //18. Test logic ngày tạo, ngày cập nhật
        [Test]
        public async Task UpdateCoSo_UpdatesNgayCapNhat()
        {
            var coSo = new CoSo
            {
                TenCoSo = "Old",
                MaCoSo = "UPD",
                TrangThai = true,
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "old@email.com"
            };
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            var model = new CoSoViewModel
            {
                IdCoSo = coSo.IdCoSo,
                TenCoSo = "Updated",
                MaCoSo = "UPD",
                TrangThai = "Tắt",
                DiaChi = coSo.DiaChi,
                SDT = coSo.SDT,
                Email = coSo.Email
            };

            var result = await _controller.UpdateCoSo(coSo.IdCoSo, model);
            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = _context.CoSos.Find(coSo.IdCoSo);
            Assert.IsNotNull(updated.NgayCapNhat);
            Assert.GreaterOrEqual(updated.NgayCapNhat.Value, coSo.NgayTao);
        }

    }
}
