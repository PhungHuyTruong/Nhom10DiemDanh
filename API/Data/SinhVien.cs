using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class SinhVien
    {
        [Key]
        public Guid IdSinhVien { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên sinh viên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sinh viên không được vượt quá 100 ký tự")]
        public string TenSinhVien { get; set; }

        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(100, ErrorMessage = "Mã sinh viên không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Mã sinh viên không được chứa khoảng trắng hoặc ký tự đặc biệt")]
        public string MaSinhVien { get; set; }
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(100, ErrorMessage = "Mã sinh viên không được vượt quá 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        public Guid? IdNhomXuong { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? IdVaiTro { get; set; }

        // Navigation properties
        public virtual NhomXuong? NhomXuong { get; set; }
        public virtual VaiTro? VaiTro { get; set; }
        public virtual ICollection<DiemDanh>? DiemDanhs { get; set; }
    }
}
