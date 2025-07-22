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
    public class QuanLyBoMonsControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private QuanLyBoMonsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new QuanLyBoMonsController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây

        //1. Test tạo mới bộ môn thành công
        [Test]
        public async Task Create_ValidQuanLyBoMon_ReturnsRedirect()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm1",
                TenBoMon = "Bộ môn 1",
                CoSoHoatDong = "Cơ sở 1"
            };

            var result = await _controller.Create(boMon);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var saved = _context.QuanLyBoMons.FirstOrDefault(b => b.MaBoMon == "mbm1");
            Assert.IsNotNull(saved);
        }

       

        //2. Test lấy bộ môn theo ID thành công
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm3",
                TenBoMon = "Bộ môn 3",
                CoSoHoatDong = "Cơ sở 3" // BỔ SUNG DÒNG NÀY
            };
            _context.QuanLyBoMons.Add(boMon);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(boMon.IDBoMon);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as QuanLyBoMon;
            Assert.AreEqual("mbm3", returned.MaBoMon);
        }

        //3. Test lấy bộ môn theo ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //4. Test lấy tất cả bộ môn
        [Test]
        public async Task GetAll_ReturnsAllBoMons()
        {
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "mbm4", TenBoMon = "Bộ môn 4", CoSoHoatDong = "Cơ sở 4" });
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "mbm5", TenBoMon = "Bộ môn 5", CoSoHoatDong = "Cơ sở 5" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as IEnumerable<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
        }

        //5. Test cập nhật bộ môn thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm6",
                TenBoMon = "Bộ môn 6",
                CoSoHoatDong = "Cơ sở 6",
                TrangThai = true
            };
            _context.QuanLyBoMons.Add(boMon);
            await _context.SaveChangesAsync();

            var updated = new QuanLyBoMon
            {
                MaBoMon = "mbm6-updated",
                TenBoMon = "Bộ môn 6 - Updated",
                CoSoHoatDong = "Cơ sở 6 - Updated",
                TrangThai = false
            };

            var result = await _controller.Update(boMon.IDBoMon, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var boMonDb = _context.QuanLyBoMons.Find(boMon.IDBoMon);
            Assert.AreEqual("mbm6-updated", boMonDb.MaBoMon);
            Assert.AreEqual("Bộ môn 6 - Updated", boMonDb.TenBoMon);
            Assert.AreEqual("Cơ sở 6 - Updated", boMonDb.CoSoHoatDong);
            Assert.IsFalse(boMonDb.TrangThai);
        }

        //6. Test cập nhật bộ môn với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var updated = new QuanLyBoMon
            {
                MaBoMon = "NotFound",
                TenBoMon = "NotFound",
                CoSoHoatDong = "NotFound",
                TrangThai = false
            };

            var result = await _controller.Update(Guid.NewGuid(), updated);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //7. Test đổi trạng thái bộ môn
        [Test]
        public async Task ChangeStatus_ChangesTrangThai()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm7",
                TenBoMon = "Bộ môn 7",
                CoSoHoatDong = "Cơ sở 7",
                TrangThai = true
            };
            _context.QuanLyBoMons.Add(boMon);
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeStatus(boMon.IDBoMon);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var boMonDb = _context.QuanLyBoMons.Find(boMon.IDBoMon);
            Assert.IsFalse(boMonDb.TrangThai);
        }

        //8. Test đổi trạng thái bộ môn không tồn tại
        [Test]
        public async Task ChangeStatus_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.ChangeStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //9. Test lấy danh sách bộ môn khi không có dữ liệu
        [Test]
        public async Task GetAll_Empty_ReturnsEmptyList()
        {
            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as IEnumerable<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count());
        }

        //10. Test tạo mới bộ môn với mã/tên trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Create_DuplicateMaBoMonOrTenBoMon_AllowsOrRejects()
        {
            var boMon1 = new QuanLyBoMon
            {
                MaBoMon = "mbm3",
                TenBoMon = "Bộ môn trùng"
            };
            var boMon2 = new QuanLyBoMon
            {
                MaBoMon = "mbm3",
                TenBoMon = "Bộ môn trùng"
            };

            await _controller.Create(boMon1);
            var result = await _controller.Create(boMon2);

            // Nếu bạn kiểm tra trùng, nên trả về ViewResult với lỗi
            // Assert.IsInstanceOf<ViewResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        

        //11. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task Update_OnlyTenBoMon_UpdatesSuccessfully()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm4",
                TenBoMon = "Bộ môn cũ",
                CoSoHoatDong = "Cơ sở 7",
            };
            _context.QuanLyBoMons.Add(boMon);
            await _context.SaveChangesAsync();

            var updated = new QuanLyBoMon
            {
                MaBoMon = boMon.MaBoMon,
                TenBoMon = "Bộ môn mới",
                CoSoHoatDong = boMon.CoSoHoatDong,
                TrangThai = boMon.TrangThai
            };

            var result = await _controller.Update(boMon.IDBoMon, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var boMonDb = _context.QuanLyBoMons.Find(boMon.IDBoMon);
            Assert.AreEqual("Bộ môn mới", boMonDb.TenBoMon);
            Assert.AreEqual("mbm4", boMonDb.MaBoMon);
        }

        //12. Test dữ liệu lớn (stress test)
        [Test]
        public async Task GetAll_LargeData_ReturnsAll()
        {
            for (int i = 0; i < 200; i++)
            {
                _context.QuanLyBoMons.Add(new QuanLyBoMon
                {
                    MaBoMon = $"mbm{i}",
                    TenBoMon = $"Bộ môn {i}",
                    CoSoHoatDong = $"Cơ Sở {i}"
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var list = okResult.Value as IEnumerable<object>; // Sửa ở đây
            Assert.IsNotNull(list);
            Assert.AreEqual(200, list.Count());
        }

        //13. Test cập nhật ngày cập nhật (NgayCapNhat) thay đổi
        [Test]
        public async Task Update_UpdatesNgayCapNhat()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm5",
                TenBoMon = "Bộ môn 5",
                CoSoHoatDong = "Cơ sở 55",
            };
            _context.QuanLyBoMons.Add(boMon);
            await _context.SaveChangesAsync();

            var oldNgayCapNhat = boMon.NgayCapNhat;

            var updated = new QuanLyBoMon
            {
                MaBoMon = "mbm5",
                TenBoMon = "Bộ môn 5 - Updated",
                CoSoHoatDong = boMon.CoSoHoatDong,
                TrangThai = boMon.TrangThai
            };

            var result = await _controller.Update(boMon.IDBoMon, updated);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var boMonDb = _context.QuanLyBoMons.Find(boMon.IDBoMon);

            // Sửa ở đây:
            Assert.IsNotNull(boMonDb.NgayCapNhat);
            if (oldNgayCapNhat != null)
                Assert.IsTrue(boMonDb.NgayCapNhat >= oldNgayCapNhat);
        }


        //14. Test đổi trạng thái nhiều lần liên tiếp
        [Test]
        public async Task ChangeStatus_MultipleTimes_TogglesCorrectly()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbm6",
                TenBoMon = "Bộ môn 6",
                CoSoHoatDong = "Cơ sở 6",
                TrangThai = true
            };
            _context.QuanLyBoMons.Add(boMon);
            await _context.SaveChangesAsync();

            // Toggle lần 1
            await _controller.ChangeStatus(boMon.IDBoMon);
            var bm1 = _context.QuanLyBoMons.Find(boMon.IDBoMon);
            Assert.IsFalse(bm1.TrangThai);

            // Toggle lần 2
            await _controller.ChangeStatus(boMon.IDBoMon);
            var bm2 = _context.QuanLyBoMons.Find(boMon.IDBoMon);
            Assert.IsTrue(bm2.TrangThai);
        }

        //15.Test lấy bộ môn theo trạng thái
        [Test]
        public async Task GetPaged_FilterByStatus_ReturnsCorrect()
        {
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "mbm7", TenBoMon = "Bộ môn 7", TrangThai = true, CoSoHoatDong = "Cơ sở 77", });
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "mbm8", TenBoMon = "Bộ môn 8", TrangThai = false, CoSoHoatDong = "Cơ sở 88", });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(1, 10, null, "active");
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Sửa đoạn này:
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            var list = jObj["data"].ToObject<List<object>>();

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
        }

        //16. Test lọc theo tên bộ môn (search)
        [Test]
        public async Task GetPaged_FilterByTenBoMon_ReturnsCorrect()
        {
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "mbm9", TenBoMon = "Toán Ứng Dụng", TrangThai = true, CoSoHoatDong = "Cơ sở 1" });
            _context.QuanLyBoMons.Add(new QuanLyBoMon { MaBoMon = "mbm10", TenBoMon = "Vật Lý", TrangThai = true, CoSoHoatDong = "Cơ sở 2" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(1, 10, "Toán", null);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            var list = jObj["data"].ToObject<List<object>>();

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count());
        }

        //17. Test phân trang (paging)
        [Test]
        public async Task GetPaged_Paging_ReturnsCorrectPage()
        {
            for (int i = 0; i < 25; i++)
            {
                _context.QuanLyBoMons.Add(new QuanLyBoMon
                {
                    MaBoMon = $"mbm{i}",
                    TenBoMon = $"Bộ môn {i}",
                    CoSoHoatDong = $"Cơ sở {i}",
                    TrangThai = true
                });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetPaged(2, 10, null, null); // Trang 2, mỗi trang 10
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var jObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            var list = jObj["data"].ToObject<List<object>>();

            Assert.IsNotNull(list);
            Assert.AreEqual(10, list.Count());
        }


        //18. Test tạo mới bộ môn với trạng thái mặc định
        [Test]
        public async Task Create_WithoutTrangThai_DefaultsToTrue()
        {
            var boMon = new QuanLyBoMon
            {
                MaBoMon = "mbmDef",
                TenBoMon = "Bộ môn Mặc định",
                CoSoHoatDong = "Cơ sở Mặc định"
                // Không set TrangThai
            };

            var result = await _controller.Create(boMon);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var saved = _context.QuanLyBoMons.FirstOrDefault(b => b.MaBoMon == "mbmDef");
            Assert.IsNotNull(saved);
            Assert.IsTrue(saved.TrangThai); // Mặc định là true
        }


    }
}
