    using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using API.Data; // dùng lại model từ API
using System.Security.Claims;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class NhomXuongCuaToiController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7296/api/NhomXuongCuaToi";

        public NhomXuongCuaToiController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var email = User.Identity.IsAuthenticated
                    ? User.FindFirst(ClaimTypes.Email)?.Value
                    : null;

                var requestUrl = string.IsNullOrEmpty(email)
                    ? _apiUrl
                    : $"{_apiUrl}?email={email}";

                Console.WriteLine($"📤 Gửi request API với email: {email}");

                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var nhomXuongs = await response.Content.ReadFromJsonAsync<List<NhomXuong>>();
                    return View(nhomXuongs);
                }

                ViewBag.Error = "Không thể lấy dữ liệu nhóm xưởng.";
                return View(new List<NhomXuong>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View(new List<NhomXuong>());
            }
        }
    }
}
