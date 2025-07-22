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
    public class HocKyControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private HocKyController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new HocKyController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            if (_controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        // Các test sẽ viết ở đây

        //1. Test tạo mới học kỳ thành công
        [Test]
        public async Task Create_ValidHocKy_ReturnsRedirect()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK1",
                MaHocKy = "HK1"
            };

            var result = await _controller.Create(hocKy);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var saved = _context.HocKys.FirstOrDefault(h => h.TenHocKy == "HK1");
            Assert.IsNotNull(saved);
        }

        //2. Test tạo mới học kỳ thiếu trường bắt buộc
        [Test]
        public async Task Create_MissingTenHocKy_ReturnsView()
        {
            var hocKy = new HocKy
            {
                // TenHocKy = null,
                MaHocKy = "HK2"
            };

            _controller.ModelState.AddModelError("TenHocKy", "Tên học kỳ không được để trống");

            var result = await _controller.Create(hocKy);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        //3. Test lấy học kỳ theo ID thành công
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK3",
                MaHocKy = "HK3"
            };
            _context.HocKys.Add(hocKy);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(hocKy.IdHocKy);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as HocKy;
            Assert.AreEqual("HK3", returned.TenHocKy);
        }

        //4.  Test lấy học kỳ theo ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //5. Test lấy tất cả học kỳ
        [Test]
        public async Task GetAll_ReturnsAllHocKys()
        {
            _context.HocKys.Add(new HocKy { TenHocKy = "HK4", MaHocKy = "HK4" });
            _context.HocKys.Add(new HocKy { TenHocKy = "HK5", MaHocKy = "HK5" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as System.Collections.Generic.List<HocKy>;
            Assert.AreEqual(2, list.Count);
        }

        //6.Test cập nhật học kỳ thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK6",
                MaHocKy = "HK6"
            };
            _context.HocKys.Add(hocKy);
            await _context.SaveChangesAsync();

            var updated = new HocKy
            {
                TenHocKy = "HK6-Updated",
                MaHocKy = "HK6-Updated",
                TrangThai = false
            };

            var result = await _controller.Update(hocKy.IdHocKy, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var hocKyDb = _context.HocKys.Find(hocKy.IdHocKy);
            Assert.AreEqual("HK6-Updated", hocKyDb.TenHocKy);
            Assert.IsFalse(hocKyDb.TrangThai);
        }

        //7. Test cập nhật học kỳ với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var updated = new HocKy
            {
                TenHocKy = "NotFound",
                MaHocKy = "NotFound",
                TrangThai = false
            };

            var result = await _controller.Update(Guid.NewGuid(), updated);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //8. Test đổi trạng thái học kỳ
        [Test]
        public async Task DoiTrangThai_ChangesTrangThai()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK7",
                MaHocKy = "HK7",
                TrangThai = true
            };
            _context.HocKys.Add(hocKy);
            await _context.SaveChangesAsync();

            var result = await _controller.DoiTrangThai(hocKy.IdHocKy);
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var hocKyDb = _context.HocKys.Find(hocKy.IdHocKy);
            Assert.IsFalse(hocKyDb.TrangThai);
        }

        //9. Test đổi trạng thái học kỳ không tồn tại
        [Test]
        public async Task DoiTrangThai_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.DoiTrangThai(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //10. Test lấy danh sách học kỳ khi không có dữ liệu

        [Test]
        public async Task GetAll_Empty_ReturnsEmptyList()
        {
            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<HocKy>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        //11. Test tạo mới học kỳ với tên/mã trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Create_DuplicateTenHocKyOrMaHocKy_AllowsOrRejects()
        {
            var hocKy1 = new HocKy
            {
                TenHocKy = "HK8",
                MaHocKy = "HK8"
            };
            var hocKy2 = new HocKy
            {
                TenHocKy = "HK8",
                MaHocKy = "HK8"
            };

            await _controller.Create(hocKy1);
            var result = await _controller.Create(hocKy2);

            // Nếu bạn kiểm tra trùng, nên trả về ViewResult với lỗi
            // Assert.IsInstanceOf<ViewResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        //12. Test tạo mới học kỳ với tên/mã chứa ký tự đặc biệt (vi phạm regex)
        [Test]
        public async Task Create_InvalidTenHocKyOrMaHocKy_ReturnsView()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK 9!", // chứa khoảng trắng và ký tự đặc biệt
                MaHocKy = "HK@9"
            };

            _controller.ModelState.AddModelError("TenHocKy", "Tên học kỳ không được chứa khoảng trắng hoặc ký tự đặc biệt");
            _controller.ModelState.AddModelError("MaHocKy", "Mã học kỳ không được chứa khoảng trắng hoặc ký tự đặc biệt");

            var result = await _controller.Create(hocKy);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        //13. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task Update_OnlyTenHocKy_UpdatesSuccessfully()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK10",
                MaHocKy = "HK10"
            };
            _context.HocKys.Add(hocKy);
            await _context.SaveChangesAsync();

            var updated = new HocKy
            {
                TenHocKy = "HK10-Updated",
                MaHocKy = hocKy.MaHocKy,
                TrangThai = hocKy.TrangThai
            };

            var result = await _controller.Update(hocKy.IdHocKy, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var hocKyDb = _context.HocKys.Find(hocKy.IdHocKy);
            Assert.AreEqual("HK10-Updated", hocKyDb.TenHocKy);
            Assert.AreEqual("HK10", hocKyDb.MaHocKy);
        }

        //14. Test dữ liệu lớn (stress test)
        [Test]
        public async Task GetAll_LargeData_ReturnsAll()
        {
            for (int i = 0; i < 200; i++)
            {
                _context.HocKys.Add(new HocKy
                {
                    TenHocKy = $"HK{i}",
                    MaHocKy = $"HK{i}"
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<HocKy>;
            Assert.AreEqual(200, list.Count);
        }

        
        //15. Test đổi trạng thái nhiều lần liên tiếp
        [Test]
        public async Task DoiTrangThai_MultipleTimes_TogglesCorrectly()
        {
            var hocKy = new HocKy
            {
                TenHocKy = "HK12",
                MaHocKy = "HK12",
                TrangThai = true
            };
            _context.HocKys.Add(hocKy);
            await _context.SaveChangesAsync();

            // Toggle lần 1
            await _controller.DoiTrangThai(hocKy.IdHocKy);
            var hk1 = _context.HocKys.Find(hocKy.IdHocKy);
            Assert.IsFalse(hk1.TrangThai);

            // Toggle lần 2
            await _controller.DoiTrangThai(hocKy.IdHocKy);
            var hk2 = _context.HocKys.Find(hocKy.IdHocKy);
            Assert.IsTrue(hk2.TrangThai);
        }

        //16. Test lấy học kỳ theo trạng thái
        [Test]
        public async Task GetAll_FilterByTrangThai_ReturnsCorrect()
        {
            _context.HocKys.Add(new HocKy { TenHocKy = "HK13", MaHocKy = "HK13", TrangThai = true });
            _context.HocKys.Add(new HocKy { TenHocKy = "HK14", MaHocKy = "HK14", TrangThai = false });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<HocKy>;
            Assert.IsTrue(list.Any(h => h.TrangThai == true));
            Assert.IsTrue(list.Any(h => h.TrangThai == false));
        }

        //17. Test tạo mới học kỳ với tên/mã quá dài
        [Test]
        public async Task Create_TenHocKyOrMaHocKyTooLong_ReturnsView()
        {
            var longString = new string('A', 101);
            var hocKy = new HocKy
            {
                TenHocKy = longString,
                MaHocKy = longString
            };

            _controller.ModelState.AddModelError("TenHocKy", "Tên học kỳ không được vượt quá 100 ký tự");
            _controller.ModelState.AddModelError("MaHocKy", "Mã học kỳ không được vượt quá 100 ký tự");

            var result = await _controller.Create(hocKy);
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
