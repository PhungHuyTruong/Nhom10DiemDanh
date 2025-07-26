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

namespace TestUnit
{
    public class CoSoControllerTests
    {
        private CoSoController _controller;
        private DbContextOptions<ModuleDiemDanhDbContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ModuleDiemDanhDbContext(_options);
            _controller = new CoSoController(context);
        }

        [Test]
        public async Task TC001_ThemCoSo_HopLe()
        {
            // Arrange
            var context = new ModuleDiemDanhDbContext(_options);
            var controller = new CoSoController(context);

            var model = new CoSoViewModel
            {
                TenCoSo = "Trịnh văn bô",
                MaCoSo = "tvb1",
                TrangThai = "Hoạt động",
                DiaChi = "123 Đường A",
                Email = "trinhvb@example.com",
                SDT = "0987654321",
                IdDiaDiem = Guid.NewGuid()
            };

            // Act
            var result = await controller.CreateCoSo(model);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);

            var created = (CreatedAtActionResult)result.Result;
            var createdModel = created.Value as CoSoViewModel;

            Assert.NotNull(createdModel);
            Assert.AreEqual("Trịnh văn bô", createdModel.TenCoSo);
            Assert.AreEqual("tvb1", createdModel.MaCoSo);
            Assert.AreEqual("Hoạt động", createdModel.TrangThai);
        }


        [Test]
        public async Task TC002_ThemCoSo_ThieuTen()
        {
            var model = new CoSoViewModel { TenCoSo = "", MaCoSo = "tvb1", TrangThai = "Hoạt động" };
            _controller.ModelState.AddModelError("TenCoSo", "Vui lòng nhập tên Cơ Sở");
            var result = await _controller.CreateCoSo(model);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public async Task TC003_ThemCoSo_ThieuMa()
        {
            var model = new CoSoViewModel { TenCoSo = "Trịnh văn bô", MaCoSo = "", TrangThai = "Hoạt động" };
            _controller.ModelState.AddModelError("MaCoSo", "Vui lòng nhập mã Cơ Sở");
            var result = await _controller.CreateCoSo(model);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public async Task TC004_GetCoSo_HopLe()
        {
            // Arrange
            var context = new ModuleDiemDanhDbContext(_options);
            var coSo = new CoSo
            {
                IdCoSo = Guid.NewGuid(),
                TenCoSo = "Cơ sở A",
                MaCoSo = "CSA001",
                DiaChi = "123 Đường A",
                Email = "csa@example.com",
                SDT = "0901234567",
                TrangThai = true,
                IdDiaDiem = Guid.NewGuid()
            };
            context.CoSos.Add(coSo);
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);

            // Act
            var result = await controller.GetCoSo(coSo.IdCoSo);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var returned = (result.Result as OkObjectResult).Value as CoSoViewModel;
            Assert.AreEqual("Cơ sở A", returned.TenCoSo);
        }


        [Test]
        public async Task TC005_GetCoSo_KhongTonTai()
        {
            var result = await _controller.GetCoSo(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task TC006_GetCoSos_TrangThaiHoatDong()
        {
            var context = new ModuleDiemDanhDbContext(_options);

            context.CoSos.AddRange(
                new CoSo
                {
                    TenCoSo = "CS A",
                    MaCoSo = "CSA001",
                    DiaChi = "123 Đường A",
                    Email = "csa@example.com",
                    SDT = "0901111222",
                    TrangThai = true,
                    IdDiaDiem = Guid.NewGuid()
                },
                new CoSo
                {
                    TenCoSo = "CS B",
                    MaCoSo = "CSB002",
                    DiaChi = "456 Đường B",
                    Email = "csb@example.com",
                    SDT = "0903333444",
                    TrangThai = false,
                    IdDiaDiem = Guid.NewGuid()
                }
            );
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);

            // Act
            var result = await controller.GetCoSos(null, "Hoạt động");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            var list = okResult.Value as List<CoSoViewModel>;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Hoạt động", list[0].TrangThai);

        }


        [Test]
        public async Task TC007_UpdateCoSo_HopLe()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            var id = Guid.NewGuid();
            context.CoSos.Add(new CoSo
            {
                IdCoSo = id,
                TenCoSo = "Old Name",
                MaCoSo = "OLD123",
                DiaChi = "123 Đường cũ",
                Email = "old@example.com",
                SDT = "0912345678",
                TrangThai = true,
                IdDiaDiem = Guid.NewGuid()
            });
            await context.SaveChangesAsync();
            var controller = new CoSoController(context);

            var model = new CoSoViewModel
            {
                IdCoSo = id,
                TenCoSo = "Updated Name",
                MaCoSo = "NEW123",
                DiaChi = "456 Đường mới",
                Email = "new@example.com",
                SDT = "0987654321",
                TrangThai = "Hoạt động",
                IdDiaDiem = Guid.NewGuid()
            };

            var result = await controller.UpdateCoSo(id, model);
            Assert.IsInstanceOf<NoContentResult>(result);
        }


        [Test]
        public async Task TC008_UpdateCoSo_IdMismatch()
        {
            var model = new CoSoViewModel { IdCoSo = Guid.NewGuid() };
            var result = await _controller.UpdateCoSo(Guid.NewGuid(), model);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task TC009_DeleteCoSo_HopLe()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            var id = Guid.NewGuid();
            context.CoSos.Add(new CoSo
            {
                IdCoSo = id,
                TenCoSo = "Cơ sở xóa",
                MaCoSo = "DEL123",
                DiaChi = "99 Đường xóa",
                Email = "delete@example.com",
                SDT = "0909090909",
                TrangThai = true,
                IdDiaDiem = Guid.NewGuid()
            });
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);
            var result = await controller.DeleteCoSo(id);

            Assert.IsInstanceOf<NoContentResult>(result);
        }


        [Test]
        public async Task TC010_ToggleStatus_ValidId_ChangesStatus()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            var id = Guid.NewGuid();
            context.CoSos.Add(new CoSo
            {
                IdCoSo = id,
                TenCoSo = "Cơ sở Toggle",
                MaCoSo = "TGL123",
                DiaChi = "Địa chỉ ABC",
                Email = "toggle@example.com",
                SDT = "0999999999",
                TrangThai = true,
                IdDiaDiem = Guid.NewGuid()
            });
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);
            var result = await controller.ToggleStatus(id);

            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = await context.CoSos.FindAsync(id);
            Assert.IsFalse(updated.TrangThai); // Đã bị đổi từ true sang false
        }

        [Test]
        public async Task TC011_GetCoSos_FilterByTenCoSo()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            context.CoSos.AddRange(
                new CoSo
                {
                    TenCoSo = "Alpha School",
                    MaCoSo = "AS001",
                    DiaChi = "123 Street",
                    Email = "alpha@example.com",
                    SDT = "0901111222",
                    TrangThai = true,
                    IdDiaDiem = Guid.NewGuid()
                },
                new CoSo
                {
                    TenCoSo = "Beta Campus",
                    MaCoSo = "BC002",
                    DiaChi = "456 Street",
                    Email = "beta@example.com",
                    SDT = "0903333444",
                    TrangThai = true,
                    IdDiaDiem = Guid.NewGuid()
                }
            );
            await context.SaveChangesAsync();
            var controller = new CoSoController(context);

            var result = await controller.GetCoSos("Alpha", null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var list = (result.Result as OkObjectResult).Value as List<CoSoViewModel>;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Alpha School", list[0].TenCoSo);
        }

        [Test]
        public async Task TC012_ToggleStatus_FalseToTrue()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            var id = Guid.NewGuid();
            context.CoSos.Add(new CoSo
            {
                IdCoSo = id,
                TenCoSo = "Inactive Branch",
                MaCoSo = "IB001",
                DiaChi = "789 Street",
                Email = "inactive@example.com",
                SDT = "0902222333",
                TrangThai = false,
                IdDiaDiem = Guid.NewGuid()
            });
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);
            var result = await controller.ToggleStatus(id);

            Assert.IsInstanceOf<NoContentResult>(result);

            var updated = await context.CoSos.FindAsync(id);
            Assert.IsTrue(updated.TrangThai);
        }


        [Test]
        public async Task TC013_DeleteAndCheck_GetShouldReturnNotFound()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            var id = Guid.NewGuid();
            context.CoSos.Add(new CoSo
            {
                IdCoSo = id,
                TenCoSo = "To Be Deleted",
                MaCoSo = "DEL999",
                DiaChi = "Del Street",
                Email = "delete@example.com",
                SDT = "0904444555",
                TrangThai = true,
                IdDiaDiem = Guid.NewGuid()
            });
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);
            var deleteResult = await controller.DeleteCoSo(id);
            Assert.IsInstanceOf<NoContentResult>(deleteResult);

            var getResult = await controller.GetCoSo(id);
            Assert.IsInstanceOf<NotFoundResult>(getResult.Result);
        }

        [Test]
        public async Task TC014_UpdateCoSo_ThayDoiTenVaTrangThai_ThanhCong()
        {
            // Arrange
            var context = new ModuleDiemDanhDbContext(_options);
            var id = Guid.NewGuid();

            context.CoSos.Add(new CoSo
            {
                IdCoSo = id,
                TenCoSo = "Tên cũ",
                MaCoSo = "CS014",
                DiaChi = "Địa chỉ cũ",
                Email = "cu@example.com",
                SDT = "0901111222",
                TrangThai = true,
                IdDiaDiem = Guid.NewGuid()
            });
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);
            var model = new CoSoViewModel
            {
                IdCoSo = id,
                TenCoSo = "Tên mới cập nhật",
                MaCoSo = "CS014",
                DiaChi = "Địa chỉ mới",
                Email = "moi@example.com",
                SDT = "0903333444",
                TrangThai = "Tắt", // sẽ chuyển sang false
                IdDiaDiem = Guid.NewGuid()
            };

            // Act
            var result = await controller.UpdateCoSo(id, model);

            // Assert kết quả API
            Assert.IsInstanceOf<NoContentResult>(result);

            // Assert dữ liệu trong DB
            var updated = await context.CoSos.FindAsync(id);
            Assert.AreEqual("Tên mới cập nhật", updated.TenCoSo);
            Assert.AreEqual("Địa chỉ mới", updated.DiaChi);
            Assert.AreEqual("moi@example.com", updated.Email);
            Assert.IsFalse(updated.TrangThai); // vì model.TrangThai == "Tắt"
        }







        [Test]
        public async Task TC015_GetAll_WhenTrangThaiIsTatCa()
        {
            var context = new ModuleDiemDanhDbContext(_options);
            context.CoSos.AddRange(
                new CoSo
                {
                    TenCoSo = "A",
                    MaCoSo = "A1",
                    DiaChi = "Đường A",
                    Email = "a@example.com",
                    SDT = "0900000001",
                    TrangThai = true,
                    IdDiaDiem = Guid.NewGuid()
                },
                new CoSo
                {
                    TenCoSo = "B",
                    MaCoSo = "B2",
                    DiaChi = "Đường B",
                    Email = "b@example.com",
                    SDT = "0900000002",
                    TrangThai = false,
                    IdDiaDiem = Guid.NewGuid()
                }
            );
            await context.SaveChangesAsync();

            var controller = new CoSoController(context);
            var result = await controller.GetCoSos(null, "Tất cả trạng thái");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var list = (result.Result as OkObjectResult).Value as List<CoSoViewModel>;
            Assert.AreEqual(2, list.Count);
        }


    }
}
