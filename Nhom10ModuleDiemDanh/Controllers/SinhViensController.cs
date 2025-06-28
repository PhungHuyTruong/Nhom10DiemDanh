using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class SinhViensController : Controller
    {
        public IActionResult Index()
        {
            // Lấy thông tin sinh viên từ session
            var sinhVienId = HttpContext.Session.GetString("SinhVienId");
            var sinhVienEmail = HttpContext.Session.GetString("SinhVienEmail");
            var sinhVienTen = HttpContext.Session.GetString("SinhVienTen");
            var sinhVienMa = HttpContext.Session.GetString("SinhVienMa");

            // Kiểm tra xem sinh viên đã đăng nhập chưa
            if (string.IsNullOrEmpty(sinhVienId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Truyền thông tin sinh viên vào ViewBag
            ViewBag.SinhVienId = sinhVienId;
            ViewBag.SinhVienEmail = sinhVienEmail;
            ViewBag.SinhVienTen = sinhVienTen;
            ViewBag.SinhVienMa = sinhVienMa;
            return View("SinhViens");
        }

    }
}
