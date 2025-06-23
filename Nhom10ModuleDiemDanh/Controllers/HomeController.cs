using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string error = null)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.ErrorMessage = error.Contains("access_denied")
                    ? "B?n ?ã h?y ??ng nh?p Google."
                    : "?ã x?y ra l?i khi ??ng nh?p. Vui lòng th? l?i!";
            }

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
