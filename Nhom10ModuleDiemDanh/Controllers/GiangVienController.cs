using Microsoft.AspNetCore.Mvc;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class GiangVienController : Controller
    {
        public IActionResult Index()
        {
            return View("GiangVien");
        }
    }
}
