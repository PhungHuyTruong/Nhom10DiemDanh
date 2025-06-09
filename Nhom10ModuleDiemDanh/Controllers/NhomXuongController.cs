using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class NhomXuongsController : Controller
    {
        private readonly string apiUrl = "https://localhost:7296/api/NhomXuongs";

        public async Task<IActionResult> Index(int page = 1, string search = "", int? trangThai = null)
        {
            int pageSize = 5;

            var pagedData = new
            {
                data = new List<NhomXuong>(),
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
                var url = $"{apiUrl}/paging?page={page}&pageSize={pageSize}&search={search}&trangThai={(trangThai.HasValue ? trangThai.ToString() : "")}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    pagedData = JsonConvert.DeserializeAnonymousType(json, pagedData);
                }
            }

            ViewBag.Pagination = pagedData.pagination;
            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;
            return View(pagedData.data);
        }

        public IActionResult Create()
        {
            ViewBag.GiangViens = new SelectList(new List<PhuTrachXuong>
                {
                 new PhuTrachXuong { IdNhanVien = Guid.NewGuid(), TenNhanVien = "Nguyễn Văn A" },
                 new PhuTrachXuong { IdNhanVien = Guid.NewGuid(), TenNhanVien = "Trần Thị B" },
                }, "Id", "TenNhanVien");

            ViewBag.DuAns = new SelectList(new List<DuAn>
                {
                 new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "Dự án A" },
                    new DuAn { IdDuAn = Guid.NewGuid(), TenDuAn = "Dự án B" },
                }, "Id", "TenDuAn");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhomXuong nhom)
        {
            if (!ModelState.IsValid)
                return View(nhom);

            using (HttpClient client = new HttpClient())
            {
                nhom.IdNhomXuong = Guid.NewGuid();
                nhom.NgayTao = DateTime.Now;
                nhom.TrangThai = 1;

                var content = new StringContent(JsonConvert.SerializeObject(nhom), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            return View(nhom);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            NhomXuong model = null;
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<NhomXuong>(json);
                }
            }

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, NhomXuong model)
        {
            if (id != model.IdNhomXuong || !ModelState.IsValid)
                return View(model);

            model.NgayCapNhat = DateTime.Now;

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{apiUrl}/{id}", content);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PutAsync($"{apiUrl}/toggle-status/{id}", null);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}