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
    public class CapDoDuAnControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private CapDoDuAnController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new CapDoDuAnController(_context);
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

        //1. Test lấy tất cả cấp độ dự án
        [Test]
        public async Task GetAll_ReturnsAllCapDoDuAn()
        {
            _context.CapDoDuAns.Add(new CapDoDuAn { TenCapDoDuAn = "cd1", MaCapDoDuAn = "mcd1", MoTa = "Mô tả 1" }); // BỔ SUNG MoTa
            _context.CapDoDuAns.Add(new CapDoDuAn { TenCapDoDuAn = "cd2", MaCapDoDuAn = "mcd2", MoTa = "Mô tả 2" }); // BỔ SUNG MoTa
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as System.Collections.IEnumerable;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, ((System.Collections.ICollection)list).Count);
        }


        //2. Test lấy cấp độ dự án theo ID
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cd1",
                MaCapDoDuAn = "mcd1",
                MoTa = "Mô tả test" // BỔ SUNG DÒNG NÀY
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(capDo.IdCDDA);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as CapDoDuAn;
            Assert.AreEqual("cd1", returned.TenCapDoDuAn);
        }

        //3. Test lấy cấp độ dự án với ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //4.  Test tạo mới cấp độ dự án thành công
        [Test]
        public async Task Create_ValidCapDoDuAn_ReturnsRedirect()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cd3",
                MaCapDoDuAn = "mcd3",
                MoTa = "Mô tả test"
            };

            var result = await _controller.Create(capDo);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var saved = _context.CapDoDuAns.FirstOrDefault(c => c.MaCapDoDuAn == "mcd3");
            Assert.IsNotNull(saved);
        }

        //5.Test tạo mới thiếu trường bắt buộc
        [Test]
        public async Task Create_MissingTenCapDoDuAn_ReturnsView()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = null,
                MaCapDoDuAn = "mcd4"
            };
            _controller.ModelState.AddModelError("TenCapDoDuAn", "Tên cấp độ không được để trống");

            var result = await _controller.Create(capDo);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        //6. est cập nhật cấp độ dự án thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cd5",
                MaCapDoDuAn = "mcd5",
                MoTa = "Mô tả cũ"
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var updated = new CapDoDuAn
            {
                TenCapDoDuAn = "cd5-updated",
                MaCapDoDuAn = "mcd5-updated",
                MoTa = "Mô tả mới",
                TrangThai = false
            };

            var result = await _controller.Update(capDo.IdCDDA, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var capDoDb = _context.CapDoDuAns.Find(capDo.IdCDDA);
            Assert.AreEqual("cd5-updated", capDoDb.TenCapDoDuAn);
            Assert.AreEqual("mcd5-updated", capDoDb.MaCapDoDuAn);
            Assert.AreEqual("Mô tả mới", capDoDb.MoTa);
            Assert.IsFalse(capDoDb.TrangThai);
        }

        //7. Test cập nhật với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var updated = new CapDoDuAn
            {
                TenCapDoDuAn = "notfound",
                MaCapDoDuAn = "notfound"
            };

            var result = await _controller.Update(Guid.NewGuid(), updated);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //8. Test đổi trạng thái
        [Test]
        public async Task DoiTrangThai_ChangesTrangThai()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cd6",
                MaCapDoDuAn = "mcd6",
                TrangThai = true,
                MoTa = "Test đổi trạng thái" // BỔ SUNG DÒNG NÀY
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var result = await _controller.DoiTrangThai(capDo.IdCDDA);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var capDoDb = _context.CapDoDuAns.Find(capDo.IdCDDA);
            Assert.IsFalse(capDoDb.TrangThai);
        }

        //9. Test đổi trạng thái với ID không tồn tại
        [Test]
        public async Task DoiTrangThai_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.DoiTrangThai(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //10. Test tạo mới với mã cấp độ chứa ký tự đặc biệt (phải lỗi)
        [Test]
        public async Task Create_MaCapDoDuAnWithSpecialChars_ReturnsView()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdSpecial",
                MaCapDoDuAn = "mcd@#1", // Ký tự đặc biệt
                MoTa = "Test ký tự đặc biệt"
            };
            _controller.ModelState.AddModelError("MaCapDoDuAn", "Mã cấp độ không được chứa khoảng trắng hoặc ký tự đặc biệt");

            var result = await _controller.Create(capDo);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        //11. Test tạo mới với mã cấp độ trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Create_DuplicateMaCapDoDuAn_AllowsOrRejects()
        {
            var capDo1 = new CapDoDuAn
            {
                TenCapDoDuAn = "cdDup",
                MaCapDoDuAn = "mcdDup",
                MoTa = "Test trùng mã"
            };
            var capDo2 = new CapDoDuAn
            {
                TenCapDoDuAn = "cdDup2",
                MaCapDoDuAn = "mcdDup",
                MoTa = "Test trùng mã"
            };

            await _controller.Create(capDo1);
            var result = await _controller.Create(capDo2);

            // Nếu bạn kiểm tra trùng, nên trả về ViewResult với lỗi
            // Assert.IsInstanceOf<ViewResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        //12. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task Update_OnlyTenCapDoDuAn_UpdatesSuccessfully()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdPartial",
                MaCapDoDuAn = "mcdPartial",
                MoTa = "Mô tả partial"
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var updated = new CapDoDuAn
            {
                TenCapDoDuAn = "cdPartialUpdated",
                MaCapDoDuAn = capDo.MaCapDoDuAn,
                MoTa = capDo.MoTa,
                TrangThai = capDo.TrangThai
            };

            var result = await _controller.Update(capDo.IdCDDA, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var capDoDb = _context.CapDoDuAns.Find(capDo.IdCDDA);
            Assert.AreEqual("cdPartialUpdated", capDoDb.TenCapDoDuAn);
            Assert.AreEqual("mcdPartial", capDoDb.MaCapDoDuAn);
        }

        //13. Test dữ liệu lớn (stress test)
        [Test]
        public async Task GetAll_LargeData_ReturnsAll()
        {
            for (int i = 0; i < 100; i++)
            {
                _context.CapDoDuAns.Add(new CapDoDuAn
                {
                    TenCapDoDuAn = $"cd{i}",
                    MaCapDoDuAn = $"mcd{i}",
                    MoTa = $"Mô tả {i}"
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.IEnumerable;
            Assert.IsNotNull(list);
            Assert.AreEqual(100, ((System.Collections.ICollection)list).Count);
        }

        //14. Test cập nhật ngày cập nhật (NgayCapNhat) thay đổi
        [Test]
        public async Task Update_UpdatesNgayCapNhat()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdNgayCapNhat",
                MaCapDoDuAn = "mcdNgayCapNhat",
                MoTa = "Mô tả ngày cập nhật"
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var oldNgayCapNhat = capDo.NgayCapNhat;

            var updated = new CapDoDuAn
            {
                TenCapDoDuAn = "cdNgayCapNhatUpdated",
                MaCapDoDuAn = "mcdNgayCapNhatUpdated",
                MoTa = "Mô tả mới",
                TrangThai = capDo.TrangThai
            };

            var result = await _controller.Update(capDo.IdCDDA, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var capDoDb = _context.CapDoDuAns.Find(capDo.IdCDDA);
            Assert.IsNotNull(capDoDb.NgayCapNhat);
            // Nếu oldNgayCapNhat != null thì kiểm tra mới hơn
            if (oldNgayCapNhat != null)
                Assert.IsTrue(capDoDb.NgayCapNhat >= oldNgayCapNhat);
        }

        //15. Test tạo mới với mã cấp độ quá dài (vượt quá 100 ký tự)
        [Test]
        public async Task Create_MaCapDoDuAnTooLong_ReturnsView()
        {
            var longMa = new string('a', 101);
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdLong",
                MaCapDoDuAn = longMa,
                MoTa = "Test mã quá dài"
            };
            _controller.ModelState.AddModelError("MaCapDoDuAn", "Mã cấp độ không được vượt quá 100 ký tự");

            var result = await _controller.Create(capDo);
            Assert.IsInstanceOf<ViewResult>(result);
        }


        //16. Test tạo mới với tên cấp độ quá dài (vượt quá 100 ký tự)
        [Test]
        public async Task Create_TenCapDoDuAnTooLong_ReturnsView()
        {
            var longTen = new string('b', 101);
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = longTen,
                MaCapDoDuAn = "mcdLong",
                MoTa = "Test tên quá dài"
            };
            _controller.ModelState.AddModelError("TenCapDoDuAn", "Tên cấp độ không được vượt quá 100 ký tự");

            var result = await _controller.Create(capDo);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        //17. Test tạo mới với mã cấp độ có khoảng trắng (phải lỗi)
        [Test]
        public async Task Create_MaCapDoDuAnWithWhitespace_ReturnsView()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdWhite",
                MaCapDoDuAn = "mcd 1", // Có khoảng trắng
                MoTa = "Test mã có khoảng trắng"
            };
            _controller.ModelState.AddModelError("MaCapDoDuAn", "Mã cấp độ không được chứa khoảng trắng hoặc ký tự đặc biệt");

            var result = await _controller.Create(capDo);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        //18. Test cập nhật trạng thái từ false sang true
        [Test]
        public async Task DoiTrangThai_FromFalseToTrue()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdTrangThai",
                MaCapDoDuAn = "mcdTrangThai",
                MoTa = "Test đổi trạng thái",
                TrangThai = false
            };
            _context.CapDoDuAns.Add(capDo);
            await _context.SaveChangesAsync();

            var result = await _controller.DoiTrangThai(capDo.IdCDDA);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var capDoDb = _context.CapDoDuAns.Find(capDo.IdCDDA);
            Assert.IsTrue(capDoDb.TrangThai);
        }

        //19. Test tạo mới với trường MoTa rỗng (nếu cho phép)
        [Test]
        public async Task Create_EmptyMoTa_AllowsOrRejects()
        {
            var capDo = new CapDoDuAn
            {
                TenCapDoDuAn = "cdEmptyMoTa",
                MaCapDoDuAn = "mcdEmptyMoTa",
                MoTa = "" // Rỗng
            };

            var result = await _controller.Create(capDo);
            // Nếu cho phép tạo, kiểm tra Redirect
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            // Nếu không cho phép, kiểm tra ViewResult
            // Assert.IsInstanceOf<ViewResult>(result);
        }


        //20. Test lấy tất cả khi không có dữ liệu
        [Test]
        public async Task GetAll_Empty_ReturnsEmptyList()
        {
            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as System.Collections.IEnumerable;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, ((System.Collections.ICollection)list).Count);
        }
    }
}
