﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyBoMonsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public QuanLyBoMonsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.QuanLyBoMons
                .Select(x => new
                {
                    x.IDBoMon,
                    x.MaBoMon,
                    x.TenBoMon,
                    x.TrangThai
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/QuanLyBoMons
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int pageSize = 5,
            string? search = null,
            string? status = null
            )
        {
            var query = _context.QuanLyBoMons.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    x.MaBoMon.ToLower().Contains(search) ||
                    x.TenBoMon.ToLower().Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                    query = query.Where(x => x.TrangThai == true);
                else if (status == "inactive")
                    query = query.Where(x => x.TrangThai == false);
            }
            var totalItems = await query.CountAsync();
            var data = await query
                    .OrderByDescending(x => x.NgayTao)
                    .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .Select(x => new
                     {
                         x.IDBoMon,
                         x.MaBoMon,
                         x.TenBoMon,
                         // 🔥 Lấy danh sách tên cơ sở hoạt động từ bảng BoMonCoSo
                         CoSoHoatDong = string.Join(", ", _context.BoMonCoSos
                        .Where(bcs => bcs.IdBoMon == x.IDBoMon)
                        .Include(bcs => bcs.CoSo)
                        .Select(bcs => bcs.CoSo.TenCoSo)
                        .Distinct()
                         ),
                         x.NgayTao,
                         x.NgayCapNhat,
                         x.TrangThai
                     })
                    .ToListAsync();

            return Ok(new
            {
                data,
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    totalItems,
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                }
            });
        }

        // GET: api/QuanLyBoMons/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var quanlyBoMon = await _context.QuanLyBoMons.FindAsync(id);
            if (quanlyBoMon == null)
            {
                return NotFound();
            }
            return Ok(quanlyBoMon);
        }

        // PUT: api/QuanLyBoMons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, QuanLyBoMon quanLyBoMon)
        {
            var qlbm = await _context.QuanLyBoMons.FindAsync(id);
            if (qlbm == null)
            {
                return NotFound();
            }
            qlbm.MaBoMon = quanLyBoMon.MaBoMon;
            qlbm.TenBoMon = quanLyBoMon.TenBoMon;
            qlbm.CoSoHoatDong = quanLyBoMon.CoSoHoatDong;
            qlbm.NgayCapNhat = DateTime.Now;
            qlbm.TrangThai = quanLyBoMon.TrangThai;
            await _context.SaveChangesAsync();
            return Ok(qlbm);
        }

        // POST: api/QuanLyBoMons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] QuanLyBoMon quanLyBoMon)
        {
            quanLyBoMon.IDBoMon = Guid.NewGuid();
            quanLyBoMon.NgayTao = DateTime.Now;
            quanLyBoMon.TrangThai = true; // Default to active status
            if (string.IsNullOrEmpty(quanLyBoMon.CoSoHoatDong))
            {
                quanLyBoMon.CoSoHoatDong = "Chưa có";
            }
            _context.QuanLyBoMons.Add(quanLyBoMon);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            var quanLyBoMon = await _context.QuanLyBoMons.FindAsync(id);
            if (quanLyBoMon == null)
            {
                return NotFound();
            }
            quanLyBoMon.TrangThai = !quanLyBoMon.TrangThai; // Toggle status
            quanLyBoMon.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(quanLyBoMon);
        }
    }
}