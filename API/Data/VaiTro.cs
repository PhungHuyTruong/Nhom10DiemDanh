using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class VaiTro
    {
        [Key]
        public Guid IdVaiTro { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên vai trò không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự.")]
        public string TenVaiTro { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual ICollection<BanDaoTao>? BanDaoTaos { get; set; } = new List<BanDaoTao>();

        public virtual ICollection<SinhVien>? SinhViens { get; set; } = new List<SinhVien>();

        public virtual ICollection<VaiTroNhanVien>? VaiTroNhanViens { get; set; } = new List<VaiTroNhanVien>();
    }
}
