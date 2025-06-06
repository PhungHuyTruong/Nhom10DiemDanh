using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using API.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class PhuTrachXuongController : Controller
    {
        private readonly HttpClient _httpClient;

        public PhuTrachXuongController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5017/api/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Thêm query include nếu API hỗ trợ
                var list = await _httpClient.GetFromJsonAsync<List<PhuTrachXuong>>("PhuTrachXuongs?include=CoSo");
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lấy danh sách: " + ex.Message;
                return View(new List<PhuTrachXuong>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var item = await _httpClient.GetFromJsonAsync<PhuTrachXuong>($"PhuTrachXuongs/{id}");
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin phụ trách xưởng";
                    return RedirectToAction(nameof(Index));
                }

                if (item.IdCoSo != null)
                {
                    try
                    {
                        var coSo = await _httpClient.GetFromJsonAsync<CoSo>($"CoSos/{item.IdCoSo}");
                        item.CoSo = coSo;
                    }
                    catch (HttpRequestException)
                    {
                        // Ghi log nếu cần
                        item.CoSo = null;
                    }
                }

                return View(item);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải dữ liệu";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Create()
        {
            await LoadCoSoListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhuTrachXuong model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCoSoListAsync();
                return View(model);
            }

            try
            {
                model.IdNhanVien = Guid.NewGuid();

                var response = await _httpClient.PostAsJsonAsync("PhuTrachXuongs", model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Tạo mới thất bại: " + errorContent);
                await LoadCoSoListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo mới: " + ex.Message);
                await LoadCoSoListAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var item = await _httpClient.GetFromJsonAsync<PhuTrachXuong>($"PhuTrachXuongs/{id}");
                if (item == null)
                    return NotFound();

                await LoadCoSoListAsync(item.IdCoSo);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lấy dữ liệu để sửa: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PhuTrachXuong model)
        {
            if (id != model.IdNhanVien)
                return BadRequest("ID không khớp");

            if (!ModelState.IsValid)
            {
                await LoadCoSoListAsync(model.IdCoSo);
                return View(model);
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"PhuTrachXuongs/{id}", model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Cập nhật thất bại: " + errorContent);
                await LoadCoSoListAsync(model.IdCoSo);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                await LoadCoSoListAsync(model.IdCoSo);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var item = await _httpClient.GetFromJsonAsync<PhuTrachXuong>($"PhuTrachXuongs/{id}");
                if (item == null)
                    return NotFound();

                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lấy dữ liệu để xóa: " + ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"PhuTrachXuongs/{id}");
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Xóa thất bại: " + errorContent);

                var item = await _httpClient.GetFromJsonAsync<PhuTrachXuong>($"PhuTrachXuongs/{id}");
                return View("Delete", item);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa: " + ex.Message);
                var item = await _httpClient.GetFromJsonAsync<PhuTrachXuong>($"PhuTrachXuongs/{id}");
                return View("Delete", item);
            }
        }

        private async Task LoadCoSoListAsync(Guid? selectedId = null)
        {
            try
            {
                // Thêm timeout và headers
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
                var response = await _httpClient.GetAsync("CoSo");

                // Debug HTTP status
                Console.WriteLine($"API Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonString}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var coSoList = JsonSerializer.Deserialize<List<CoSo>>(jsonString, options);

                    // Debug deserialized data
                    if (coSoList != null)
                    {
                        Console.WriteLine($"Deserialized {coSoList.Count} items");
                        foreach (var item in coSoList)
                        {
                            Console.WriteLine($"{item.IdCoSo} - {item.DiaChi}");
                        }
                    }

                    ViewBag.IdCoSo = new SelectList(coSoList ?? new List<CoSo>(), "IdCoSo", "DiaChi", selectedId);
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    ViewBag.IdCoSo = new SelectList(new List<CoSo>(), "IdCoSo", "DiaChi");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.ToString()}");
                ViewBag.IdCoSo = new SelectList(new List<CoSo>(), "IdCoSo", "DiaChi");
            }
        }
    } 
}