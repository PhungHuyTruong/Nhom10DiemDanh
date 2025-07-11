using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleDiemDanhTest
{
    public class BanDaoTaoControllerTest
    {
        private ModuleDiemDanhDbContext _context;
        private BanDaoTaoController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test 1 DB riêng
                .Options;

            _context = new ModuleDiemDanhDbContext(options);

            // Seed dữ liệu giả
            var banDaoTao = new BanDaoTao
            {
                IdBanDaoTao = Guid.NewGuid(),
                MaBanDaoTao = "BDT01",
                TenBanDaoTao = "Ban 1",
                Email = "bdt1@example.com",
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            _context.BanDaoTaos.Add(banDaoTao);
            _context.SaveChanges();

            _controller = new BanDaoTaoController(_context);
        }

        [Test]
        public async Task ChangeStatus_ValidId_TogglesTrangThai()
        {
            // Arrange
            var banDaoTao = await _context.BanDaoTaos.FirstAsync();
            var originalStatus = banDaoTao.TrangThai;

            // Act
            var result = await _controller.ChangeStatus(banDaoTao.IdBanDaoTao);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var updated = (result as OkObjectResult)?.Value as BanDaoTao;
            Assert.IsNotNull(updated);
            Assert.AreNotEqual(originalStatus, updated?.TrangThai);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
