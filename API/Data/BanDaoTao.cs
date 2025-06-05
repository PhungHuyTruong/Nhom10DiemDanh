using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class BanDaoTao
    {
        [Key]
        public Guid IdBanDaoTao { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên ban đào tạo là bắt buộc.")]
        public string TenBanDaoTao { get; set; }

        [Required(ErrorMessage = "Mã ban đào tạo là bắt buộc.")]
        public string MaBanDaoTao { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        public Guid? IdVaiTro { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation property
        public virtual VaiTro? VaiTro { get; set; }
    }
}