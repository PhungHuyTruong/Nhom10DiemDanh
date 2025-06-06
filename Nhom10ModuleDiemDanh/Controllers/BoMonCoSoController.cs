using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom10ModuleDiemDanh.Models;
using Nhom10ModuleDiemDanh.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class BoMonCoSoController : Controller
    {
        private readonly IBoMonCoSoService _service;

        public BoMonCoSoController(IBoMonCoSoService service)
        {
            _service = service;
        }

        [HttpGet("/BoMonCoSo/Index")]
        public async Task<IActionResult> Index(string tenBoMon = null, Guid? idCoSo = null, string trangThai = null, Guid? idBoMon = null)
        {
            var boMonCoSos = await _service.GetAllAsync(tenBoMon, idCoSo) ?? new List<BoMonCoSoViewModel>();

            if (idBoMon.HasValue)
            {
                boMonCoSos = boMonCoSos.Where(b => b.IdBoMon == idBoMon).ToList();
            }

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
            {
                bool isActive = trangThai == "Hoạt động";
                boMonCoSos = boMonCoSos.Where(b => b.TrangThai == isActive).ToList();
            }

            var coSos = await _service.GetCoSosAsync() ?? new List<CoSo>();
            var boMons = await _service.GetBoMonsAsync() ?? new List<QuanLyBoMon>();

            ViewData["CoSos"] = new SelectList(
                new List<CoSo> { new CoSo { IdCoSo = Guid.Empty, TenCoSo = "Tất cả cơ sở" } }.Concat(coSos),
                "IdCoSo",
                "TenCoSo",
                idCoSo
            );

            ViewData["BoMons"] = new SelectList(boMons, "IDBoMon", "TenBoMon");

            // Chỉ sử dụng tenBoMon nếu người dùng bấm "Lọc" (từ form)
            bool isFilterApplied = Request.Method == "GET" && !string.IsNullOrEmpty(Request.Query["tenBoMon"]) && Request.Query.ContainsKey("tenBoMon");
            ViewData["tenBoMon"] = isFilterApplied ? tenBoMon : null;
            ViewData["idCoSo"] = idCoSo?.ToString();
            ViewData["trangThai"] = trangThai ?? "Tất cả trạng thái";
            ViewData["idBoMon"] = idBoMon;

            return View(boMonCoSos);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? idBoMon = null, string tenBoMon = "bm2")
        {
            var model = new BoMonCoSoViewModel
            { 
                IdBoMon = idBoMon,
                TenBoMon = tenBoMon,
                TrangThai = true,
                TenCoSo = "Tất cả cơ sở"
            };

            var coSos = await _service.GetCoSosAsync() ?? new List<CoSo>();
            ViewData["CoSos"] = new SelectList(
                new List<CoSo> { new CoSo { IdCoSo = Guid.Empty, TenCoSo = "Tất cả cơ sở" } }.Concat(coSos),
                "IdCoSo",
                "TenCoSo"
            );

            return PartialView("_CreateBoMonCoSo", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoMonCoSoViewModel model)
        {
            if (model.IdCoSo == Guid.Empty)
            {
                model.IdCoSo = null;
                model.TenCoSo = "Tất cả cơ sở";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.TrangThai = true;
                    model.NgayTao = DateTime.Now;
                    var result = await _service.CreateAsync(model);
                    if (result != null)
                        return Json(new { success = true, message = "Thêm thành công." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Thêm thất bại: {ex.Message}" });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            var boMons = await _service.GetBoMonsAsync() ?? new List<QuanLyBoMon>();
            var coSos = await _service.GetCoSosAsync() ?? new List<CoSo>();

            ViewData["BoMons"] = boMons;
            ViewData["CoSos"] = new SelectList(
                new List<CoSo> { new CoSo { IdCoSo = Guid.Empty, TenCoSo = "Tất cả cơ sở" } }.Concat(coSos),
                "IdCoSo",
                "TenCoSo",
                model.IdCoSo
            );

            return PartialView("_EditBoMonCoSo", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BoMonCoSoViewModel model)
        {
            if (model.IdCoSo == Guid.Empty)
            {
                model.IdCoSo = null;
                model.TenCoSo = "Tất cả cơ sở";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IdBoMon.HasValue)
                    {
                        var boMon = (await _service.GetBoMonsAsync()).FirstOrDefault(b => b.IDBoMon == model.IdBoMon);
                        model.TenBoMon = boMon?.TenBoMon ?? "Không xác định";
                    }
                    if (model.IdCoSo.HasValue)
                    {
                        var coSo = (await _service.GetCoSosAsync()).FirstOrDefault(c => c.IdCoSo == model.IdCoSo);
                        model.TenCoSo = coSo?.TenCoSo ?? "Không xác định";
                    }
                    else if (!model.IdCoSo.HasValue)
                    {
                        model.TenCoSo = "Tất cả cơ sở";
                    }


                    model.NgayCapNhat = DateTime.Now;
                    await _service.UpdateAsync(model.IdBoMonCoSo, model);
                    return Json(new { success = true, message = "Sửa thành công." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Sửa thất bại: {ex.Message}" });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_DetailsBoMonCoSo", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Xóa thất bại: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                await _service.ToggleStatusAsync(id);
                var boMonCoSo = await _service.GetByIdAsync(id);
                return Json(new { success = true, trangThai = boMonCoSo.TrangThai ? "Hoạt động" : "Tắt" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Không thể cập nhật trạng thái: {ex.Message}" });
            }
        }
    }
}