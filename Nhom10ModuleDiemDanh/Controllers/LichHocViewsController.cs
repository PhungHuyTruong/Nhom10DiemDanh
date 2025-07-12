using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nhom10ModuleDiemDanh.Models; // nơi chứa LichHocViewDto

public class LichHocViewsController : Controller
{
    private readonly HttpClient _httpClient;

    public LichHocViewsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MyApi"); // cấu hình trong Program.cs
    }

    public async Task<IActionResult> Index()
    {
        var result = await _httpClient.GetFromJsonAsync<List<LichHocViewDto>>("LichHocViews");
        return View(result);
    }
}
