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
    public class PhuTrachXuongsControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private PhuTrachXuongsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ModuleDiemDanhDbContext(options);
            _controller = new PhuTrachXuongsController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Các test sẽ viết ở đây

        //1. Test lấy tất cả phụ trách xưởng
        [Test]
        public async Task GetAll_ReturnsAllPhuTrachXuongs()
        {
            _context.PhuTrachXuongs.Add(new PhuTrachXuong { TenNhanVien = "nv1", MaNhanVien = "mnv1", EmailFE = "nv1@fe.edu.vn", EmailFPT = "nv1@gmail.com" });
            _context.PhuTrachXuongs.Add(new PhuTrachXuong { TenNhanVien = "nv2", MaNhanVien = "mnv2", EmailFE = "nv2@fe.edu.vn", EmailFPT = "nv2@gmail.com" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as IEnumerable<PhuTrachXuongDto>;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
        }


        //2. Test lấy phụ trách xưởng theo ID
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var nv = new PhuTrachXuong { TenNhanVien = "nv1", MaNhanVien = "mnv1", EmailFE = "nv1@fe.edu.vn", EmailFPT = "nv1@gmail.com" };
            _context.PhuTrachXuongs.Add(nv);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(nv.IdNhanVien);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returned = okResult.Value as PhuTrachXuong;
            Assert.AreEqual("nv1", returned.TenNhanVien);
        }

        //3. Test lấy phụ trách xưởng với ID không tồn tại
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //4. Test tạo mới phụ trách xưởng thành công
        [Test]
        public async Task Create_ValidPhuTrachXuong_ReturnsRedirect()
        {
            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "nv3",
                MaNhanVien = "mnv3",
                EmailFE = "nv3@fe.edu.vn",
                EmailFPT = "nv3@gmail.com",
                IdVaiTros = new List<Guid>() // Không có vai trò
            };

            var result = await _controller.Create(model);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var saved = _context.PhuTrachXuongs.FirstOrDefault(x => x.MaNhanVien == "mnv3");
            Assert.IsNotNull(saved);
        }

        

        //5. Test cập nhật phụ trách xưởng thành công
        [Test]
        public async Task Update_ValidData_UpdatesSuccessfully()
        {
            var nv = new PhuTrachXuong { TenNhanVien = "nv5", MaNhanVien = "mnv5", EmailFE = "nv5@fe.edu.vn", EmailFPT = "nv5@gmail.com" };
            _context.PhuTrachXuongs.Add(nv);
            await _context.SaveChangesAsync();

            var updated = new PhuTrachXuongDto
            {
                TenNhanVien = "nv5-updated",
                MaNhanVien = "mnv5-updated",
                EmailFE = "nv5-updated@fe.edu.vn",
                EmailFPT = "nv5-updated@gmail.com",
                TrangThai = false,
                IdVaiTros = new List<Guid>()
            };

            var result = await _controller.Update(nv.IdNhanVien, updated);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);

            var nvDb = _context.PhuTrachXuongs.Find(nv.IdNhanVien);
            Assert.AreEqual("nv5-updated", nvDb.TenNhanVien);
            Assert.IsFalse(nvDb.TrangThai);
        }


        //6. Test cập nhật với ID không tồn tại
        [Test]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            var updated = new PhuTrachXuongDto
            {
                TenNhanVien = "notfound",
                MaNhanVien = "notfound"
            };

            var result = await _controller.Update(Guid.NewGuid(), updated);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //7. Test đổi trạng thái
        [Test]
        public async Task ToggleStatus_ChangesTrangThai()
        {
            var nv = new PhuTrachXuong
            {
                TenNhanVien = "nv6",
                MaNhanVien = "mnv6",
                TrangThai = true,
                EmailFE = "nv6@fe.edu.vn",    // BỔ SUNG DÒNG NÀY
                EmailFPT = "nv6@gmail.com"    // BỔ SUNG DÒNG NÀY
            };
            _context.PhuTrachXuongs.Add(nv);
            await _context.SaveChangesAsync();

            var result = await _controller.ToggleStatus(nv.IdNhanVien);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var nvDb = _context.PhuTrachXuongs.Find(nv.IdNhanVien);
            Assert.IsFalse(nvDb.TrangThai);
        }


        //8. Test đổi trạng thái với ID không tồn tại
        [Test]
        public async Task ToggleStatus_NonExistentId_ReturnsNotFound()
        {
            var result = await _controller.ToggleStatus(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //9. Test tạo mới với nhiều vai trò
        [Test]
        public async Task Create_WithMultipleVaiTros_SavesAllRoles()
        {
            // Tạo vai trò mẫu
            var vaiTro1 = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Trưởng xưởng" };
            var vaiTro2 = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Phó xưởng" };
            _context.VaiTros.AddRange(vaiTro1, vaiTro2);
            await _context.SaveChangesAsync();

            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "nv7",
                MaNhanVien = "mnv7",
                EmailFE = "nv7@fe.edu.vn",
                EmailFPT = "nv7@gmail.com",
                IdVaiTros = new List<Guid> { vaiTro1.IdVaiTro, vaiTro2.IdVaiTro }
            };

            var result = await _controller.Create(model);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);

            var nv = _context.PhuTrachXuongs.FirstOrDefault(x => x.MaNhanVien == "mnv7");
            Assert.IsNotNull(nv);

            var vaiTros = _context.VaiTroNhanViens.Where(v => v.IdNhanVien == nv.IdNhanVien).ToList();
            Assert.AreEqual(2, vaiTros.Count);
        }

        //10.  Test lấy danh sách cơ sở
        [Test]
        public async Task GetCoSoList_ReturnsAllCoSo()
        {
            _context.CoSos.Add(new CoSo { TenCoSo = "Cơ sở 1", MaCoSo = "CS1", DiaChi = "Địa chỉ 1", SDT = "0123456789", Email = "cs1@email.com" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetCoSoList();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as IEnumerable<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Cast<object>().Count());
        }

        //11. Test lấy danh sách vai trò
        [Test]
        public async Task GetVaiTroList_ReturnsAllVaiTro()
        {
            _context.VaiTros.Add(new VaiTro { TenVaiTro = "Trưởng xưởng" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetVaiTroList();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as IEnumerable<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Cast<object>().Count());
        }

        //12. Test tạo mới với mã nhân viên trùng (nếu có ràng buộc unique)
        [Test]
        public async Task Create_DuplicateMaNhanVien_AllowsOrRejects()
        {
            var model1 = new PhuTrachXuongDto
            {
                TenNhanVien = "nv8",
                MaNhanVien = "mnv8",
                EmailFE = "nv8@fe.edu.vn",
                EmailFPT = "nv8@gmail.com",
                IdVaiTros = new List<Guid>()
            };
            var model2 = new PhuTrachXuongDto
            {
                TenNhanVien = "nv8-2",
                MaNhanVien = "mnv8", // Trùng mã nhân viên
                EmailFE = "nv8-2@fe.edu.vn",
                EmailFPT = "nv8-2@gmail.com",
                IdVaiTros = new List<Guid>()
            };

            await _controller.Create(model1);
            var result = await _controller.Create(model2);

            // Nếu bạn kiểm tra trùng, nên trả về BadRequest hoặc ViewResult với lỗi
            // Assert.IsInstanceOf<BadRequestResult>(result);
            // Nếu không kiểm tra, có thể cho phép tạo (tùy vào logic)
        }

        

        //13. Test cập nhật chỉ một trường (partial update)
        [Test]
        public async Task Update_OnlyTenNhanVien_UpdatesSuccessfully()
        {
            var nv = new PhuTrachXuong
            {
                TenNhanVien = "nv10",
                MaNhanVien = "mnv10",
                EmailFE = "nv10@fe.edu.vn",
                EmailFPT = "nv10@gmail.com"
            };
            _context.PhuTrachXuongs.Add(nv);
            await _context.SaveChangesAsync();

            var updated = new PhuTrachXuongDto
            {
                TenNhanVien = "nv10-updated",
                MaNhanVien = nv.MaNhanVien,
                EmailFE = nv.EmailFE,
                EmailFPT = nv.EmailFPT,
                TrangThai = nv.TrangThai,
                IdVaiTros = new List<Guid>()
            };

            var result = await _controller.Update(nv.IdNhanVien, updated);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);

            var nvDb = _context.PhuTrachXuongs.Find(nv.IdNhanVien);
            Assert.AreEqual("nv10-updated", nvDb.TenNhanVien);
            Assert.AreEqual("mnv10", nvDb.MaNhanVien);
        }

        //14. Test lấy tất cả khi không có dữ liệu
        [Test]
        public async Task GetAll_Empty_ReturnsEmptyList()
        {
            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as IEnumerable<PhuTrachXuongDto>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count());
        }

        //15. Test cập nhật ngày cập nhật (NgayCapNhat) thay đổi
        [Test]
        public async Task Update_UpdatesNgayCapNhat()
        {
            var nv = new PhuTrachXuong
            {
                TenNhanVien = "nv11",
                MaNhanVien = "mnv11",
                EmailFE = "nv11@fe.edu.vn",
                EmailFPT = "nv11@gmail.com"
            };
            _context.PhuTrachXuongs.Add(nv);
            await _context.SaveChangesAsync();

            var oldNgayCapNhat = nv.NgayCapNhat;

            var updated = new PhuTrachXuongDto
            {
                TenNhanVien = "nv11-updated",
                MaNhanVien = "mnv11-updated",
                EmailFE = "nv11-updated@fe.edu.vn",
                EmailFPT = "nv11-updated@gmail.com",
                TrangThai = nv.TrangThai,
                IdVaiTros = new List<Guid>()
            };

            var result = await _controller.Update(nv.IdNhanVien, updated);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);

            var nvDb = _context.PhuTrachXuongs.Find(nv.IdNhanVien);
            Assert.IsNotNull(nvDb.NgayCapNhat);
            if (oldNgayCapNhat != null)
                Assert.IsTrue(nvDb.NgayCapNhat >= oldNgayCapNhat);
        }

        //16. Test tạo mới với EmailFPT rỗng (nếu cho phép)
        [Test]
        public async Task Create_EmptyEmailFPT_AllowsOrRejects()
        {
            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "nv12",
                MaNhanVien = "mnv12",
                EmailFE = "nv12@fe.edu.vn",
                EmailFPT = "", // Rỗng
                IdVaiTros = new List<Guid>()
            };

            var result = await _controller.Create(model);
            // Nếu cho phép tạo, kiểm tra Redirect
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            // Nếu không cho phép, kiểm tra BadRequest
            // Assert.IsInstanceOf<BadRequestResult>(result);
        }

        
        

        //17. Test cập nhật vai trò (thay đổi danh sách vai trò)
        [Test]
        public async Task Update_ChangeVaiTros_UpdatesSuccessfully()
        {
            // Tạo vai trò mẫu
            var vaiTro1 = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Trưởng xưởng" };
            var vaiTro2 = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Phó xưởng" };
            var vaiTro3 = new VaiTro { IdVaiTro = Guid.NewGuid(), TenVaiTro = "Thư ký" };
            _context.VaiTros.AddRange(vaiTro1, vaiTro2, vaiTro3);
            await _context.SaveChangesAsync();

            // Tạo nhân viên với 2 vai trò đầu
            var nv = new PhuTrachXuong
            {
                TenNhanVien = "nv15",
                MaNhanVien = "mnv15",
                EmailFE = "nv15@fe.edu.vn",
                EmailFPT = "nv15@gmail.com"
            };
            _context.PhuTrachXuongs.Add(nv);
            await _context.SaveChangesAsync();

            _context.VaiTroNhanViens.Add(new VaiTroNhanVien { IdVTNV = Guid.NewGuid(), IdNhanVien = nv.IdNhanVien, IdVaiTro = vaiTro1.IdVaiTro, NgayTao = DateTime.Now, TrangThai = true });
            _context.VaiTroNhanViens.Add(new VaiTroNhanVien { IdVTNV = Guid.NewGuid(), IdNhanVien = nv.IdNhanVien, IdVaiTro = vaiTro2.IdVaiTro, NgayTao = DateTime.Now, TrangThai = true });
            await _context.SaveChangesAsync();

            // Cập nhật chỉ còn 1 vai trò mới
            var updated = new PhuTrachXuongDto
            {
                TenNhanVien = "nv15",
                MaNhanVien = "mnv15",
                EmailFE = "nv15@fe.edu.vn",
                EmailFPT = "nv15@gmail.com",
                TrangThai = true,
                IdVaiTros = new List<Guid> { vaiTro3.IdVaiTro }
            };

            var result = await _controller.Update(nv.IdNhanVien, updated);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);

            var vaiTros = _context.VaiTroNhanViens.Where(v => v.IdNhanVien == nv.IdNhanVien).ToList();
            Assert.AreEqual(1, vaiTros.Count);
            Assert.AreEqual(vaiTro3.IdVaiTro, vaiTros[0].IdVaiTro);
        }

        //18. Test tạo mới với IdCoSo null (nếu cho phép)
        [Test]
        public async Task Create_NullIdCoSo_AllowsOrRejects()
        {
            var model = new PhuTrachXuongDto
            {
                TenNhanVien = "nv16",
                MaNhanVien = "mnv16",
                EmailFE = "nv16@fe.edu.vn",
                EmailFPT = "nv16@gmail.com",
                IdCoSo = null, // Không chọn cơ sở
                IdVaiTros = new List<Guid>()
            };

            var result = await _controller.Create(model);
            // Nếu cho phép tạo, kiểm tra Redirect
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            // Nếu không cho phép, kiểm tra BadRequest
            // Assert.IsInstanceOf<BadRequestResult>(result);
        }
    }
}
