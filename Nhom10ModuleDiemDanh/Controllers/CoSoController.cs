using Microsoft.AspNetCore.Mvc;
using Nhom10ModuleDiemDanh.Models;
using Nhom10ModuleDiemDanh.Services;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class CoSoController : Controller
    {
        private readonly ICoSoService _coSoService;

        public CoSoController(ICoSoService coSoService)
        {
            _coSoService = coSoService;
        }

        public async Task<IActionResult> Index(string tenCoSo, string trangThai)
        {
            var coSoList = await _coSoService.GetCoSosAsync(tenCoSo, trangThai);
            ViewData["tenCoSo"] = tenCoSo;
            ViewData["trangThai"] = trangThai ?? "Tất cả trạng thái";
            return View(coSoList);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var coSo = await _coSoService.GetCoSoAsync(id);
            if (coSo == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", coSo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new CoSoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoSoViewModel coSoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    coSoViewModel.IdCoSo = Guid.NewGuid();
                    coSoViewModel.DiaChi ??= string.Empty;
                    coSoViewModel.SDT ??= string.Empty;
                    coSoViewModel.Email ??= string.Empty;

                    await _coSoService.CreateCoSoAsync(coSoViewModel);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Không thể thêm cơ sở: " + ex.Message });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var coSo = await _coSoService.GetCoSoAsync(id);
            if (coSo == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", coSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CoSoViewModel coSoViewModel)
        {
            if (id != coSoViewModel.IdCoSo)
            {
                return Json(new { success = false, message = "ID không khớp." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    coSoViewModel.DiaChi ??= string.Empty;
                    coSoViewModel.SDT ??= string.Empty;
                    coSoViewModel.Email ??= string.Empty;

                    await _coSoService.UpdateCoSoAsync(id, coSoViewModel);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Không thể sửa cơ sở: " + ex.Message });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coSo = await _coSoService.GetCoSoAsync(id.Value);
            if (coSo == null)
            {
                return NotFound();
            }

            return View(coSo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _coSoService.DeleteCoSoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            await _coSoService.ToggleStatusAsync(id);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> CaHoc(Guid id)
        {
            var coSo = await _coSoService.GetCoSoAsync(id);
            if (coSo == null) return NotFound();
            // Logic hiển thị danh sách ca học (có thể là view hoặc JSON)
            return View("_CaHoc", coSo); // Tạo partial view _CaHoc.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> Ip(Guid id)
        {
            var coSo = await _coSoService.GetCoSoAsync(id);
            if (coSo == null) return NotFound();
            // Logic hiển thị thông tin IP
            return View("_Ip", coSo); // Tạo partial view _Ip.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> DiaDiem(Guid id)
        {
            var coSo = await _coSoService.GetCoSoAsync(id);
            if (coSo == null) return NotFound();
            // Logic hiển thị địa điểm
            return View("_DiaDiem", coSo); // Tạo partial view _DiaDiem.cshtml
        }

    }
}