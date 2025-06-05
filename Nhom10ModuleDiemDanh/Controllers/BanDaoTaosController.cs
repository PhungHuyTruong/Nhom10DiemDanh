﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Newtonsoft.Json;
using System.Text;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class BanDaoTaosController : Controller
    {
        private readonly string apiUrl = "https://localhost:7296/api/BanDaoTao";


        public BanDaoTaosController()
        {

        }

        // GET: BanDaoTaos
        public async Task<IActionResult> Index(int page = 1, string search = "", string status = "")
        {
            int pageSize = 5;

            var pagedData = new
            {
                data = new List<BanDaoTao>(),
                pagination = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalItems = 0,
                    totalPages = 0
                }
            };

            using (HttpClient client = new HttpClient())
            {
                // Encode các tham số để tránh lỗi khi có ký tự đặc biệt
                string encodedSearch = Uri.EscapeDataString(search ?? "");
                string encodedStatus = Uri.EscapeDataString(status ?? "");

                var url = $"{apiUrl}/paging?page={page}&pageSize={pageSize}&search={encodedSearch}&status={encodedStatus}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    pagedData = JsonConvert.DeserializeAnonymousType(json, pagedData);
                }
            }

            // Truyền các tham số lọc để View giữ lại giá trị
            ViewBag.Pagination = pagedData.pagination;
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(pagedData.data);
        }


        // GET: BanDaoTaos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BanDaoTao banDaoTao = new BanDaoTao();
            using (HttpClient client = new HttpClient())
            {
                using (var reponse = await client.GetAsync($"{apiUrl}/{id}"))
                {
                    if (reponse.IsSuccessStatusCode)
                    {
                        string api = await reponse.Content.ReadAsStringAsync();
                        banDaoTao = JsonConvert.DeserializeObject<BanDaoTao>(api);
                    }
                    else
                    {
                        return NotFound();
                    }
                }

            }
            return View(banDaoTao);

        }

        // GET: BanDaoTaos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BanDaoTaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenBanDaoTao,MaBanDaoTao,Email,IdVaiTro")] BanDaoTao banDaoTao)
        {
            if (!ModelState.IsValid)
                return View(banDaoTao);

            using (var httpClient = new HttpClient())
            {
                string apiUrl = "https://localhost:7296/api/BanDaoTao"; // <-- Đúng URL API
                var content = new StringContent(JsonConvert.SerializeObject(banDaoTao), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi API: {error}");
                return RedirectToAction("Index");
            }
        }

        // GET: BanDaoTaos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BanDaoTao banDaoTao = new BanDaoTao();
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.GetAsync($"{apiUrl}/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string api = await response.Content.ReadAsStringAsync();
                        banDaoTao = JsonConvert.DeserializeObject<BanDaoTao>(api);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            return View(banDaoTao);
        }

        // POST: BanDaoTaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IdBanDaoTao,TenBanDaoTao,MaBanDaoTao,Email,IdVaiTro,NgayTao,NgayCapNhat,TrangThai")] BanDaoTao banDaoTao)
        {
            if (id != banDaoTao.IdBanDaoTao)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(banDaoTao);
            }

            using (var httpClient = new HttpClient())
            {
                string apiUrl = $"https://localhost:7296/api/BanDaoTao/{id}";
                var content = new StringContent(JsonConvert.SerializeObject(banDaoTao), Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync(apiUrl, content); // <-- Sửa ở đây

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi API: {error}");

                return View(banDaoTao); // <-- Không Redirect nếu có lỗi
            }
        }



        // GET: BanDaoTaos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BanDaoTao banDaoTao = new BanDaoTao();
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.GetAsync($"{apiUrl}/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string api = await response.Content.ReadAsStringAsync();
                        banDaoTao = JsonConvert.DeserializeObject<BanDaoTao>(api);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            return View(banDaoTao);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.DeleteAsync($"{apiUrl}/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            // Gọi API lấy bản ghi hiện tại
            BanDaoTao banDaoTao = null;
            using (var httpClient = new HttpClient())
            {
                var res = await httpClient.GetAsync($"{apiUrl}/{id}");
                if (res.IsSuccessStatusCode)
                {
                    string json = await res.Content.ReadAsStringAsync();
                    banDaoTao = JsonConvert.DeserializeObject<BanDaoTao>(json);
                }
                else return NotFound();
            }

            // Đảo trạng thái
            banDaoTao.TrangThai = !banDaoTao.TrangThai;
            banDaoTao.NgayCapNhat = DateTime.Now;

            // Gửi PUT lại API
            using (var httpClient = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(banDaoTao);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{apiUrl}/{id}", content);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }

            return RedirectToAction("Index");
        }

    }
}