using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class PhuTrachXuong
    {
        [Key]
        public Guid IdNhanVien { get; set; }

        [Required, MaxLength(100)]
        public string TenNhanVien { get; set; }

        [Required, MaxLength(50)]
        public string MaNhanVien { get; set; }

        [MaxLength(100)]
        public string EmailFE { get; set; }

        [MaxLength(100)]
        public string EmailFPT { get; set; }

        [Required]
        public Guid IdCoSo { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual CoSo CoSo { get; set; }
        public virtual ICollection<DiemDanh> DiemDanhs { get; set; }
        public virtual ICollection<NhomXuong> NhomXuongs { get; set; }
    }
}
