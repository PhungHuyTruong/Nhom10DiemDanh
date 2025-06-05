using Microsoft.AspNetCore.Mvc;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class SinhVienController : Controller
    {
        public IActionResult Index()
        {
            return View("SinhVien");
        }
    }
}
