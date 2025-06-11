using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class IPController : Controller
    {
        private readonly ModuleDiemDanhDbContext _context;

        public IPController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var data = await _context.IPs
                .Where(ip => ip.IdCoSo == id)
                .Select(ip => new IP
                {
                    IdIP = ip.IdIP,
                    KieuIP = ip.KieuIP,
                    IP_DaiIP = ip.IP_DaiIP,
                    NgayTao = ip.NgayTao,
                    NgayCapNhat = ip.NgayCapNhat,
                    TrangThai = ip.TrangThai,
                    IdCoSo = ip.IdCoSo
                })
                .ToListAsync();

            ViewBag.IdCoSo = id; // để hiển thị trong View nếu cần
            return View(data ?? new List<IP>());
        }

        public IActionResult Create(Guid? idCoSo)
        {
            ViewBag.IdCoSo = idCoSo;
            return View(new IP());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IP model)
        {
            if (string.IsNullOrWhiteSpace(model.KieuIP) || string.IsNullOrWhiteSpace(model.IP_DaiIP))
            {
                ModelState.AddModelError(string.Empty, "KieuIP and IP_DaiIP are required.");
                return View(model);
            }

            var ip = new IP
            {
                IdIP = Guid.NewGuid(),
                KieuIP = model.KieuIP,
                IP_DaiIP = model.IP_DaiIP,
                NgayTao = DateTime.Now,
                TrangThai = model.TrangThai,
                IdCoSo = model.IdCoSo
            };

            _context.IPs.Add(ip);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = model.IdCoSo });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null)
            {
                return NotFound();
            }

            return View(ip);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IP model)
        {
            if (id != model.IdIP)
            {
                return BadRequest();
            }

            var ip = await _context.IPs.FindAsync(id);
            if (ip == null)
            {
                return NotFound();
            }

            ip.KieuIP = model.KieuIP;
            ip.IP_DaiIP = model.IP_DaiIP;
            ip.NgayCapNhat = DateTime.Now;
            ip.TrangThai = model.TrangThai;
            ip.IdCoSo = model.IdCoSo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.IPs.AnyAsync(e => e.IdIP == id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction("Index", new { id = model.IdCoSo });
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null)
            {
                return NotFound();
            }

            return View(ip);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ip = await _context.IPs.FindAsync(id);
            if (ip == null)
            {
                return NotFound();
            }

            _context.IPs.Remove(ip);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = ip.IdCoSo });
        }
    }
}