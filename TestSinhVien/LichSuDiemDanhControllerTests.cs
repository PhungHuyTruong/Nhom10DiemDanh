using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSinhVien
{
    [TestFixture]
    public class LichSuDiemDanhControllerTests
    {
        private ModuleDiemDanhDbContext _context;
        private LichSuDiemDanhController _controller;
        private Guid _sinhVienId;
        private Guid _hocKyId;
        private Guid _nhomXuongId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ModuleDiemDanhDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ModuleDiemDanhDbContext(options);
            _controller = new LichSuDiemDanhController(_context);

            _sinhVienId = Guid.NewGuid();
            _hocKyId = Guid.NewGuid();
            _nhomXuongId = Guid.NewGuid();

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Tạo đầy đủ các trường bắt buộc cho HocKy
            var hocKy = new HocKy
            {
                IdHocKy = _hocKyId,
                TenHocKy = "HK1 2023-2024",
                MaHocKy = "HK1-2023-2024"
            };

            var duAn = new DuAn
            {
                IdDuAn = Guid.NewGuid(),
                TenDuAn = "Dự án test",
                MoTa = "Dự án test cho unit test", // Bổ sung dòng này
                IdHocKy = _hocKyId,
                HocKy = hocKy
            };

            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                TenKeHoach = "KH Test",
                NoiDung = "Kế hoạch test cho unit test", // Bổ sung dòng này
                IdDuAn = duAn.IdDuAn,
                DuAn = duAn
            };

            var nhomXuong = new NhomXuong
            {
                IdNhomXuong = _nhomXuongId,
                TenNhomXuong = "Nhóm 1",
                MoTa = "Nhóm xưởng test cho unit test" // Bổ sung dòng này
            };

            var keHoachNhomXuong = new KeHoachNhomXuong
            {
                IdKHNX = Guid.NewGuid(),
                IdKeHoach = keHoach.IdKeHoach,
                IdNhomXuong = _nhomXuongId,
                KeHoach = keHoach,
                NhomXuong = nhomXuong
            };

            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca 1",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };

            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = keHoachNhomXuong.IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = DateTime.Now.Date,
                NoiDung = "Bài test",
                DiemDanhTre = "15",
                Buoi = "Sáng", // <-- Bổ sung
                LinkOnline = "", // <-- Bổ sung (có thể để rỗng nếu không dùng)
                ThoiGian = "07:00-11:00", // <-- Bổ sung
                CaHoc = caHoc,
                KeHoachNhomXuong = keHoachNhomXuong
            };

            var diemDanh = new DiemDanh
            {
                IdDiemDanh = Guid.NewGuid(),
                IdSinhVien = _sinhVienId,
                IdCaHoc = caHoc.IdCaHoc,
                IdNhomXuong = _nhomXuongId
            };

            var lichSuDiemDanh = new LichSuDiemDanh
            {
                IdLSDD = Guid.NewGuid(),
                IdDiemDanh = diemDanh.IdDiemDanh,
                IdNXCH = khnxCaHoc.IdNXCH,
                ThoiGianDiemDanh = DateTime.Now,
                NoiDungBuoiHoc = "Test nội dung",
                HinhThuc = "Trực tiếp",
                DiaDiem = "Phòng A1",
                GhiChu = "Ghi chú test",
                TrangThai = 3,
                TrangThaiDuyet = 1,
                DiemDanh = diemDanh,
                KHNXCaHoc = khnxCaHoc
            };

            _context.HocKys.Add(hocKy);
            _context.DuAns.Add(duAn);
            _context.KeHoachs.Add(keHoach);
            _context.NhomXuongs.Add(nhomXuong);
            _context.KeHoachNhomXuongs.Add(keHoachNhomXuong);
            _context.CaHocs.Add(caHoc);
            _context.KHNXCaHocs.Add(khnxCaHoc);
            _context.DiemDanhs.Add(diemDanh);
            _context.LichSuDiemDanhs.Add(lichSuDiemDanh);
            _context.SaveChanges();
        }

        //1
        [Test]
        public async Task GetLichSuDiemDanh_WithValidData_ReturnsOkResult()
        {
            var result = await _controller.GetLichSuDiemDanh(_sinhVienId, _hocKyId, _nhomXuongId);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Not.Empty);
        }

        //2
        [Test]
        public async Task GetLichSuDiemDanh_WithInvalidSinhVienId_ReturnsNoCheckinCheckout()
        {
            var invalidSinhVienId = Guid.NewGuid();

            var result = await _controller.GetLichSuDiemDanh(invalidSinhVienId, _hocKyId, _nhomXuongId);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Not.Empty); // Vẫn có ca học

            // Kiểm tra tất cả các ca học đều không có checkin/checkout
            foreach (var item in items)
            {
                var type = item.GetType();
                var checkIn = type.GetProperty("CheckIn")?.GetValue(item)?.ToString();
                var checkOut = type.GetProperty("CheckOut")?.GetValue(item)?.ToString();
                Assert.That(checkIn, Is.EqualTo("Chưa checkin"));
                Assert.That(checkOut, Is.EqualTo("Chưa checkout"));
            }
        }
        
        //3
        [Test]
        public async Task GetLichSuDiemDanh_WithoutFilters_ReturnsAllRecords()
        {
            var result = await _controller.GetLichSuDiemDanh(_sinhVienId, null, null);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Not.Empty);
        }

        //4
        [Test]
        public async Task DownloadTemplate_WithValidData_ReturnsFileResult()
        {
            var result = await _controller.DownloadTemplate(_sinhVienId, _hocKyId, _nhomXuongId);

            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        }


        //5
        [Test]
        public async Task DownloadTemplate_WithInvalidData_ReturnsEmptyFile()
        {
            var invalidSinhVienId = Guid.NewGuid();

            var result = await _controller.DownloadTemplate(invalidSinhVienId, _hocKyId, _nhomXuongId);

            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        }

        //6. Test lọc chỉ theo học kỳ
        [Test]
        public async Task GetLichSuDiemDanh_FilterByHocKy_ReturnsCorrectRecords()
        {
            // Arrange: tạo thêm 1 học kỳ và ca học khác
            var anotherHocKyId = Guid.NewGuid();
            var hocKy2 = new HocKy { IdHocKy = anotherHocKyId, TenHocKy = "HK2", MaHocKy = "HK2" };
            _context.HocKys.Add(hocKy2);
            _context.SaveChanges();

            // Act: chỉ truyền IdHocKy
            var result = await _controller.GetLichSuDiemDanh(_sinhVienId, anotherHocKyId, null);

            // Assert: không có ca học nào vì sinh viên không có ca học ở học kỳ này
            var okResult = result as OkObjectResult;
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Empty);
        }


        

        //7. Test ca học đã qua, chỉ check-in
        [Test]
        public async Task GetLichSuDiemDanh_CaHocDaQua_ChiCheckIn_TrangThaiDaCheckIn()
        {
            // Arrange: tạo ca học hôm qua, chỉ có điểm danh check-in
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var caHoc = new CaHoc
            {
                IdCaHoc = Guid.NewGuid(),
                TenCaHoc = "Ca hôm qua",
                ThoiGianBatDau = new TimeSpan(7, 0, 0),
                ThoiGianKetThuc = new TimeSpan(11, 0, 0)
            };
            var khnxCaHoc = new KHNXCaHoc
            {
                IdNXCH = Guid.NewGuid(),
                IdKHNX = _context.KeHoachNhomXuongs.First().IdKHNX,
                IdCaHoc = caHoc.IdCaHoc,
                NgayHoc = yesterday,
                NoiDung = "Bài test hôm qua",
                DiemDanhTre = "15",
                Buoi = "Sáng",
                LinkOnline = "",
                ThoiGian = "07:00-11:00",
                CaHoc = caHoc,
                KeHoachNhomXuong = _context.KeHoachNhomXuongs.First()
            };
            var diemDanh = new DiemDanh
            {
                IdDiemDanh = Guid.NewGuid(),
                IdSinhVien = _sinhVienId,
                IdCaHoc = caHoc.IdCaHoc,
                IdNhomXuong = _nhomXuongId
            };
            var lichSuDiemDanh = new LichSuDiemDanh
            {
                IdLSDD = Guid.NewGuid(),
                IdDiemDanh = diemDanh.IdDiemDanh,
                IdNXCH = khnxCaHoc.IdNXCH,
                ThoiGianDiemDanh = yesterday.AddHours(7),
                NoiDungBuoiHoc = "Test nội dung hôm qua",
                HinhThuc = "Trực tiếp",
                DiaDiem = "Phòng A1",
                GhiChu = "Ghi chú test",
                TrangThai = 3, // Check-in
                TrangThaiDuyet = 1,
                DiemDanh = diemDanh,
                KHNXCaHoc = khnxCaHoc
            };
            _context.CaHocs.Add(caHoc);
            _context.KHNXCaHocs.Add(khnxCaHoc);
            _context.DiemDanhs.Add(diemDanh);
            _context.LichSuDiemDanhs.Add(lichSuDiemDanh);
            _context.SaveChanges();

            // Act
            var result = await _controller.GetLichSuDiemDanh(_sinhVienId, _hocKyId, _nhomXuongId);

            // Assert: dùng reflection
            var okResult = result as OkObjectResult;
            var items = okResult.Value as IEnumerable<object>;
            object yesterdayItem = items.FirstOrDefault(x =>
                x.GetType().GetProperty("NgayHoc")?.GetValue(x)?.ToString().Contains(yesterday.Year.ToString()) == true
            );
            Assert.IsNotNull(yesterdayItem);
            var trangThai = yesterdayItem.GetType().GetProperty("TrangThai")?.GetValue(yesterdayItem)?.ToString();
            Assert.That(trangThai, Is.EqualTo("Đã check-in"));
        }


        //8. Test DownloadTemplate không có dữ liệu\
        [Test]
        public async Task DownloadTemplate_NoData_ReturnsEmptyFile()
        {
            // Arrange: xóa hết dữ liệu
            foreach (var entity in _context.LichSuDiemDanhs) _context.LichSuDiemDanhs.Remove(entity);
            foreach (var entity in _context.KHNXCaHocs) _context.KHNXCaHocs.Remove(entity);
            _context.SaveChanges();

            // Act
            var result = await _controller.DownloadTemplate(_sinhVienId, _hocKyId, _nhomXuongId);

            // Assert
            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
            Assert.That(fileResult.FileContents.Length, Is.GreaterThan(0)); // File vẫn có header
        }

        //9. Test lọc chỉ theo nhóm xưởng (Filter by NhomXuong)
        [Test]
        public async Task GetLichSuDiemDanh_FilterByNhomXuong_ReturnsCorrectRecords()
        {
            // Arrange: tạo thêm 1 nhóm xưởng khác và ca học khác
            var anotherNhomXuongId = Guid.NewGuid();
            var nhomXuong2 = new NhomXuong { IdNhomXuong = anotherNhomXuongId, TenNhomXuong = "Nhóm 2", MoTa = "Nhóm khác" };
            _context.NhomXuongs.Add(nhomXuong2);
            _context.SaveChanges();

            // Act: chỉ truyền IdNhomXuong
            var result = await _controller.GetLichSuDiemDanh(_sinhVienId, null, anotherNhomXuongId);

            // Assert: không có ca học nào vì sinh viên không thuộc nhóm này
            var okResult = result as OkObjectResult;
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Empty);
        }

        //10. Test truyền cả 3 filter nhưng không có dữ liệu phù hợp
        [Test]
        public async Task GetLichSuDiemDanh_AllFilters_NoMatch_ReturnsEmpty()
        {
            var invalidSinhVienId = Guid.NewGuid();
            var invalidHocKyId = Guid.NewGuid();
            var invalidNhomXuongId = Guid.NewGuid();

            var result = await _controller.GetLichSuDiemDanh(invalidSinhVienId, invalidHocKyId, invalidNhomXuongId);

            var okResult = result as OkObjectResult;
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Empty);
        }

        //11.  Test DownloadTemplate với chỉ filter học kỳ
        [Test]
        public async Task DownloadTemplate_FilterByHocKy_ReturnsFile()
        {
            var result = await _controller.DownloadTemplate(_sinhVienId, _hocKyId, null);

            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        }

        //12. Test DownloadTemplate với chỉ filter nhóm xưởng
        [Test]
        public async Task DownloadTemplate_FilterByNhomXuong_ReturnsFile()
        {
            var result = await _controller.DownloadTemplate(_sinhVienId, null, _nhomXuongId);

            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        }

        

        //13. Test DownloadTemplate với sinh viên không tồn tại
        [Test]
        public async Task DownloadTemplate_InvalidSinhVienId_ReturnsFile()
        {
            var invalidSinhVienId = Guid.NewGuid();
            var result = await _controller.DownloadTemplate(invalidSinhVienId, null, null);

            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        }


        //14. Test GetLichSuDiemDanh với tất cả tham số là null (trả về tất cả ca học của sinh viên)
        [Test]
        public async Task GetLichSuDiemDanh_AllNullParams_ReturnsAllSinhVienRecords()
        {
            // Act
            var result = await _controller.GetLichSuDiemDanh(_sinhVienId, null, null);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);
            var items = okResult.Value as IEnumerable<object>;
            Assert.That(items, Is.Not.Empty);
        }

        //15. Test DownloadTemplate khi không truyền filter nào (trả về file tổng hợp)
        [Test]
        public async Task DownloadTemplate_NoFilters_ReturnsFile()
        {
            // Act
            var result = await _controller.DownloadTemplate(_sinhVienId, null, null);

            // Assert
            Assert.That(result, Is.TypeOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("LichSuDiemDanh.xlsx"));
        }







        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

       
    }
}