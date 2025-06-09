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
            return View("SinhVien");
        }

    }
}
