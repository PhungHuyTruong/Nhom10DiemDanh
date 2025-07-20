using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class PhuTrachXuong
    {
        [Key]
        public Guid IdNhanVien { get; set; }

        [Required(ErrorMessage = "Tên nhân viên không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên nhân viên không được vượt quá 100 ký tự.")]
        public string TenNhanVien { get; set; }

        [Required(ErrorMessage = "Mã nhân viên không được để trống.")]
        [MaxLength(50, ErrorMessage = "Mã nhân viên không được vượt quá 50 ký tự.")]
        public string MaNhanVien { get; set; }

        [EmailAddress(ErrorMessage = "Email FE không hợp lệ.")]
        [MaxLength(100, ErrorMessage = "Email FE không được vượt quá 100 ký tự.")]
        public string EmailFE { get; set; }

        [EmailAddress(ErrorMessage = "Email FPT không hợp lệ.")]
        [MaxLength(100, ErrorMessage = "Email FPT không được vượt quá 100 ký tự.")]
        public string EmailFPT { get; set; }

        public Guid? IdCoSo { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;

        [Required]
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<VaiTroNhanVien>? VaiTroNhanViens { get; set; } = new List<VaiTroNhanVien>();

        [JsonIgnore]
        public virtual CoSo? CoSo { get; set; }

        [JsonIgnore]
        public virtual ICollection<DiemDanh>? DiemDanhs { get; set; } = new List<DiemDanh>();

        [JsonIgnore]
        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; } = new List<NhomXuong>();
    }
}
