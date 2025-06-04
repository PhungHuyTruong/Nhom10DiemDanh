using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

       
        public IActionResult Login(string role)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
            };

            HttpContext.Session.SetString("SelectedRole", role);

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }


        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                TempData["Error"] = "Bạn đã hủy đăng nhập hoặc có lỗi xảy ra. Vui lòng thử lại!";
                return RedirectToAction("Index", "Home");
            }

            var selectedRole = HttpContext.Session.GetString("SelectedRole") ?? "";

            switch (selectedRole.ToLower())
            {
                case "canbo":
                    return RedirectToAction("Index", "CanBoDaoTao");

                case "phutrachxuong":
                    return RedirectToAction("Index", "PhuTrachXuong");

                case "giangvien":
                    return RedirectToAction("Index", "GiangVien");

                case "sinhvien":
                    return RedirectToAction("Index", "SinhVien");

                default:
                    TempData["Error"] = "Không xác định được vai trò đăng nhập.";
                    return RedirectToAction("Index", "Home");
            }
        }


        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
