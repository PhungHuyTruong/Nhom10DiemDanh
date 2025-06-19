using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportHistoryController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public ImportHistoryController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/ImportHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportHistory>>> GetImportHistory()
        {
            var history = await _context.ImportHistory
                .OrderByDescending(h => h.ImportDate)
                .ToListAsync();

            return Ok(new { success = true, message = "Lấy lịch sử import thành công", data = history });
        }

        // GET: api/ImportHistory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImportHistory>> GetImportHistory(Guid id)
        {
            var history = await _context.ImportHistory.FindAsync(id);
            if (history == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lịch sử import" });
            }
            return Ok(new { success = true, message = "Lấy lịch sử import thành công", data = history });
        }

        // GET: api/ImportHistory/type/KHNXCaHoc
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<ImportHistory>>> GetImportHistoryByType(string type)
        {
            var history = await _context.ImportHistory
                .Where(h => h.Type == type)
                .OrderByDescending(h => h.ImportDate)
                .ToListAsync();

            return Ok(new { success = true, message = "Lấy lịch sử import theo loại thành công", data = history });
        }
    }
}