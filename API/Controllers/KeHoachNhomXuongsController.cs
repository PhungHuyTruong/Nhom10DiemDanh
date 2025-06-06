using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using API.Data; // Assuming your DbContext and Models are here

namespace API.Controllers
{
    #region Data Transfer Objects (DTOs)

    /// <summary>
    /// DTO for creating a new KeHoachNhomXuong.
    /// </summary>
    public class CreateKeHoachNhomXuongDto
    {
        [Required]
        public Guid IdNhomXuong { get; set; }

        [Required]
        public Guid IdKeHoach { get; set; }

        [MaxLength(100)]
        public string ThoiGianThucTe { get; set; }

        public int SoBuoi { get; set; }
        public int SoSinhVien { get; set; }
        public int TrangThai { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing KeHoachNhomXuong.
    /// </summary>
    public class UpdateKeHoachNhomXuongDto
    {
        [MaxLength(100)]
        public string ThoiGianThucTe { get; set; }
        public int SoBuoi { get; set; }
        public int SoSinhVien { get; set; }
        public int TrangThai { get; set; }
    }

    /// <summary>
    /// DTO for returning KeHoachNhomXuong data to the client.
    /// Includes related data for context.
    /// </summary>
    public class KeHoachNhomXuongDto
    {
        public Guid IdKHNX { get; set; }
        public Guid IdNhomXuong { get; set; }
        public Guid IdKeHoach { get; set; }
        
        // NOTE: Assuming KeHoach has 'TenKeHoach' and NhomXuong has 'TenNhom' properties.
        // Adjust these property names if they are different in your actual models.
        public string TenKeHoach { get; set; }
        public string TenNhomXuong { get; set; }
        
        public string ThoiGianThucTe { get; set; }
        public int SoBuoi { get; set; }
        public int SoSinhVien { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int TrangThai { get; set; }
    }

    #endregion

    [Route("api/[controller]")]
    [ApiController]
    public class KeHoachNhomXuongsController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;

        public KeHoachNhomXuongsController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }

        // GET: api/KeHoachNhomXuongs
        /// <summary>
        /// Gets a list of all KeHoachNhomXuong entries, including related KeHoach and NhomXuong information.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<KeHoachNhomXuongDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<KeHoachNhomXuongDto>>> GetKeHoachNhomXuongs()
        {
            var keHoachNhomXuongs = await _context.KeHoachNhomXuongs
                .AsNoTracking() // Use AsNoTracking for read-only queries to improve performance.
                .Include(k => k.KeHoach) // Eagerly load the related KeHoach.
                .Include(k => k.NhomXuong) // Eagerly load the related NhomXuong.
                .Select(k => new KeHoachNhomXuongDto
                {
                    IdKHNX = k.IdKHNX,
                    IdKeHoach = k.IdKeHoach,
                    IdNhomXuong = k.IdNhomXuong,
                    // Assuming property names, adjust if necessary
                    TenKeHoach = k.KeHoach.TenKeHoach, 
                    TenNhomXuong = k.NhomXuong.TenNhomXuong,
                    ThoiGianThucTe = k.ThoiGianThucTe,
                    SoBuoi = k.SoBuoi,
                    SoSinhVien = k.SoSinhVien,
                    NgayTao = k.NgayTao,
                    NgayCapNhat = k.NgayCapNhat,
                    TrangThai = k.TrangThai
                })
                .ToListAsync();

            return Ok(keHoachNhomXuongs);
        }

        // GET: api/KeHoachNhomXuongs/5
        /// <summary>
        /// Gets a specific KeHoachNhomXuong by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(KeHoachNhomXuongDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KeHoachNhomXuongDto>> GetKeHoachNhomXuong(Guid id)
        {
            var keHoachNhomXuong = await _context.KeHoachNhomXuongs
                .AsNoTracking()
                .Include(k => k.KeHoach)
                .Include(k => k.NhomXuong)
                .Select(k => new KeHoachNhomXuongDto
                {
                    IdKHNX = k.IdKHNX,
                    IdKeHoach = k.IdKeHoach,
                    IdNhomXuong = k.IdNhomXuong,
                    TenKeHoach = k.KeHoach.TenKeHoach,
                    TenNhomXuong = k.NhomXuong.TenNhomXuong,
                    ThoiGianThucTe = k.ThoiGianThucTe,
                    SoBuoi = k.SoBuoi,
                    SoSinhVien = k.SoSinhVien,
                    NgayTao = k.NgayTao,
                    NgayCapNhat = k.NgayCapNhat,
                    TrangThai = k.TrangThai
                })
                .FirstOrDefaultAsync(k => k.IdKHNX == id);

            if (keHoachNhomXuong == null)
            {
                return NotFound();
            }

            return Ok(keHoachNhomXuong);
        }

        // POST: api/KeHoachNhomXuongs
        /// <summary>
        /// Creates a new KeHoachNhomXuong entry.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(KeHoachNhomXuongDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<KeHoachNhomXuongDto>> PostKeHoachNhomXuong(CreateKeHoachNhomXuongDto createDto)
        {
            // Map DTO to the entity model
            var keHoachNhomXuong = new KeHoachNhomXuong
            {
                IdKeHoach = createDto.IdKeHoach,
                IdNhomXuong = createDto.IdNhomXuong,
                ThoiGianThucTe = createDto.ThoiGianThucTe,
                SoBuoi = createDto.SoBuoi,
                SoSinhVien = createDto.SoSinhVien,
                TrangThai = createDto.TrangThai
                // IdKHNX and NgayTao are set by default in the model
            };

            _context.KeHoachNhomXuongs.Add(keHoachNhomXuong);
            await _context.SaveChangesAsync();
            
            // To return the created object with details, we can fetch it again
            var createdDto = await GetKeHoachNhomXuong(keHoachNhomXuong.IdKHNX);

            return CreatedAtAction(nameof(GetKeHoachNhomXuong), new { id = keHoachNhomXuong.IdKHNX }, createdDto.Value);
        }
        
        // PUT: api/KeHoachNhomXuongs/5
        /// <summary>
        /// Updates an existing KeHoachNhomXuong.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutKeHoachNhomXuong(Guid id, UpdateKeHoachNhomXuongDto updateDto)
        {
            var keHoachNhomXuongFromDb = await _context.KeHoachNhomXuongs.FindAsync(id);

            if (keHoachNhomXuongFromDb == null)
            {
                return NotFound();
            }

            // Update properties from the DTO
            keHoachNhomXuongFromDb.ThoiGianThucTe = updateDto.ThoiGianThucTe;
            keHoachNhomXuongFromDb.SoBuoi = updateDto.SoBuoi;
            keHoachNhomXuongFromDb.SoSinhVien = updateDto.SoSinhVien;
            keHoachNhomXuongFromDb.TrangThai = updateDto.TrangThai;
            
            // Automatically set the update timestamp
            keHoachNhomXuongFromDb.NgayCapNhat = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeHoachNhomXuongExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/KeHoachNhomXuongs/5
        /// <summary>
        /// Deletes a KeHoachNhomXuong by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKeHoachNhomXuong(Guid id)
        {
            var keHoachNhomXuong = await _context.KeHoachNhomXuongs.FindAsync(id);
            if (keHoachNhomXuong == null)
            {
                return NotFound();
            }

            _context.KeHoachNhomXuongs.Remove(keHoachNhomXuong);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KeHoachNhomXuongExists(Guid id)
        {
            return _context.KeHoachNhomXuongs.Any(e => e.IdKHNX == id);
        }
    }
}
