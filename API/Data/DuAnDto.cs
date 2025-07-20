using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class DuAnDto
    {
        public Guid IdDuAn { get; set; }

        [Required(ErrorMessage = "Tên dự án không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên dự án không được vượt quá 255 ký tự.")]
        public string TenDuAn { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        [Required(ErrorMessage = "Cấp độ dự án không được để trống.")]
        public Guid? IdCDDA { get; set; }

        public string? TenCapDo { get; set; }

        [Required(ErrorMessage = "Bộ môn không được để trống.")]
        public Guid? IdBoMon { get; set; }

        public string? TenBoMon { get; set; }

        [Required(ErrorMessage = "Học kỳ không được để trống.")]
        public Guid? IdHocKy { get; set; }

        public string? TenHocKy { get; set; }
    }
}
