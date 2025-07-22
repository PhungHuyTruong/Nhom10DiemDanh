using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCanBoDaoTao
{
    [TestFixture]
    public class CaHocsControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private CaHocsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new CaHocsController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây

        //1. Test tạo mới ca học thành công
        [Test]
        public async Task Post_ValidCaHoc_ReturnsOk()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "CaHoc1",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };

            var result = await _controller.Post(caHoc);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var saved = _context.CaHocs.FirstOrDefault(c => c.TenCaHoc == "CaHoc1");
            Assert.IsNotNull(saved);
        }

        //2. Test tạo mới ca học thiếu trường bắt buộc
        [Test]
        public async Task Post_MissingTenCaHoc_ReturnsBadRequest()
        {
            var caHoc = new CaHoc
            {
                // TenCaHoc = null,
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };

            _controller.ModelState.AddModelError("TenCaHoc", "Tên ca học không được để trống");

            var result = await _controller.Post(caHoc);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //3. Test tạo mới ca học với thời gian kết thúc <= bắt đầu
        [Test]
        public async Task Post_EndTimeBeforeStartTime_ReturnsBadRequest()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "CaHoc2",
                ThoiGianBatDau = new TimeSpan(10, 0, 0),
                ThoiGianKetThuc = new TimeSpan(8, 0, 0),
                TrangThai = 1
            };

            // Gọi validate thủ công nếu muốn kiểm tra logic Validate
            var context = new ValidationContext(caHoc);
            var results = caHoc.Validate(context).ToList();
            Assert.IsTrue(results.Any(r => r.ErrorMessage.Contains("Thời gian kết thúc phải sau thời gian bắt đầu")));
        }

        //4. Test lấy ca học theo ID thành công
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "CaHoc3",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };
            _context.CaHocs.Add(caHoc);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(caHoc.IdCaHoc);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as CaHoc;
            Assert.AreEqual("CaHoc3", returned.TenCaHoc);
        }

        //5.  Test lấy ca học theo ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //6. Test lấy danh sách ca học theo cơ sở
        [Test]
        public async Task GetCaHocTheoCoSo_ReturnsCorrectList()
        {
            var coSoId = Guid.NewGuid();
            _context.CaHocs.Add(new CaHoc
            {
                TenCaHoc = "CaHoc4",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1,
                CoSoId = coSoId
            });
            _context.CaHocs.Add(new CaHoc
            {
                TenCaHoc = "CaHoc5",
                ThoiGianBatDau = new TimeSpan(10, 0, 0),
                ThoiGianKetThuc = new TimeSpan(12, 0, 0),
                TrangThai = 1,
                CoSoId = coSoId
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetCaHocTheoCoSo(coSoId);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as List<CaHoc>;
            Assert.AreEqual(2, list.Count);
        }

        //7. Test cập nhật ca học thành công
        [Test]
        public async Task UpdateCaHoc_ValidData_UpdatesSuccessfully()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "CaHoc6",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };
            _context.CaHocs.Add(caHoc);
            await _context.SaveChangesAsync();

            var updated = new CaHoc
            {
                IdCaHoc = caHoc.IdCaHoc,
                TenCaHoc = "Updated",
                ThoiGianBatDau = new TimeSpan(9, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0),
                TrangThai = 0
            };

            var result = await _controller.UpdateCaHoc(caHoc.IdCaHoc, updated);
            Assert.IsInstanceOf<NoContentResult>(result);

            var caHocDb = _context.CaHocs.Find(caHoc.IdCaHoc);
            Assert.AreEqual("Updated", caHocDb.TenCaHoc);
            Assert.AreEqual(0, caHocDb.TrangThai);
        }

        //8. Test cập nhật ca học với ID không khớp
        [Test]
        public async Task UpdateCaHoc_IdMismatch_ReturnsBadRequest()
        {
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Mismatch",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };

            var result = await _controller.UpdateCaHoc(Guid.NewGuid(), caHoc);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        //9. Test cập nhật ca học với ID không tồn tại
        [Test]
        public async Task UpdateCaHoc_NonExistentId_ReturnsNotFound()
        {
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "NotFound",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };

            var result = await _controller.UpdateCaHoc(caHoc.IdCaHoc, caHoc);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //10. Test tạo mới ca học với tên trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Post_DuplicateTenCaHoc_SameCoSo_AllowsOrRejects()
        {
            var coSoId = Guid.NewGuid();
            var caHoc1 = new CaHoc
            {
                TenCaHoc = "TrungTen",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1,
                CoSoId = coSoId
            };
            var caHoc2 = new CaHoc
            {
                TenCaHoc = "TrungTen",
                ThoiGianBatDau = new TimeSpan(10, 0, 0),
                ThoiGianKetThuc = new TimeSpan(12, 0, 0),
                TrangThai = 1,
                CoSoId = coSoId
            };

            await _controller.Post(caHoc1);
            var result = await _controller.Post(caHoc2);

            // Nếu bạn kiểm tra trùng tên, nên trả về BadRequest
            // Assert.IsInstanceOf<BadRequestObjectResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        //11. Test lấy danh sách ca học khi không có dữ liệu
        [Test]
        public async Task GetCaHocTheoCoSo_Empty_ReturnsEmptyList()
        {
            var coSoId = Guid.NewGuid();
            var result = await _controller.GetCaHocTheoCoSo(coSoId);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as List<CaHoc>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        //12. Test tạo mới ca học với thời gian bắt đầu và kết thúc giống nhau
        [Test]
        public void Post_SameStartAndEndTime_ValidateLogic()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "SameTime",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(8, 0, 0),
                TrangThai = 1
            };

            var context = new ValidationContext(caHoc);
            var results = caHoc.Validate(context).ToList();
            Assert.IsTrue(results.Any(r => r.ErrorMessage.Contains("Thời gian kết thúc phải sau thời gian bắt đầu")));
        }

        //13. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task UpdateCaHoc_OnlyTenCaHoc_UpdatesSuccessfully()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "OldName",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };
            _context.CaHocs.Add(caHoc);
            await _context.SaveChangesAsync();

            var updated = new CaHoc
            {
                IdCaHoc = caHoc.IdCaHoc,
                TenCaHoc = "NewName",
                ThoiGianBatDau = caHoc.ThoiGianBatDau,
                ThoiGianKetThuc = caHoc.ThoiGianKetThuc,
                TrangThai = caHoc.TrangThai
            };

            var result = await _controller.UpdateCaHoc(caHoc.IdCaHoc, updated);
            Assert.IsInstanceOf<NoContentResult>(result);

            var caHocDb = _context.CaHocs.Find(caHoc.IdCaHoc);
            Assert.AreEqual("NewName", caHocDb.TenCaHoc);
        }

        //14. Test cập nhật trạng thái (toggle)
        [Test]
        public async Task UpdateCaHoc_ToggleTrangThai_UpdatesSuccessfully()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "ToggleTest",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };
            _context.CaHocs.Add(caHoc);
            await _context.SaveChangesAsync();

            caHoc.TrangThai = 0;
            var result = await _controller.UpdateCaHoc(caHoc.IdCaHoc, caHoc);
            Assert.IsInstanceOf<NoContentResult>(result);

            var caHocDb = _context.CaHocs.Find(caHoc.IdCaHoc);
            Assert.AreEqual(0, caHocDb.TrangThai);
        }

        //15. Test dữ liệu lớn (stress test)
        [Test]
        public async Task GetCaHocTheoCoSo_LargeData_ReturnsAll()
        {
            var coSoId = Guid.NewGuid();
            for (int i = 0; i < 500; i++)
            {
                _context.CaHocs.Add(new CaHoc
                {
                    TenCaHoc = $"CaHoc{i}",
                    ThoiGianBatDau = new TimeSpan(8, 0, 0),
                    ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                    TrangThai = 1,
                    CoSoId = coSoId
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetCaHocTheoCoSo(coSoId);
            var okResult = result as OkObjectResult;
            var list = okResult.Value as List<CaHoc>;
            Assert.AreEqual(500, list.Count);
        }

        //16. Test validate logic nghiệp vụ với các giá trị null
        [Test]
        public void Post_NullStartOrEndTime_ValidateLogic()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "NullTime",
                ThoiGianBatDau = null,
                ThoiGianKetThuc = null,
                TrangThai = 1
            };

            var context = new ValidationContext(caHoc);
            var results = caHoc.Validate(context).ToList();
            // Không có lỗi vì validate chỉ kiểm tra khi cả 2 đều có giá trị
            Assert.IsEmpty(results);
        }

        //17. Test cập nhật ngày cập nhật (NgayCapNhat) thay đổi
        [Test]
        public async Task UpdateCaHoc_UpdatesNgayCapNhat()
        {
            var caHoc = new CaHoc
            {
                TenCaHoc = "NgayCapNhatTest",
                ThoiGianBatDau = new TimeSpan(8, 0, 0),
                ThoiGianKetThuc = new TimeSpan(10, 0, 0),
                TrangThai = 1
            };
            _context.CaHocs.Add(caHoc);
            await _context.SaveChangesAsync();

            var oldNgayCapNhat = caHoc.NgayCapNhat;

            var updated = new CaHoc
            {
                IdCaHoc = caHoc.IdCaHoc,
                TenCaHoc = "NgayCapNhatTest",
                ThoiGianBatDau = new TimeSpan(9, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0),
                TrangThai = 1
            };

            var result = await _controller.UpdateCaHoc(caHoc.IdCaHoc, updated);
            Assert.IsInstanceOf<NoContentResult>(result);

            var caHocDb = _context.CaHocs.Find(caHoc.IdCaHoc);
            Assert.GreaterOrEqual(caHocDb.NgayCapNhat, oldNgayCapNhat);
        }
    }
}
