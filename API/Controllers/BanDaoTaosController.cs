using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanDaoTaoController : ControllerBase
    {
        private readonly ModuleDiemDanhDbContext _context;
        public BanDaoTaoController(ModuleDiemDanhDbContext context)
        {
            _context = context;
        }
        // GET: api/<BanDaoTaoController>
        // --- Sửa API để hỗ trợ tìm kiếm và lọc ---
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int pageSize = 5,
            string? search = null,
            string? status = null)
        {
            var query = _context.BanDaoTaos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    x.MaBanDaoTao.ToLower().Contains(search) ||
                    x.TenBanDaoTao.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                    query = query.Where(x => x.TrangThai == true);
                else if (status == "inactive")
                    query = query.Where(x => x.TrangThai == false);
            }

            var totalItems = await query.CountAsync();
            var data = await query.OrderByDescending(x => x.NgayTao)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
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



        // GET api/<BanDaoTaoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var banDaoTao = await _context.BanDaoTaos.FindAsync(id);
            if (banDaoTao == null)
            {
                return NotFound();
            }
            return Ok(banDaoTao);
        }

        // POST api/<BanDaoTaoController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BanDaoTao banDaoTao)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            banDaoTao.IdBanDaoTao = Guid.NewGuid();
            banDaoTao.NgayTao = DateTime.Now;
            banDaoTao.TrangThai = true;

            _context.BanDaoTaos.Add(banDaoTao);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // PUT api/<BanDaoTaoController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, BanDaoTao updateBanDaoTao)
        {
            var banDaoTao = await _context.BanDaoTaos.FindAsync(id);
            if (banDaoTao == null)
            {
                return NotFound();
            }
            banDaoTao.TenBanDaoTao = updateBanDaoTao.TenBanDaoTao;
            banDaoTao.MaBanDaoTao = updateBanDaoTao.MaBanDaoTao;
            banDaoTao.Email = updateBanDaoTao.Email;
            banDaoTao.TrangThai = updateBanDaoTao.TrangThai;
            banDaoTao.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(banDaoTao);

        }

        // DELETE api/<BanDaoTaoController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var banDaoTao = await _context.BanDaoTaos.FindAsync(id);
            if (banDaoTao == null)
            {
                return NotFound();
            }
            _context.BanDaoTaos.Remove(banDaoTao);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("TrangThai/{id}")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            var banDaoTao = await _context.BanDaoTaos.FindAsync(id);
            if (banDaoTao == null)
            {
                return NotFound();
            }
            banDaoTao.TrangThai = !banDaoTao.TrangThai;
            banDaoTao.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(banDaoTao);
        }
    }
}