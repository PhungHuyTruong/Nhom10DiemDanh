using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class LichHocsController : Controller
    {
        private readonly ModuleDiemDanhDbContext _context;

        public LichHocsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: LichHocs
        public async Task<IActionResult> Index()
        {
            var moduleDiemDanhDbContext = _context.LichHocs.Include(l => l.DuAn).Include(l => l.KHNXCaHoc).Include(l => l.NhomXuong);
            return View(await moduleDiemDanhDbContext.ToListAsync());
        }

        // GET: LichHocs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichHoc = await _context.LichHocs
                .Include(l => l.DuAn)
                .Include(l => l.KHNXCaHoc)
                .Include(l => l.NhomXuong)
                .FirstOrDefaultAsync(m => m.IDLichHoc == id);
            if (lichHoc == null)
            {
                return NotFound();
            }

            return View(lichHoc);
        }

        // GET: LichHocs/Create
        public IActionResult Create()
        {
            ViewData["IDHocKy"] = new SelectList(_context.DuAns, "IdDuAn", "MoTa");
            ViewData["IdNXCH"] = new SelectList(_context.KHNXCaHocs, "IdNXCH", "Buoi");
            ViewData["IdNhomXuong"] = new SelectList(_context.NhomXuongs, "IdNhomXuong", "MoTa");
            return View();
        }

        // POST: LichHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDLichHoc,IdNXCH,IdNhomXuong,IDHocKy,TrangThai")] LichHoc lichHoc)
        {
            if (ModelState.IsValid)
            {
                lichHoc.IDLichHoc = Guid.NewGuid();
                _context.Add(lichHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDHocKy"] = new SelectList(_context.DuAns, "IdDuAn", "MoTa", lichHoc.IDHocKy);
            ViewData["IdNXCH"] = new SelectList(_context.KHNXCaHocs, "IdNXCH", "Buoi", lichHoc.IdNXCH);
            ViewData["IdNhomXuong"] = new SelectList(_context.NhomXuongs, "IdNhomXuong", "MoTa", lichHoc.IdNhomXuong);
            return View(lichHoc);
        }

        // GET: LichHocs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichHoc = await _context.LichHocs.FindAsync(id);
            if (lichHoc == null)
            {
                return NotFound();
            }
            ViewData["IDHocKy"] = new SelectList(_context.DuAns, "IdDuAn", "MoTa", lichHoc.IDHocKy);
            ViewData["IdNXCH"] = new SelectList(_context.KHNXCaHocs, "IdNXCH", "Buoi", lichHoc.IdNXCH);
            ViewData["IdNhomXuong"] = new SelectList(_context.NhomXuongs, "IdNhomXuong", "MoTa", lichHoc.IdNhomXuong);
            return View(lichHoc);
        }

        // POST: LichHocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IDLichHoc,IdNXCH,IdNhomXuong,IDHocKy,TrangThai")] LichHoc lichHoc)
        {
            if (id != lichHoc.IDLichHoc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichHocExists(lichHoc.IDLichHoc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDHocKy"] = new SelectList(_context.DuAns, "IdDuAn", "MoTa", lichHoc.IDHocKy);
            ViewData["IdNXCH"] = new SelectList(_context.KHNXCaHocs, "IdNXCH", "Buoi", lichHoc.IdNXCH);
            ViewData["IdNhomXuong"] = new SelectList(_context.NhomXuongs, "IdNhomXuong", "MoTa", lichHoc.IdNhomXuong);
            return View(lichHoc);
        }

        // GET: LichHocs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichHoc = await _context.LichHocs
                .Include(l => l.DuAn)
                .Include(l => l.KHNXCaHoc)
                .Include(l => l.NhomXuong)
                .FirstOrDefaultAsync(m => m.IDLichHoc == id);
            if (lichHoc == null)
            {
                return NotFound();
            }

            return View(lichHoc);
        }

        // POST: LichHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lichHoc = await _context.LichHocs.FindAsync(id);
            if (lichHoc != null)
            {
                _context.LichHocs.Remove(lichHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichHocExists(Guid id)
        {
            return _context.LichHocs.Any(e => e.IDLichHoc == id);
        }
    }
}
