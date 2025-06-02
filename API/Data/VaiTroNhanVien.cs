using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class VaiTroNhanVien
    {
        [Key]
        public Guid IdVTNV { get; set; } = Guid.NewGuid();
        public Guid? IdNhanVien { get; set; }
        public Guid? IdVaiTro { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual PhuTrachXuong PhuTrachXuong { get; set; }
        public virtual VaiTro VaiTro { get; set; }
    }
}
