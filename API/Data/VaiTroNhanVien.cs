using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class VaiTroNhanVien
    {
        [Key]
        public Guid IdVTNV { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Nhân viên không được để trống.")]
        public Guid? IdNhanVien { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        public Guid? IdVaiTro { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual PhuTrachXuong PhuTrachXuong { get; set; }
        public virtual VaiTro VaiTro { get; set; }
    }
}
