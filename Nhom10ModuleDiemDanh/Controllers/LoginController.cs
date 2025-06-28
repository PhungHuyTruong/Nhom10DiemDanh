using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data; 

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LoginController : Controller
    {
        private readonly ModuleDiemDanhDbContext _dbContext; 

        public LoginController(ModuleDiemDanhDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            Console.WriteLine("Hiển thị trang login.");
            return View();
        }

        public IActionResult Login(string role)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
            };

            HttpContext.Session.SetString("SelectedRole", role);
            Console.WriteLine($"Bắt đầu đăng nhập với vai trò: {role}");

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                Console.WriteLine("Đăng nhập thất bại hoặc bị hủy.");
                TempData["Error"] = "Bạn đã hủy đăng nhập hoặc có lỗi xảy ra. Vui lòng thử lại!";
                return RedirectToAction("Index", "Home");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var selectedRole = HttpContext.Session.GetString("SelectedRole")?.ToLower() ?? "";

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Không lấy được email từ Google.");
                TempData["Error"] = "Không thể xác thực email.";
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("UserEmail", email!); // OK vì đã kiểm tra null


            Console.WriteLine($"Đăng nhập thành công với email: {email}, vai trò được chọn: {selectedRole}");

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Không lấy được email từ Google.");
                TempData["Error"] = "Không thể xác thực email.";
                return RedirectToAction("Index", "Home");
            }

            bool isAuthorized = false;
            string redirectAction = "Index";
            string redirectController = "Home";

            try
            {
                switch (selectedRole)
                {
                    case "canbo":
                        // Kiểm tra email trong bảng BanDaoTao
                        isAuthorized = await _dbContext.BanDaoTaos
                            .AnyAsync(b => b.Email == email && b.TrangThai);
                        redirectController = "CanBoDaoTao";
                        Console.WriteLine($"Kiểm tra quyền cán bộ đào tạo: {(isAuthorized ? "Thành công" : "Thất bại")}");
                        break;

                    //case "phutrachxuong":
                    //    // Kiểm tra email và vai trò phụ trách xưởng
                    //    isAuthorized = await _dbContext.VaiTroNhanViens
                    //        .Include(v => v.PhuTrachXuong)
                    //        .Include(v => v.VaiTro)
                    //        .AnyAsync(v => (v.PhuTrachXuong.EmailFE == email || v.PhuTrachXuong.EmailFPT == email)
                    //                    && v.TrangThai
                    //                    && v.VaiTro.TenVaiTro.ToLower() == "phutrachxuong");
                    //    redirectController = "PhuTrachXuong";
                    //    Console.WriteLine($"Kiểm tra quyền phụ trách xưởng: {(isAuthorized ? "Thành công" : "Thất bại")}");
                    //    break;

                    //case "giangvien":
                    //    // Kiểm tra email và vai trò giảng viên
                    //    isAuthorized = await _dbContext.VaiTroNhanViens
                    //        .Include(v => v.PhuTrachXuong)
                    //        .Include(v => v.VaiTro)
                    //        .AnyAsync(v => (v.PhuTrachXuong.EmailFE == email || v.PhuTrachXuong.EmailFPT == email)
                    //                    && v.TrangThai
                    //                    && v.VaiTro.TenVaiTro.ToLower() == "giangvien");
                    //    redirectController = "GiangVien";
                    //    Console.WriteLine($"Kiểm tra quyền giảng viên: {(isAuthorized ? "Thành công" : "Thất bại")}");
                    //    break;
                    case "phutrachxuong":
                        // Chỉ kiểm tra email, không kiểm tra vai trò nữa
                        isAuthorized = await _dbContext.VaiTroNhanViens
                            .Include(v => v.PhuTrachXuong)
                            .AnyAsync(v => (v.PhuTrachXuong.EmailFE == email || v.PhuTrachXuong.EmailFPT == email)
                                        && v.TrangThai);
                        redirectController = "PhuTrachXuong";
                        Console.WriteLine($"Kiểm tra quyền phụ trách xưởng (theo email): {(isAuthorized ? "Thành công" : "Thất bại")}");
                        break;

                    case "giangvien":
                        // Chỉ kiểm tra email, không kiểm tra vai trò nữa
                        isAuthorized = await _dbContext.VaiTroNhanViens
                            .Include(v => v.PhuTrachXuong)
                            .AnyAsync(v => (v.PhuTrachXuong.EmailFE == email || v.PhuTrachXuong.EmailFPT == email)
                                        && v.TrangThai);
                        redirectController = "GiangVien";
                        Console.WriteLine($"Kiểm tra quyền giảng viên (theo email): {(isAuthorized ? "Thành công" : "Thất bại")}");
                        break;

                    case "sinhvien":
                        // Kiểm tra email trong bảng SinhVien
                        var sinhVien = await _dbContext.SinhViens
                            .FirstOrDefaultAsync(s => s.Email == email && s.TrangThai);
                        isAuthorized = sinhVien != null;

                        if (isAuthorized)
                        {
                            // Lưu thông tin sinh viên vào session
                            HttpContext.Session.SetString("SinhVienId", sinhVien.IdSinhVien.ToString());
                            HttpContext.Session.SetString("SinhVienEmail", sinhVien.Email);
                            HttpContext.Session.SetString("SinhVienTen", sinhVien.TenSinhVien);
                            HttpContext.Session.SetString("SinhVienMa", sinhVien.MaSinhVien);
                        }

                        var sinhVien = await _dbContext.SinhViens.FirstOrDefaultAsync(s => s.Email == email && s.TrangThai);
                        if (sinhVien != null)
                        {
                            isAuthorized = true;
                            HttpContext.Session.SetString("IdSinhVien", sinhVien.IdSinhVien.ToString());
                        }
                        redirectController = "SinhViens";
                        Console.WriteLine($"Kiểm tra quyền sinh viên: {(isAuthorized ? "Thành công" : "Thất bại")}");
                        break;

                    default:
                        Console.WriteLine("Vai trò không hợp lệ.");
                        TempData["Error"] = "Không xác định được vai trò đăng nhập.";
                        return RedirectToAction("Index", "Home");
                }

                if (!isAuthorized)
                {
                    Console.WriteLine($"Không có quyền truy cập cho email {email} với vai trò {selectedRole}.");
                    TempData["Error"] = "Bạn không có quyền truy cập với vai trò này.";
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine($"Chuyển hướng đến {redirectController}/{redirectAction}");
                return RedirectToAction(redirectAction, redirectController);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra phân quyền: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi xác thực vai trò.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Đăng xuất người dùng.");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
