using API.Data;
using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichGiangDaysController : Controller
    {
        private readonly HttpClient _httpClient;

        public LichGiangDaysController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7296/");
        }

        // GET: LichGiangDays
        public async Task<IActionResult> Schedule()
        {
            var idNhanVienStr = HttpContext.Session.GetString("IdNhanVien");
            if (string.IsNullOrEmpty(idNhanVienStr)) return RedirectToAction("Login");

            Guid idNhanVien = Guid.Parse(idNhanVienStr);

            // Gọi API: GET api/lichgiangday/giangvien/{id}
            var response = await _httpClient.GetFromJsonAsync<List<LichGiangDayViewModel>>(
                $"api/lichgiangdays/giangvien/{idNhanVien}"
            );

            return View(response);
        }

    }
}
