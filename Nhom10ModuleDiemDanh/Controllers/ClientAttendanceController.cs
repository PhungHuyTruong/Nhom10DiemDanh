using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class ClientAttendanceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7296/api/Attendance";

        public ClientAttendanceController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Model to deserialize API response
        public class AttendanceResult
        {
            public int Status { get; set; }
            public string Message { get; set; }
        }

        // Display today's schedule
        public async Task<IActionResult> Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("No email found in claims, redirecting to login.");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/schedule?email={email}");
                Console.WriteLine($"API response status: {response.StatusCode} for email: {email}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API call failed with status {response.StatusCode}: {errorContent}");
                    ViewBag.Error = $"Lỗi API: {errorContent}";
                    return View(new List<ScheduleDto>());
                }

                var schedules = await response.Content.ReadFromJsonAsync<List<ScheduleDto>>();
                Console.WriteLine($"Successfully retrieved {schedules?.Count ?? 0} schedules for email {email}");

                if (schedules == null || !schedules.Any())
                {
                    Console.WriteLine($"No schedules found for email {email}");
                    ViewBag.Error = "Không tìm thấy lịch học cho hôm nay.";
                    return View(new List<ScheduleDto>());
                }

                return View(schedules);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize API response for email {email}: {ex.Message}");
                ViewBag.Error = "Lỗi xử lý dữ liệu lịch học.";
                return View(new List<ScheduleDto>());
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed for email {email}: {ex.Message}");
                ViewBag.Error = "Lỗi kết nối đến server.";
                return View(new List<ScheduleDto>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error for email {email}: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi không mong muốn.";
                return View(new List<ScheduleDto>());
            }
        }

        // Handle check-in/check-out
        [HttpPost]
        public async Task<IActionResult> CheckAttendance(Guid idNXCH, bool isCheckIn, string ipAddress = null, double? latitude = null, double? longitude = null)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            // Get client IP if not provided
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = GetClientIPAddress();
            }

            var dto = new
            {
                IdNXCH = idNXCH,
                Email = email,
                IsCheckIn = isCheckIn,
                IPAddress = ipAddress,
                Latitude = latitude,
                Longitude = longitude
            };

            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(dto),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{_apiUrl}/check", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Check attendance failed with status {response.StatusCode}: {error}");
                    return Json(new { success = false, message = error });
                }

                var result = await response.Content.ReadFromJsonAsync<AttendanceResult>();
                Console.WriteLine($"Attendance recorded for IdNXCH {idNXCH} with status {result.Status}");
                return Json(new { success = true, message = result.Message });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize API response for IdNXCH {idNXCH}: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during check attendance for IdNXCH {idNXCH}: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // Get client IP address
        private string GetClientIPAddress()
        {
            var forwarded = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
            {
                return forwarded.Split(',')[0].Trim();
            }

            var realIP = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
            {
                return realIP;
            }

            return Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }
    }
}