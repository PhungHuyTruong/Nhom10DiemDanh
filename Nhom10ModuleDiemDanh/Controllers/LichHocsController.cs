using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nhom10ModuleDiemDanh.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichHocsController : Controller
    {
        private readonly HttpClient _httpClient;

        public LichHocsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/LichHocs");
            if (!response.IsSuccessStatusCode)
                return Problem("Không thể lấy dữ liệu lịch học từ API");

            var jsonData = await response.Content.ReadAsStringAsync();
            var lichHocList = JsonConvert.DeserializeObject<List<LichHocDtoV>>(jsonData);

            return View(lichHocList);
        }
    }
}
