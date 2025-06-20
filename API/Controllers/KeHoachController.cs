using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeHoachController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public KeHoachController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetKeHoachs(string tuKhoa = "", string trangThai = "", string idBoMon = "", string idCapDoDuAn = "", string idHocKy = "", string namHoc = "")
        {
            var query = from k in _context.KeHoachs
                        join d in _context.DuAns on k.IdDuAn equals d.IdDuAn into duAnGroup
                        from d in duAnGroup.DefaultIfEmpty()
                        join b in _context.QuanLyBoMons on d.IdBoMon equals b.IDBoMon into boMonGroup
                        from b in boMonGroup.DefaultIfEmpty()
                        join c in _context.CapDoDuAns on d.IdCDDA equals c.IdCDDA into capDoGroup
                        from c in capDoGroup.DefaultIfEmpty()
                        join h in _context.hocKy on d.IdHocKy equals h.IdHocKy into hocKyGroup
                        from h in hocKyGroup.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(tuKhoa) || k.TenKeHoach.Contains(tuKhoa)) &&
                             (string.IsNullOrEmpty(trangThai) || trangThai == "Tất cả trạng thái" ||
                            (trangThai == "sapdienra" && k.ThoiGianBatDau > DateTime.Now) ||
                            (trangThai == "dangdienra" && k.ThoiGianBatDau <= DateTime.Now && k.ThoiGianKetThuc >= DateTime.Now) ||
                            (trangThai == "dadienra" && k.ThoiGianKetThuc < DateTime.Now)) &&
                              (string.IsNullOrEmpty(idBoMon) || (d != null && d.IdBoMon.ToString() == idBoMon)) &&
                              (string.IsNullOrEmpty(idCapDoDuAn) || (d != null && d.IdCDDA.ToString() == idCapDoDuAn)) &&
                              (string.IsNullOrEmpty(idHocKy) || (d != null && d.IdHocKy.ToString() == idHocKy)) &&
                              (string.IsNullOrEmpty(namHoc) || (d != null && d.NgayTao.Year.ToString() == namHoc))
                        select new KeHoachInputModel
                        {
                            IdKeHoach = k.IdKeHoach,
                            TenKeHoach = k.TenKeHoach,
                            IdDuAn = k.IdDuAn,
                            NoiDung = k.NoiDung,
                            ThoiGianBatDau = k.ThoiGianBatDau,
                            ThoiGianKetThuc = k.ThoiGianKetThuc,
                            TrangThai = k.TrangThai,
                            NgayTao = k.NgayTao,
                            NgayCapNhat = k.NgayCapNhat,
                            TenDuAn = d != null ? d.TenDuAn : null,
                            TenBoMon = b != null ? b.TenBoMon : null,
                            TenCapDoDuAn = c != null ? c.TenCapDoDuAn : null
                        };

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKeHoach(Guid id)
        {
            var keHoach = await _context.KeHoachs
                .Include(k => k.DuAn)
                .ThenInclude(d => d.QuanLyBoMon)
                .Include(k => k.DuAn)
                .ThenInclude(d => d.CapDoDuAn)
                .FirstOrDefaultAsync(k => k.IdKeHoach == id);

            if (keHoach == null)
            {
                return NotFound();
            }

            var keHoachViewModel = new KeHoachInputModel
            {
                IdKeHoach = keHoach.IdKeHoach,
                TenKeHoach = keHoach.TenKeHoach,
                IdDuAn = keHoach.IdDuAn,
                NoiDung = keHoach.NoiDung,
                ThoiGianBatDau = keHoach.ThoiGianBatDau,
                ThoiGianKetThuc = keHoach.ThoiGianKetThuc,
                TrangThai = keHoach.TrangThai,
                NgayTao = keHoach.NgayTao,
                NgayCapNhat = keHoach.NgayCapNhat,
                TenDuAn = keHoach.DuAn?.TenDuAn,
                TenBoMon = keHoach.DuAn?.QuanLyBoMon?.TenBoMon,
                TenCapDoDuAn = keHoach.DuAn?.CapDoDuAn?.TenCapDoDuAn
            };

            return Ok(keHoachViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateKeHoach([FromForm] KeHoachInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var duAn = await _context.DuAns
                .Include(d => d.QuanLyBoMon)
                .Include(d => d.CapDoDuAn)
                .FirstOrDefaultAsync(d => d.IdDuAn == model.IdDuAn);

            if (duAn == null)
            {
                return BadRequest(new { message = "Không thể tạo kế hoạch: Dự án không tồn tại." });
            }
            //if (model.ThoiGianBatDau >= model.ThoiGianKetThuc)
            //{
            //    return BadRequest(new { message = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu." });
            //}

            var keHoach = new KeHoach
            {
                IdKeHoach = Guid.NewGuid(),
                TenKeHoach = model.TenKeHoach,
                IdDuAn = model.IdDuAn,
                NoiDung = model.NoiDung,
                ThoiGianBatDau = model.ThoiGianBatDau,
                ThoiGianKetThuc = model.ThoiGianKetThuc,
                TrangThai = model.TrangThai,
                NgayTao = DateTime.Now,

                TenDuAn = duAn.TenDuAn,
                TenBoMon = duAn.QuanLyBoMon?.TenBoMon,
                TenCapDoDuAn = duAn.CapDoDuAn?.TenCapDoDuAn
            };

            _context.KeHoachs.Add(keHoach);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKeHoach(Guid id, [FromForm] KeHoachInputModel model)
        {
            if (id != model.IdKeHoach)
            {
                return BadRequest();
            }

            if (model.ThoiGianBatDau >= model.ThoiGianKetThuc)
            {
                return BadRequest(new { message = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu." });
            }

            var keHoach = await _context.KeHoachs.FindAsync(id);
            if (keHoach == null)
            {
                return NotFound();
            }

            var duAn = await _context.DuAns
                .Include(d => d.QuanLyBoMon)
                .Include(d => d.CapDoDuAn)
                .FirstOrDefaultAsync(d => d.IdDuAn == model.IdDuAn);

            if (duAn == null)
            {
                return BadRequest(new { message = "Không thể cập nhật kế hoạch: Dự án không tồn tại." });
            }

            keHoach.TenKeHoach = model.TenKeHoach;
            keHoach.IdDuAn = model.IdDuAn;
            keHoach.NoiDung = model.NoiDung;
            keHoach.ThoiGianBatDau = model.ThoiGianBatDau;
            keHoach.ThoiGianKetThuc = model.ThoiGianKetThuc;
            keHoach.TrangThai = model.TrangThai;
            keHoach.NgayCapNhat = DateTime.Now;

            // Cập nhật lại thông tin từ DuAn giống Create
            keHoach.TenDuAn = duAn.TenDuAn;
            keHoach.TenBoMon = duAn.QuanLyBoMon?.TenBoMon;
            keHoach.TenCapDoDuAn = duAn.CapDoDuAn?.TenCapDoDuAn;

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKeHoach(Guid id)
        {
            var keHoach = await _context.KeHoachs.FindAsync(id);
            if (keHoach == null)
            {
                return NotFound();
            }

            _context.KeHoachs.Remove(keHoach);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        //[HttpPost("ToggleStatus/{id}")]
        //public async Task<IActionResult> ToggleStatus(Guid id)
        //{
        //    var keHoach = await _context.KeHoachs.FindAsync(id);
        //    if (keHoach == null)
        //    {
        //        return NotFound();
        //    }

        //    keHoach.TrangThai = keHoach.TrangThai == 1 ? 0 : 1;
        //    keHoach.NgayCapNhat = DateTime.Now;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { success = true, trangThai = keHoach.TrangThai == 1 ? "Hoạt động" : "Ngừng hoạt động" });
        //}
    }
}