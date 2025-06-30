using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Nhom10ModuleDiemDanh.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichGiangDaysController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public LichGiangDaysController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/LichGiangDays
        [HttpGet("giangvien/{idNhanVien}")]
        public async Task<ActionResult<List<LichGiangDayViewModel>>> GetLichGiangDayTheoNhanVien(Guid idNhanVien)
        {
            var result = await (
        from px in _context.PhuTrachXuongs
        join nx in _context.NhomXuongs on px.IdNhanVien equals nx.IdPhuTrachXuong
        join khnx in _context.KeHoachNhomXuongs on nx.IdNhomXuong equals khnx.IdNhomXuong
        join khnxCa in _context.KHNXCaHocs on khnx.IdKHNX equals khnxCa.IdKHNX
        join ca in _context.CaHocs on khnxCa.IdCaHoc equals ca.IdCaHoc
        join coSo in _context.CoSos on px.IdCoSo equals coSo.IdCoSo into coSoGroup
        from coSo in coSoGroup.DefaultIfEmpty()
        join diaDiem in _context.DiaDiems on coSo.IdCoSo equals diaDiem.IdCoSo into diaDiemGroup
        from diaDiem in diaDiemGroup.DefaultIfEmpty()
        where px.IdNhanVien == idNhanVien
        orderby khnxCa.NgayHoc, ca.TenCaHoc
        select new LichGiangDayViewModel
        {
            NgayHoc = khnxCa.NgayHoc,
            Ca = ca.TenCaHoc,
            ThoiGian = khnxCa.ThoiGian,
            TenNhomXuong = nx.TenNhomXuong,
            DiemDanhTre = khnxCa.DiemDanhTre,
            DuAn = khnx.KeHoach.TenKeHoach,
            LinkOnline = khnxCa.LinkOnline,
            HinhThuc = string.IsNullOrEmpty(khnxCa.LinkOnline) ? "Offline" : "Online",
            DiaDiem = diaDiem != null ? diaDiem.TenDiaDiem : "-"
        }
    ).ToListAsync();

            return Ok(result);
        }
    }
}
