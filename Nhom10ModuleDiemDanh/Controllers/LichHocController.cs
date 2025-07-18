using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nhom10ModuleDiemDanh.Models; // nơi chứa LichHocViewDto

public class LichHocController : Controller
{
    private readonly HttpClient _httpClient;

    public LichHocController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MyApi"); // cấu hình trong Program.cs
    }

    public async Task<IActionResult> Index()
    {
        // Giả sử bạn lưu IdSinhVien vào session khi đăng nhập
        var idSinhVien = HttpContext.Session.GetString("IdSinhVien");

        if (string.IsNullOrEmpty(idSinhVien))
        {
            return Unauthorized();
        }

        var result = await _httpClient.GetFromJsonAsync<List<LichHocViewDto>>($"LichHocViews/sinhvien/{idSinhVien}");
        return View(result);
    }
}
