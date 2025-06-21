using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom10ModuleDiemDanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nhom10ModuleDiemDanh.Controllers
{
    public class ImportHistoryController : Controller
    {
        private readonly ModuleDiemDanhDbContext _context;

        public ImportHistoryController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: ImportHistory
        public async Task<IActionResult> Index(Guid idKHNX)
        {
            var history = await _context.ImportHistory
                .OrderByDescending(h => h.ImportDate)
                .ToListAsync();

            return View(history);
        }

        // GET: ImportHistory/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importHistory = await _context.ImportHistory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (importHistory == null)
            {
                return NotFound();
            }

            return View(importHistory);
        }

        // GET: ImportHistory/ByType/KHNXCaHoc
        public async Task<IActionResult> ByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return RedirectToAction(nameof(Index));
            }

            var history = await _context.ImportHistory
                .Where(h => h.Type == type)
                .OrderByDescending(h => h.ImportDate)
                .ToListAsync();

            ViewBag.Type = type;
            return View("Index", history);
        }
    }
}