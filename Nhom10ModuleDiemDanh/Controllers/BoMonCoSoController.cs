using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom10ModuleDiemDanh.Models;
using Nhom10ModuleDiemDanh.Services;
using System;
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

        [HttpGet]
        public async Task<IActionResult> Index(string tenBoMon, Guid? idCoSo, string trangThai)
        {
            var boMonCoSos = await _service.GetAllAsync(tenBoMon, idCoSo);

            if (boMonCoSos == null)
            {
                boMonCoSos = new List<BoMonCoSoViewModel>();
            }

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
            {
                boMonCoSos = boMonCoSos.Where(b => b.TrangThai == trangThai).ToList();
            }

            var coSos = await _service.GetCoSosAsync();
            ViewData["CoSos"] = coSos ?? new List<CoSo>();
            ViewData["tenBoMon"] = tenBoMon;
            ViewData["idCoSo"] = idCoSo?.ToString();
            ViewData["trangThai"] = trangThai ?? "Tất cả trạng thái";
            return View(boMonCoSos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var boMons = await _service.GetBoMonsAsync();
            var coSos = await _service.GetCoSosAsync();

            ViewData["BoMons"] = new SelectList(boMons, "IDBoMon", "TenBoMon");
            ViewData["CoSos"] = new SelectList(coSos, "IdCoSo", "TenCoSo");
            return PartialView("_CreateBoMonCoSo", new BoMonCoSoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BoMonCoSoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.CreateAsync(model);
                if (result != null)
                    return Json(new { success = true, message = "Thêm thành công." });
            }
            return Json(new { success = false, message = "Thêm thất bại." });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            var boMons = await _service.GetBoMonsAsync();
            var coSos = await _service.GetCoSosAsync();

            ViewData["BoMons"] = boMons;
            ViewData["CoSos"] = coSos;
            return PartialView("_EditBoMonCoSo", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BoMonCoSoViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(model.IdBoMonCoSo, model);
                return Json(new { success = true, message = "Sửa thành công." });
            }
            return Json(new { success = false, message = "Sửa thất bại." });
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
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var boMonCoSo = await _service.GetByIdAsync(id);
            if (boMonCoSo == null)
                return Json(new { success = false, message = "Không tìm thấy bộ môn cơ sở." });

            boMonCoSo.TrangThai = boMonCoSo.TrangThai == "Hoạt động" ? "Tắt" : "Hoạt động";
            await _service.UpdateAsync(id, boMonCoSo);
            return Json(new { success = true });
        }
    }
}