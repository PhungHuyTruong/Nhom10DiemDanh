using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class FaceController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public FaceController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Register()
        {
            var id = HttpContext.Session.GetString("IdSinhVien");
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Home"); // hoặc thông báo lỗi

            ViewBag.IdSinhVien = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveFace(IFormFile image, string idSinhVien)
        {
            if (image == null || image.Length == 0 || string.IsNullOrWhiteSpace(idSinhVien))
                return BadRequest("Thiếu ảnh hoặc ID sinh viên.");

            var studentDir = Path.Combine(_env.WebRootPath, "faces", idSinhVien);
            if (!Directory.Exists(studentDir)) Directory.CreateDirectory(studentDir);

            var fileCount = Directory.GetFiles(studentDir, "*.jpg").Length;
            if (fileCount >= 5)
                return BadRequest("❌ Đã đủ 5 ảnh khuôn mặt cho sinh viên này.");

            var filePath = Path.Combine(studentDir, $"{fileCount + 1}.jpg");
            using (var stream = new FileStream(filePath, FileMode.Create))
                await image.CopyToAsync(stream);

            // Huấn luyện lại model
            var faceImages = new List<Mat>();
            var labels = new List<int>();
            var labelMap = new Dictionary<int, string>();

            var facesRoot = Path.Combine(_env.WebRootPath, "faces");
            var studentFolders = Directory.GetDirectories(facesRoot);
            int currentLabel = 0;

            foreach (var folder in studentFolders)
            {
                var folderName = Path.GetFileName(folder);
                foreach (var file in Directory.GetFiles(folder, "*.jpg"))
                {
                    try
                    {
                        var img = new Image<Gray, byte>(file).Resize(200, 200, Emgu.CV.CvEnum.Inter.Linear).Mat;
                        faceImages.Add(img);
                        labels.Add(currentLabel);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Bỏ qua ảnh lỗi: {file}, lỗi: {ex.Message}");
                    }
                }

                labelMap[currentLabel] = folderName;
                currentLabel++;
            }

            if (faceImages.Count < 5)
                return Ok("Lưu ảnh thành công, tiếp tục chụp ảnh tiếp theo.");

            var recognizer = new LBPHFaceRecognizer();
            var vecImages = new VectorOfMat();
            vecImages.Push(faceImages.ToArray());

            var vecLabels = new VectorOfInt(labels.ToArray());
            recognizer.Train(vecImages, vecLabels);

            var modelPath = Path.Combine(_env.WebRootPath, "model.yml");
            recognizer.Write(modelPath);

            var mapPath = Path.Combine(_env.WebRootPath, "face-mapping.json");
            System.IO.File.WriteAllText(mapPath, JsonConvert.SerializeObject(labelMap));

            return Ok("✅ Đã lưu ảnh và huấn luyện thành công.");
        }


        [HttpPost]
        public IActionResult DeleteFace(string idSinhVien)
        {
            if (string.IsNullOrWhiteSpace(idSinhVien))
                return BadRequest("Thiếu ID sinh viên.");

            var folderPath = Path.Combine(_env.WebRootPath, "faces", idSinhVien);
            if (!Directory.Exists(folderPath))
                return NotFound("❌ Không tìm thấy ảnh khuôn mặt của sinh viên.");

            Directory.Delete(folderPath, recursive: true);
            return Ok("✅ Đã xóa ảnh khuôn mặt. Bạn có thể đăng ký lại.");
        }

        public IActionResult Attendance() => View();

        // Sửa tất cả các chỗ trả về BadRequest hoặc Content
        // thành JsonResult hoặc ObjectResult với object JSON


        [HttpPost]
        public async Task<IActionResult> RecognizeFace(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "❌ Thiếu ảnh gửi lên." });

            // 📁 Tạo file tạm có tên duy nhất
            var tempFileName = $"{Guid.NewGuid()}.jpg";
            var tempPath = Path.Combine(_env.WebRootPath, tempFileName);

            try
            {
                // 📥 Lưu ảnh vào file tạm
                using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await image.CopyToAsync(stream);
                }

                // 🔍 Kiểm tra model và ánh xạ tồn tại
                var modelPath = Path.Combine(_env.WebRootPath, "model.yml");
                var mapPath = Path.Combine(_env.WebRootPath, "face-mapping.json");

                if (!System.IO.File.Exists(modelPath) || !System.IO.File.Exists(mapPath))
                    return StatusCode(500, new { message = "❌ Thiếu model hoặc dữ liệu ánh xạ." });

                // 📖 Tải model
                var recognizer = new LBPHFaceRecognizer();
                recognizer.Read(modelPath);

                // 🧠 Nhận diện khuôn mặt
                using (var testImage = new Image<Gray, byte>(tempPath).Resize(200, 200, Emgu.CV.CvEnum.Inter.Linear))
                {
                    var result = recognizer.Predict(testImage);
                    Console.WriteLine($"🔍 Dự đoán: Label={result.Label}, Distance={result.Distance}");

                    if (result.Label == -1 || result.Distance > 75)
                        return Ok(new { success = false, message = "❌ Không nhận diện được khuôn mặt." });

                    // 📑 Đọc file ánh xạ
                    var json = System.IO.File.ReadAllText(mapPath);
                    var labelMap = JsonConvert.DeserializeObject<Dictionary<int, string>>(json);

                    if (!labelMap.TryGetValue(result.Label, out var matchedId))
                        return Ok(new { success = false, message = "❌ Không tìm thấy sinh viên phù hợp." });

                    // 🧑‍🎓 Kiểm tra sinh viên đăng nhập
                    var loggedInId = HttpContext.Session.GetString("IdSinhVien");
                    if (string.IsNullOrEmpty(loggedInId))
                        return Unauthorized(new { message = "⚠️ Chưa đăng nhập." });

                    if (matchedId != loggedInId)
                        return Ok(new { success = false, message = "❌ Khuôn mặt không khớp với sinh viên đang đăng nhập." });

                    // 💾 Lưu điểm danh
                    var status = await LuuDiemDanhNoAuth(matchedId);

                    return Ok(new
                    {
                        success = true,
                        message = $"✅ Điểm danh thành công cho sinh viên: {matchedId}",
                        log = status
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi nhận diện: {ex.Message}");
                return StatusCode(500, new { message = "❌ Lỗi xử lý ảnh." });
            }
            finally
            {
                // 🧹 Xoá file tạm nếu có
                if (System.IO.File.Exists(tempPath))
                {
                    try { System.IO.File.Delete(tempPath); } catch { /* ignore */ }
                }
            }
        }



        //[HttpPost]
        //public async Task<IActionResult> RecognizeFace(IFormFile image)
        //{
        //    if (image == null || image.Length == 0)
        //        return BadRequest(new { message = "Thiếu ảnh gửi lên." });

        //    var sessionId = HttpContext.Session.GetString("IdSinhVien");
        //    if (string.IsNullOrEmpty(sessionId))
        //        return Unauthorized(new { message = "❌ Bạn chưa đăng nhập." });

        //    var tempPath = Path.Combine(_env.WebRootPath, "temp.jpg");
        //    using (var stream = new FileStream(tempPath, FileMode.Create))
        //        await image.CopyToAsync(stream);

        //    var modelPath = Path.Combine(_env.WebRootPath, "model.yml");
        //    var mapPath = Path.Combine(_env.WebRootPath, "face-mapping.json");

        //    if (!System.IO.File.Exists(modelPath) || !System.IO.File.Exists(mapPath))
        //        return StatusCode(500, new { message = "❌ Thiếu model hoặc dữ liệu ánh xạ." });

        //    var recognizer = new LBPHFaceRecognizer();
        //    recognizer.Read(modelPath);

        //    var testImage = new Image<Gray, byte>(tempPath).Resize(200, 200, Emgu.CV.CvEnum.Inter.Linear);
        //    var result = recognizer.Predict(testImage);

        //    Console.WriteLine($"🔍 Dự đoán: Label={result.Label}, Distance={result.Distance}");

        //    if (result.Label == -1 || result.Distance > 75)
        //        return Ok(new { success = false, message = "❌ Không nhận diện được khuôn mặt." });

        //    var json = System.IO.File.ReadAllText(mapPath);
        //    var labelMap = JsonConvert.DeserializeObject<Dictionary<int, string>>(json);

        //    if (!labelMap.TryGetValue(result.Label, out var matchedId))
        //        return Ok(new { success = false, message = "❌ Không tìm thấy sinh viên phù hợp." });

        //    if (matchedId != sessionId)
        //        return Ok(new { success = false, message = "❌ Khuôn mặt không khớp với tài khoản đăng nhập." });

        //    var status = await LuuDiemDanhNoAuth(matchedId);
        //    return Ok(new { success = true, message = $"✅ Điểm danh thành công cho sinh viên: {matchedId}", log = status });
        //}



        private async Task<string> LuuDiemDanhNoAuth(string idSinhVien)
        {
            try
            {
                // TODO: lưu vào DB ở đây thay vì file nếu bạn có context
                var path = Path.Combine(_env.WebRootPath, "diemdanh-log.txt");
                await System.IO.File.AppendAllTextAsync(path, $"{DateTime.Now}: {idSinhVien}\n");
                return "📝 Đã lưu điểm danh.";
            }
            catch (Exception ex)
            {
                return "❌ Lỗi khi lưu điểm danh: " + ex.Message;
            }
        }

        [HttpPost]
        public IActionResult LuuDiemDanh(string idHash)
        {
            return Ok("Đã điểm danh cho sinh viên " + idHash);
        }
    }
}
