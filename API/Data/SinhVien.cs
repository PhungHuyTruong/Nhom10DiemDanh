using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class SinhVien
    {
        [Key]
        public Guid IdSinhVien { get; set; } = Guid.NewGuid();
        public string TenSinhVien { get; set; }
        public string MaSinhVien { get; set; }
        public string Email { get; set; }
        public Guid IdNhomXuong { get; set; }
        public int TrangThai { get; set; } = 1;
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid IdVaiTro { get; set; }

        // Navigation properties
        public virtual NhomXuong NhomXuong { get; set; }
        public virtual VaiTro VaiTro { get; set; }
        public virtual ICollection<DiemDanh> DiemDanhs { get; set; }
    }
}
