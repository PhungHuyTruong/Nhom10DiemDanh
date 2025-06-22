using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeHoachNhomXuongsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public KeHoachNhomXuongsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/KeHoachNhomXuongs
        // GET: api/KeHoachNhomXuongs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.KeHoachNhomXuongs
                .Include(k => k.KeHoach)
                .Include(k => k.NhomXuong).ThenInclude(nx => nx.PhuTrachXuong).Include(k => k.KHNXCaHocs)
                .ToListAsync();

            // Set TenNhomXuong thủ công
            foreach (var item in data)
            {
                item.TenNhomXuong = item.NhomXuong?.TenNhomXuong ?? "";
                item.TenPhuTrachXuong = item.NhomXuong?.PhuTrachXuong?.TenNhanVien ?? "";
                item.SoBuoi = item.KHNXCaHocs?.Count ?? 0;
            }
            data = data.OrderByDescending(x => x.SoBuoi).ToList();

            return Ok(data);
        }

        // GET: api/KeHoachNhomXuongs/ByKeHoach/{idKeHoach}
        [HttpGet("ByKeHoach/{idKeHoach}")]
        public async Task<IActionResult> GetByKeHoach(Guid idKeHoach)
        {
            var data = await _context.KeHoachNhomXuongs
                .Where(k => k.IdKeHoach == idKeHoach)
                .Include(k => k.KeHoach)
                .Include(k => k.NhomXuong).ThenInclude(nx => nx.PhuTrachXuong).Include(k => k.KHNXCaHocs)
                .ToListAsync();

            foreach (var item in data)
            {
                item.TenNhomXuong = item.NhomXuong?.TenNhomXuong ?? "";
                item.TenPhuTrachXuong = item.NhomXuong?.PhuTrachXuong?.TenNhanVien ?? "";
                item.SoBuoi = item.KHNXCaHocs?.Count ?? 0;
            }

            foreach (var item in data)
            {
                item.SoSinhVien = await _context.SinhViens.CountAsync(sv => sv.IdNhomXuong == item.IdNhomXuong);
            }


            return Ok(data);
        }

        // GET: api/KeHoachNhomXuongs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _context.KeHoachNhomXuongs
                .Include(k => k.KeHoach)
                .Include(k => k.NhomXuong)
                .FirstOrDefaultAsync(k => k.IdKHNX == id);

            if (item == null) return NotFound();

            item.TenNhomXuong = item.NhomXuong?.TenNhomXuong ?? "";

            return Ok(item);
        }


        // POST: api/KeHoachNhomXuongs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KeHoachNhomXuong model)
        {
            // Kiểm tra IdKeHoach và IdNhomXuong có tồn tại không
            var keHoachExists = await _context.KeHoachs.AnyAsync(kh => kh.IdKeHoach == model.IdKeHoach);
            var nhomXuong = await _context.NhomXuongs
                .Include(nx => nx.SinhViens)
                .FirstOrDefaultAsync(nx => nx.IdNhomXuong == model.IdNhomXuong);

            if (!keHoachExists || nhomXuong == null)
            {
                return BadRequest("Kế hoạch hoặc Nhóm xưởng không hợp lệ.");
            }

            // ✅ Gán lại số sinh viên dựa vào nhóm xưởng
            model.SoSinhVien = nhomXuong.SinhViens?.Count ?? 0;

            _context.KeHoachNhomXuongs.Add(model);
            await _context.SaveChangesAsync();

            // Load lại để trả về với Include
            var createdItem = await _context.KeHoachNhomXuongs
                .Include(k => k.NhomXuong)
                .Include(k => k.KeHoach)
                .FirstOrDefaultAsync(k => k.IdKHNX == model.IdKHNX);

            return CreatedAtAction(nameof(GetById), new { id = model.IdKHNX }, createdItem);
        }


        // PUT: api/KeHoachNhomXuongs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KeHoachNhomXuong model)
        {
            if (id != model.IdKHNX) return BadRequest();

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/KeHoachNhomXuongs/{id}
        // Bổ sung: Thêm action Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.KeHoachNhomXuongs.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.KeHoachNhomXuongs.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/KeHoachNhomXuongs/NhomXuongs/ByKeHoach/{idKeHoach}
        // Cải thiện: Sửa lại logic để lấy các nhóm xưởng CHƯA được phân công
        [HttpGet("NhomXuongs/ByKeHoach/{idKeHoach}")]
        public async Task<IActionResult> GetNhomXuongsByKeHoach(Guid idKeHoach)
        {
            // Lấy danh sách ID của các nhóm xưởng đã được gán cho kế hoạch này
            var assignedNhomXuongIds = await _context.KeHoachNhomXuongs
                .Where(x => x.IdKeHoach == idKeHoach)
                .Select(x => x.IdNhomXuong)
                .ToListAsync();

            // Lấy tất cả nhóm xưởng mà ID không nằm trong danh sách đã được gán
            var unassignedNhomXuongs = await _context.NhomXuongs
                .Where(nx => !assignedNhomXuongIds.Contains(nx.IdNhomXuong))
                .ToListAsync();

            return Ok(unassignedNhomXuongs);
        }
    }
}