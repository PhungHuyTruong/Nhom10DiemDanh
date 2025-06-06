using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CapDoDuAnController : Controller
    {

        private readonly ModuleDiemDanhDbContext _diemDanhDbContext;
        public CapDoDuAnController(ModuleDiemDanhDbContext moduleDiemDanhDbContext)
        {
            _diemDanhDbContext = moduleDiemDanhDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _diemDanhDbContext.CapDoDuAns.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var capDo = await _diemDanhDbContext.CapDoDuAns.FindAsync(id);
            if (capDo == null) return NotFound();
            return Ok(capDo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CapDoDuAn capDoDuAn)
        {
            if (!ModelState.IsValid)
                return View(capDoDuAn);

            capDoDuAn.IdCDDA = Guid.NewGuid();
            capDoDuAn.NgayTao = DateTime.Now;
            capDoDuAn.TrangThai = true;

            _diemDanhDbContext.CapDoDuAns.Add(capDoDuAn);
            await _diemDanhDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CapDoDuAn updatedCapDo)
        {
            if (!ModelState.IsValid)
                return View(updatedCapDo);
            var capDo = await _diemDanhDbContext.CapDoDuAns.FindAsync(id);
            if (capDo == null) return NotFound();

            capDo.TenCapDoDuAn = updatedCapDo.TenCapDoDuAn;
            capDo.MaCapDoDuAn = updatedCapDo.MaCapDoDuAn;
            capDo.MoTa = updatedCapDo.MoTa;
            capDo.TrangThai = updatedCapDo.TrangThai;
            capDo.NgayCapNhat = DateTime.Now;

            _diemDanhDbContext.CapDoDuAns.Update(capDo);

            await _diemDanhDbContext.SaveChangesAsync();

            return Ok(capDo);
        }

        [HttpPost("doi-trang-thai/{id}")]
        public async Task<IActionResult> DoiTrangThai(Guid id)
        {
            var cap = await _diemDanhDbContext.CapDoDuAns.FindAsync(id);
            if (cap == null) return NotFound();

            cap.TrangThai = !cap.TrangThai;
            cap.NgayCapNhat = DateTime.Now;

            _diemDanhDbContext.CapDoDuAns.Update(cap);
            await _diemDanhDbContext.SaveChangesAsync();

            return Ok(cap);
        }
    }
}
