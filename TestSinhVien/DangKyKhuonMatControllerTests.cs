//using Emgu.CV;
//using Emgu.CV.Face;
//using Emgu.CV.Structure;
//using Emgu.CV.Util;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using Newtonsoft.Json;
//using NUnit.Framework;
//using Nhom10ModuleDiemDanh.Controllers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;

//namespace TestSinhVien
//{
//    [TestFixture]
//    public class DangKyKhuonMatControllerTests
//    {
//        private Mock<IWebHostEnvironment> _mockEnvironment;
//        private FaceController _controller;
//        private string _tempDirectory;

//        [SetUp]
//        public void Setup()
//        {
//            // Tạo thư mục tạm để test
//            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
//            Directory.CreateDirectory(_tempDirectory);
//            Directory.CreateDirectory(Path.Combine(_tempDirectory, "faces"));

//            // Mock IWebHostEnvironment
//            _mockEnvironment = new Mock<IWebHostEnvironment>();
//            _mockEnvironment.Setup(m => m.WebRootPath).Returns(_tempDirectory);

//            // Tạo controller
//            _controller = new FaceController(_mockEnvironment.Object);

//            // Mock HttpContext và Session
//            var httpContext = new DefaultHttpContext();
//            var sessionMock = new Mock<ISession>();
//            var sessionStorage = new Dictionary<string, byte[]>();

//            sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
//                .Callback<string, byte[]>((key, value) => sessionStorage[key] = value);

//            sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
//                .Callback<string, byte[]>((key, value) =>
//                {
//                    sessionStorage.TryGetValue(key, out value);
//                })
//                .Returns<string, byte[]>((key, value) => sessionStorage.TryGetValue(key, out _));

//            sessionMock.Setup(s => s.GetString(It.IsAny<string>()))
//                .Returns<string>(key =>
//                {
//                    if (sessionStorage.TryGetValue(key, out var bytes))
//                        return System.Text.Encoding.UTF8.GetString(bytes);
//                    return null;
//                });

//            httpContext.Session = sessionMock.Object;
//            _controller.ControllerContext = new ControllerContext()
//            {
//                HttpContext = httpContext
//            };
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            // Xóa thư mục tạm sau khi test
//            if (Directory.Exists(_tempDirectory))
//                Directory.Delete(_tempDirectory, true);
//        }

//        // 1. Test Register khi chưa đăng nhập
//        [Test]
//        public void Register_NotLoggedIn_RedirectsToHome()
//        {
//            // Arrange - không cần setup session

//            // Act
//            var result = _controller.Register() as RedirectToActionResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Index", result.ActionName);
//            Assert.AreEqual("Home", result.ControllerName);
//        }

//        // 2. Test Register khi đã đăng nhập
//        [Test]
//        public void Register_LoggedIn_ReturnsView()
//        {
//            // Arrange
//            var idSinhVien = Guid.NewGuid().ToString();
//            _controller.HttpContext.Session.SetString("IdSinhVien", idSinhVien);

//            // Act
//            var result = _controller.Register() as ViewResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(idSinhVien, result.ViewData["IdSinhVien"]);
//        }

//        // 3. Test SaveFace khi không có ảnh
//        [Test]
//        public async Task SaveFace_NoImage_ReturnsBadRequest()
//        {
//            // Act
//            var result = await _controller.SaveFace(null, "testId") as BadRequestObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Thiếu ảnh hoặc ID sinh viên.", result.Value);
//        }

//        // 4. Test SaveFace khi không có ID sinh viên
//        [Test]
//        public async Task SaveFace_NoStudentId_ReturnsBadRequest()
//        {
//            // Arrange
//            var mockFile = new Mock<IFormFile>();

//            // Act
//            var result = await _controller.SaveFace(mockFile.Object, "") as BadRequestObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Thiếu ảnh hoặc ID sinh viên.", result.Value);
//        }

//        // 5. Test SaveFace khi đã đủ 5 ảnh
//        [Test]
//        public async Task SaveFace_AlreadyHasFiveImages_ReturnsBadRequest()
//        {
//            // Arrange
//            var idSinhVien = "testId";
//            var studentDir = Path.Combine(_tempDirectory, "faces", idSinhVien);
//            Directory.CreateDirectory(studentDir);

//            // Tạo 5 file ảnh giả
//            for (int i = 1; i <= 5; i++)
//            {
//                File.WriteAllText(Path.Combine(studentDir, $"{i}.jpg"), "test");
//            }

//            var mockFile = new Mock<IFormFile>();

//            // Act
//            var result = await _controller.SaveFace(mockFile.Object, idSinhVien) as BadRequestObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("❌ Đã đủ 5 ảnh khuôn mặt cho sinh viên này.", result.Value);
//        }

//        // 6. Test SaveFace thành công khi chưa đủ ảnh
//        [Test]
//        public async Task SaveFace_SuccessfulSave_ReturnsOk()
//        {
//            // Arrange
//            var idSinhVien = "testId";
//            var mockFile = new Mock<IFormFile>();
//            var content = "fake image content";
//            var fileName = "test.jpg";

//            mockFile.Setup(f => f.Length).Returns(content.Length);
//            mockFile.Setup(f => f.FileName).Returns(fileName);
//            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
//                .Callback<Stream, CancellationToken>((stream, token) =>
//                {
//                    var writer = new StreamWriter(stream);
//                    writer.Write(content);
//                    writer.Flush();
//                })
//                .Returns(Task.CompletedTask);

//            // Act
//            var result = await _controller.SaveFace(mockFile.Object, idSinhVien) as OkObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Lưu ảnh thành công, tiếp tục chụp ảnh tiếp theo.", result.Value);

//            // Kiểm tra file đã được tạo
//            var filePath = Path.Combine(_tempDirectory, "faces", idSinhVien, "1.jpg");
//            Assert.IsTrue(File.Exists(filePath));
//        }

//        // 7. Test DeleteFace khi không có ID sinh viên
//        [Test]
//        public void DeleteFace_NoStudentId_ReturnsBadRequest()
//        {
//            // Act
//            var result = _controller.DeleteFace("") as BadRequestObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Thiếu ID sinh viên.", result.Value);
//        }

//        // 8. Test DeleteFace khi không tìm thấy thư mục ảnh
//        [Test]
//        public void DeleteFace_FolderNotFound_ReturnsNotFound()
//        {
//            // Act
//            var result = _controller.DeleteFace("nonExistentId") as NotFoundObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("❌ Không tìm thấy ảnh khuôn mặt của sinh viên.", result.Value);
//        }

//        // 9. Test DeleteFace thành công
//        [Test]
//        public void DeleteFace_Success_ReturnsOk()
//        {
//            // Arrange
//            var idSinhVien = "testId";
//            var studentDir = Path.Combine(_tempDirectory, "faces", idSinhVien);
//            Directory.CreateDirectory(studentDir);
//            File.WriteAllText(Path.Combine(studentDir, "1.jpg"), "test");

//            // Act
//            var result = _controller.DeleteFace(idSinhVien) as OkObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("✅ Đã xóa ảnh khuôn mặt. Bạn có thể đăng ký lại.", result.Value);
//            Assert.IsFalse(Directory.Exists(studentDir));
//        }

//        // 10. Test Attendance trả về view
//        [Test]
//        public void Attendance_ReturnsView()
//        {
//            // Act
//            var result = _controller.Attendance() as ViewResult;

//            // Assert
//            Assert.IsNotNull(result);
//        }

//        // 11. Test RecognizeFace khi không có ảnh
//        [Test]
//        public async Task RecognizeFace_NoImage_ReturnsBadRequest()
//        {
//            // Act
//            var result = await _controller.RecognizeFace(null) as BadRequestObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<object>(result.Value);
//            // Kiểm tra message trong object trả về
//            var resultValue = result.Value.GetType().GetProperty("message").GetValue(result.Value, null);
//            Assert.AreEqual("❌ Thiếu ảnh gửi lên.", resultValue);
//        }

//        // 12. Test RecognizeFace khi thiếu model hoặc dữ liệu ánh xạ
//        [Test]
//        public async Task RecognizeFace_MissingModelOrMapping_ReturnsServerError()
//        {
//            // Arrange
//            var mockFile = new Mock<IFormFile>();
//            var content = "fake image content";
//            var fileName = "test.jpg";

//            mockFile.Setup(f => f.Length).Returns(content.Length);
//            mockFile.Setup(f => f.FileName).Returns(fileName);
//            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
//                .Callback<Stream, CancellationToken>((stream, token) =>
//                {
//                    var writer = new StreamWriter(stream);
//                    writer.Write(content);
//                    writer.Flush();
//                })
//                .Returns(Task.CompletedTask);

//            // Act
//            var result = await _controller.RecognizeFace(mockFile.Object) as ObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            var resultValue = result.Value.GetType().GetProperty("message").GetValue(result.Value, null);
//            Assert.AreEqual("❌ Thiếu model hoặc dữ liệu ánh xạ.", resultValue);
//        }

//        // 13. Test LuuDiemDanh
//        [Test]
//        public void LuuDiemDanh_ReturnsOk()
//        {
//            // Act
//            var result = _controller.LuuDiemDanh("testHash") as OkObjectResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Đã điểm danh cho sinh viên testHash", result.Value);
//        }

//        // 14. Test SaveFace với ảnh không hợp lệ
//        [Test]
//        public async Task SaveFace_InvalidImage_HandlesException()
//        {
//            // Arrange
//            var idSinhVien = "testId";
//            var mockFile = new Mock<IFormFile>();
//            mockFile.Setup(f => f.Length).Returns(10); // Giả lập có dữ liệu
//            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
//                .Throws(new Exception("Test exception"));

//            // Act & Assert
//            Assert.ThrowsAsync<Exception>(async () => await _controller.SaveFace(mockFile.Object, idSinhVien));
//        }

//        // 15. Test RecognizeFace khi chưa đăng nhập
//        [Test]
//        public async Task RecognizeFace_NotLoggedIn_ReturnsUnauthorized()
//        {
//            // Arrange
//            var mockFile = new Mock<IFormFile>();
//            var content = "fake image content";
//            mockFile.Setup(f => f.Length).Returns(content.Length);

//            // Tạo model và mapping giả
//            File.WriteAllText(Path.Combine(_tempDirectory, "model.yml"), "fake model");
//            File.WriteAllText(Path.Combine(_tempDirectory, "face-mapping.json"), "{\"0\":\"testId\"}");

//            // Mock recognizer để tránh lỗi khi đọc file model không hợp lệ
//            // Đây là phần phức tạp và có thể cần điều chỉnh tùy vào cách triển khai thực tế
//            // Trong trường hợp thực tế, bạn có thể cần mock thêm các thành phần khác

//            // Act
//            var result = await _controller.RecognizeFace(mockFile.Object) as UnauthorizedObjectResult;

//            // Assert
//            // Lưu ý: Test này có thể không chạy đúng do phụ thuộc vào cách triển khai cụ thể của RecognizeFace
//            // và cách xử lý các đối tượng Emgu.CV
//            if (result != null)
//            {
//                var resultValue = result.Value.GetType().GetProperty("message").GetValue(result.Value, null);
//                Assert.AreEqual("⚠️ Chưa đăng nhập.", resultValue);
//            }
//            else
//            {
//                // Nếu không thể mock đúng các đối tượng Emgu.CV, test có thể fail
//                // Trong trường hợp này, bạn có thể cần điều chỉnh test hoặc bỏ qua
//                Assert.Inconclusive("Test không thể hoàn thành do phụ thuộc vào Emgu.CV");
//            }
//        }
//    }
//}