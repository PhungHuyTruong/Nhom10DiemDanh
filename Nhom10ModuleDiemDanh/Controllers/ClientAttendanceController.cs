using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClienNhom10ModuleDiemDanht.Controllers
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
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/schedule?email={email}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API call failed with status {response.StatusCode}: {errorContent}");
                    ViewBag.Error = $"API error: {errorContent}";
                    return RedirectToAction("Index", "SinhViens");
                }

                var schedules = await response.Content.ReadFromJsonAsync<List<ScheduleDto>>();
                Console.WriteLine($"Successfully retrieved {schedules?.Count ?? 0} schedules for email {email}");
                if (schedules == null || !schedules.Any())
                {
                    Console.WriteLine($"No schedules found for email {email}");
                    ViewBag.Error = "No schedules found for today.";
                    return View(schedules);
                }

                return View(schedules);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize API response for email {email}: {ex.Message}");
                ViewBag.Error = "Error processing schedule data.";
                return RedirectToAction("Index", "SinhViens");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed for email {email}: {ex.Message}");
                ViewBag.Error = "Error connecting to server.";
                return RedirectToAction("Index", "SinhViens");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error for email {email}: {ex.Message}");
                ViewBag.Error = "An unexpected error occurred.";
                return RedirectToAction("Index", "SinhViens");
            }
        }

        //public async Task<IActionResult> Index()
        //{
        //    var email = User.FindFirst(ClaimTypes.Email)?.Value;
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        Console.WriteLine("No email found in claims, redirecting to login.");
        //        return RedirectToAction("Login", "Account");
        //    }

        //    List<ScheduleDto> schedules = null;

        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{_apiUrl}/schedule?email={email}");
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine($"API call failed with status {response.StatusCode}: {errorContent}");
        //            ViewBag.Error = $"API error: {errorContent}";
        //            return View(schedules); // Không redirect về chính mình
        //        }

        //        schedules = await response.Content.ReadFromJsonAsync<List<ScheduleDto>>();
        //        Console.WriteLine($"Successfully retrieved {schedules?.Count ?? 0} schedules for email {email}");

        //        if (schedules == null || !schedules.Any())
        //        {
        //            Console.WriteLine($"No schedules found for email {email}");
        //            ViewBag.Error = "Không có lịch học nào cho hôm nay.";
        //        }

        //        return View(schedules);
        //    }
        //    catch (JsonException ex)
        //    {
        //        Console.WriteLine($"Failed to deserialize API response for email {email}: {ex.Message}");
        //        ViewBag.Error = "Lỗi xử lý dữ liệu.";
        //        return View(schedules);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine($"HTTP request failed for email {email}: {ex.Message}");
        //        ViewBag.Error = "Lỗi kết nối đến server.";
        //        return View(schedules);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Unexpected error for email {email}: {ex.Message}");
        //        ViewBag.Error = "Đã xảy ra lỗi không xác định.";
        //        return View(schedules);
        //    }
        //}

        // Handle check-in/check-out
        [HttpPost]
        public async Task<IActionResult> CheckAttendance(Guid idNXCH, bool isCheckIn)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Please login" });

            var dto = new
            {
                IdNXCH = idNXCH,
                Email = email,
                IsCheckIn = isCheckIn
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
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during check attendance for IdNXCH {idNXCH}: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}