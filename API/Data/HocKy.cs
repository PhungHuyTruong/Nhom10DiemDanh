using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace API.Data
{
    public class HocKy
    {
        [Key]
        public Guid IdHocKy { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên học kỳ không được để trống")]
        [StringLength(100, ErrorMessage = "Tên học kỳ không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên học kỳ không được chứa khoảng trắng hoặc ký tự đặc biệt")]
        public string TenHocKy { get; set; }
        [Required(ErrorMessage = "Tên học kỳ không được để trống")]
        [StringLength(100, ErrorMessage = "Tên học kỳ không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên học kỳ không được chứa khoảng trắng hoặc ký tự đặc biệt")]
        public string MaHocKy { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        public virtual ICollection<DuAn>? DuAns { get; set; }
    }
}
