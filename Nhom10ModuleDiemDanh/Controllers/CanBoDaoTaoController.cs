using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class CanBoDaoTaoController : Controller
    {
        //[Authorize]
        public IActionResult Index()
        {
            return View("CanBoDaoTao");
        }
    }
}
