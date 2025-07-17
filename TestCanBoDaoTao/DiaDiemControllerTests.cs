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
    public class DiaDiemControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private DiaDiemController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new DiaDiemController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây

        //1. Test tạo mới địa điểm thành công
        [Test]
        public async Task Create_ValidDiaDiem_ReturnsOk()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@dd.com"
            });
            await _context.SaveChangesAsync();

            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm 1",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };

            var result = await _controller.Create(diaDiem);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var saved = _context.DiaDiems.FirstOrDefault(d => d.TenDiaDiem == "Địa điểm 1");
            Assert.IsNotNull(saved);
        }

        

        //2. Test lấy địa điểm theo ID thành công
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm 2",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(diaDiem.IdDiaDiem);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as DiaDiem;
            Assert.AreEqual("Địa điểm 2", returned.TenDiaDiem);
        }

        //3. Test lấy địa điểm theo ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //4. Test lấy danh sách địa điểm theo cơ sở
        [Test]
        public async Task GetByCoSo_ReturnsCorrectList()
        {
            var coSoId = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem
            {
                TenDiaDiem = "Địa điểm 3",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            });
            _context.DiaDiems.Add(new DiaDiem
            {
                TenDiaDiem = "Địa điểm 4",
                IdCoSo = coSoId,
                ViDo = 21.0286,
                KinhDo = 105.8543,
                BanKinh = 200,
                TrangThai = false
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetByCoSo(coSoId);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as System.Collections.Generic.List<DiaDiem>;
            Assert.AreEqual(2, list.Count);
        }

        //5. Test cập nhật địa điểm thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm 5",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            var updated = new DiaDiem
            {
                TenDiaDiem = "Địa điểm 5 - Updated",
                ViDo = 21.0290,
                KinhDo = 105.8550,
                BanKinh = 150,
                TrangThai = false
            };

            var result = await _controller.Update(diaDiem.IdDiaDiem, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var diaDiemDb = _context.DiaDiems.Find(diaDiem.IdDiaDiem);
            Assert.AreEqual("Địa điểm 5 - Updated", diaDiemDb.TenDiaDiem);
            Assert.IsFalse(diaDiemDb.TrangThai);
        }

        //6. Test cập nhật địa điểm với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var updated = new DiaDiem
            {
                TenDiaDiem = "NotFound",
                ViDo = 21.0290,
                KinhDo = 105.8550,
                BanKinh = 150,
                TrangThai = false
            };

            var result = await _controller.Update(Guid.NewGuid(), updated);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //7. Test đổi trạng thái địa điểm
        [Test]
        public async Task DoiTrangThai_ChangesTrangThai()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm 6",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            var result = await _controller.DoiTrangThai(diaDiem.IdDiaDiem);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var diaDiemDb = _context.DiaDiems.Find(diaDiem.IdDiaDiem);
            Assert.IsFalse(diaDiemDb.TrangThai);
        }

        //8. Test xóa địa điểm thành công
        [Test]
        public async Task Delete_ValidId_RemovesDiaDiem()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm 7",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(diaDiem.IdDiaDiem);
            Assert.IsInstanceOf<NoContentResult>(result);

            var deleted = _context.DiaDiems.Find(diaDiem.IdDiaDiem);
            Assert.IsNull(deleted);
        }


        //9. Test xóa địa điểm không tồn tại
        [Test]
        public async Task Delete_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.Delete(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        //10. Test lấy tất cả địa điểm
        [Test]
        public async Task GetAll_ReturnsAllDiaDiems()
        {
            var coSoId = Guid.NewGuid();
            _context.DiaDiems.Add(new DiaDiem
            {
                TenDiaDiem = "Địa điểm 8",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            });
            _context.DiaDiems.Add(new DiaDiem
            {
                TenDiaDiem = "Địa điểm 9",
                IdCoSo = coSoId,
                ViDo = 21.0286,
                KinhDo = 105.8543,
                BanKinh = 200,
                TrangThai = false
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as System.Collections.Generic.List<DiaDiem>;
            Assert.AreEqual(2, list.Count);
        }

        //11. Test lấy danh sách địa điểm khi không có dữ liệu
        [Test]
        public async Task GetAll_Empty_ReturnsEmptyList()
        {
            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<DiaDiem>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        //12. Test tạo mới địa điểm với tên trùng trong cùng một cơ sở (nếu có ràng buộc unique)
        [Test]
        public async Task Create_DuplicateTenDiaDiem_SameCoSo_AllowsOrRejects()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@dd.com"
            });
            await _context.SaveChangesAsync();

            var diaDiem1 = new DiaDiem
            {
                TenDiaDiem = "TrungTen",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            var diaDiem2 = new DiaDiem
            {
                TenDiaDiem = "TrungTen",
                IdCoSo = coSoId,
                ViDo = 21.0286,
                KinhDo = 105.8543,
                BanKinh = 200,
                TrangThai = true
            };

            await _controller.Create(diaDiem1);
            var result = await _controller.Create(diaDiem2);

            // Nếu bạn kiểm tra trùng tên, nên trả về BadRequest
            // Assert.IsInstanceOf<BadRequestResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        //13. Test tạo mới địa điểm với ViDo, KinhDo, BanKinh là null hoặc âm
        [Test]
        public async Task Create_NullOrNegativeCoordinates_ReturnsOkOrBadRequest()
        {
            var coSoId = Guid.NewGuid();
            _context.CoSos.Add(new CoSo
            {
                IdCoSo = coSoId,
                TenCoSo = "TestCoSo",
                MaCoSo = "TCS",
                DiaChi = "Address",
                SDT = "0123456789",
                Email = "test@dd.com"
            });
            await _context.SaveChangesAsync();

            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm biên",
                IdCoSo = coSoId,
                ViDo = null, // hoặc -90, 91, v.v.
                KinhDo = null, // hoặc -181, 181, v.v.
                BanKinh = -10, // hoặc null
                TrangThai = true
            };

            // Nếu bạn validate, nên trả về BadRequest
            // _controller.ModelState.AddModelError("ViDo", "Vĩ độ không hợp lệ");
            // _controller.ModelState.AddModelError("KinhDo", "Kinh độ không hợp lệ");
            // _controller.ModelState.AddModelError("BanKinh", "Bán kính phải lớn hơn 0");

            var result = await _controller.Create(diaDiem);
            // Assert.IsInstanceOf<BadRequestResult>(result);
        }


        //14. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task Update_OnlyTenDiaDiem_UpdatesSuccessfully()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "OldName",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            var updated = new DiaDiem
            {
                TenDiaDiem = "NewName",
                ViDo = diaDiem.ViDo,
                KinhDo = diaDiem.KinhDo,
                BanKinh = diaDiem.BanKinh,
                TrangThai = diaDiem.TrangThai
            };

            var result = await _controller.Update(diaDiem.IdDiaDiem, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var diaDiemDb = _context.DiaDiems.Find(diaDiem.IdDiaDiem);
            Assert.AreEqual("NewName", diaDiemDb.TenDiaDiem);
        }

        //15. Test dữ liệu lớn (stress test)

        [Test]
        public async Task GetAll_LargeData_ReturnsAll()
        {
            var coSoId = Guid.NewGuid();
            for (int i = 0; i < 300; i++)
            {
                _context.DiaDiems.Add(new DiaDiem
                {
                    TenDiaDiem = $"Địa điểm {i}",
                    IdCoSo = coSoId,
                    ViDo = 21.0 + i * 0.001,
                    KinhDo = 105.8 + i * 0.001,
                    BanKinh = 100 + i,
                    TrangThai = i % 2 == 0
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as System.Collections.Generic.List<DiaDiem>;
            Assert.AreEqual(300, list.Count);
        }

        //16. Test đổi trạng thái nhiều lần liên tiếp
        [Test]
        public async Task DoiTrangThai_MultipleTimes_TogglesCorrectly()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm Toggle",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            // Toggle lần 1
            await _controller.DoiTrangThai(diaDiem.IdDiaDiem);
            var dd1 = _context.DiaDiems.Find(diaDiem.IdDiaDiem);
            Assert.IsFalse(dd1.TrangThai);

            // Toggle lần 2
            await _controller.DoiTrangThai(diaDiem.IdDiaDiem);
            var dd2 = _context.DiaDiems.Find(diaDiem.IdDiaDiem);
            Assert.IsTrue(dd2.TrangThai);
        }

        //17. Test xóa địa điểm đã bị xóa (double delete)
        [Test]
        public async Task Delete_AlreadyDeleted_ReturnsNotFound()
        {
            var coSoId = Guid.NewGuid();
            var diaDiem = new DiaDiem
            {
                TenDiaDiem = "Địa điểm Xóa",
                IdCoSo = coSoId,
                ViDo = 21.0285,
                KinhDo = 105.8542,
                BanKinh = 100,
                TrangThai = true
            };
            _context.DiaDiems.Add(diaDiem);
            await _context.SaveChangesAsync();

            // Xóa lần 1
            await _controller.Delete(diaDiem.IdDiaDiem);

            // Xóa lần 2
            var result = await _controller.Delete(diaDiem.IdDiaDiem);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        
    }
}
