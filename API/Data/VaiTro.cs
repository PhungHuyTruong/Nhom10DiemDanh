using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class VaiTro
    {
        [Key]
        public Guid IdVaiTro { get; set; } = Guid.NewGuid();

        [Required]
        public string TenVaiTro { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        public virtual ICollection<PhuTrachXuong>? PhuTrachXuongs { get; set; }

        public virtual ICollection<BanDaoTao>? BanDaoTaos { get; set; }
        public virtual ICollection<SinhVien>? SinhViens { get; set; }
        public virtual ICollection<VaiTroNhanVien>? VaiTroNhanViens { get; set; }
    }
}
