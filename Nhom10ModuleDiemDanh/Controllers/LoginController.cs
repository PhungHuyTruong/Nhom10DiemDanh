using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LoginController : Controller
    {
        // Trang login
        public IActionResult Index()
        {
            return View();
        }

        // ✅ ĐÃ SỬA: Thêm kiểu trả về IActionResult và return Challenge
        // Để xử lý đăng nhập với Google đúng chuẩn ASP.NET Core
        public IActionResult Login()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse") // Sau khi login xong, chuyển về đây
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        // Xử lý khi Google trả về dữ liệu sau khi đăng nhập thành công
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                TempData["Error"] = "Bạn đã hủy đăng nhập hoặc có lỗi xảy ra. Vui lòng thử lại!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "CanBoDaoTao");
        }

        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
