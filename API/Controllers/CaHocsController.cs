using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CaHocsController : ControllerBase
{
    private readonly ModuleDiemDanhDbContext _context;

    public CaHocsController(ModuleDiemDanhDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCaHocs()
    {
        var caHocs = await _context.CaHocs.ToListAsync();
        if (caHocs == null || !caHocs.Any())
            return NotFound(new { success = false, message = "Không có ca học nào." });

        return Ok(new { success = true, message = "Lấy danh sách ca học thành công", data = caHocs });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCaHoc(Guid id)
    {
        var caHoc = await _context.CaHocs.FindAsync(id);
        if (caHoc == null)
            return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });

        return Ok(new { success = true, message = "Lấy ca học thành công", data = caHoc });
    }

    [HttpPost]
    public async Task<IActionResult> PostCaHoc(CaHoc caHoc)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

        if (caHoc.ThoiGianKetThuc <= caHoc.ThoiGianBatDau)
            return BadRequest(new { success = false, message = "Thời gian kết thúc phải sau thời gian bắt đầu." });

        caHoc.IdCaHoc = Guid.NewGuid();
        caHoc.NgayCapNhat = DateTime.Now;

        _context.CaHocs.Add(caHoc);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCaHoc), new { id = caHoc.IdCaHoc }, new
        {
            success = true,
            message = "Thêm ca học thành công",
            data = caHoc
        });
    }

    [HttpPut]
    public async Task<IActionResult> PutCaHoc(Guid id, CaHoc caHoc)
    {
        if (id != caHoc.IdCaHoc)
            return BadRequest(new { success = false, message = "ID ca học không khớp" });

        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

        if (caHoc.ThoiGianKetThuc <= caHoc.ThoiGianBatDau)
            return BadRequest(new { success = false, message = "Thời gian kết thúc phải sau thời gian bắt đầu." });

        var existing = await _context.CaHocs.FindAsync(id);
        if (existing == null)
            return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });

        existing.TenCaHoc = caHoc.TenCaHoc;
        existing.ThoiGianBatDau = caHoc.ThoiGianBatDau;
        existing.ThoiGianKetThuc = caHoc.ThoiGianKetThuc;
        existing.NgayCapNhat = DateTime.Now;
        existing.TrangThai = caHoc.TrangThai;

        _context.CaHocs.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Cập nhật ca học thành công", data = existing });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCaHoc(Guid id)
    {
        var caHoc = await _context.CaHocs.FindAsync(id);
        if (caHoc == null)
            return NotFound(new { success = false, message = $"Không tìm thấy ca học với ID {id}" });

        _context.CaHocs.Remove(caHoc);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Xóa ca học thành công", data = caHoc });
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter(string? tenCaHoc, int? trangThai, int page = 1, int pageSize = 5)
    {
        var query = _context.CaHocs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(tenCaHoc))
            query = query.Where(x => x.TenCaHoc.Contains(tenCaHoc));

        if (trangThai.HasValue)
            query = query.Where(x => x.TrangThai == trangThai.Value);

        query = query.OrderBy(x => x.ThoiGianBatDau);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            success = true,
            message = "Lọc thành công",
            data,
            pagination = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalItems,
                totalPages
            }
        });
    }
}