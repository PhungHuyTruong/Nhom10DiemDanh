using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class PhuTrachXuongController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
