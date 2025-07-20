using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class QuanLyBoMon
    {
        [Key]
        public Guid IDBoMon { get; set; }

        [Required(ErrorMessage = "Mã bộ môn không được để trống.")]
        [MaxLength(50, ErrorMessage = "Mã bộ môn không được vượt quá 50 ký tự.")]
        public string MaBoMon { get; set; }

        [Required(ErrorMessage = "Tên bộ môn không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên bộ môn không được vượt quá 100 ký tự.")]
        public string TenBoMon { get; set; }

        [MaxLength(255, ErrorMessage = "Cơ sở hoạt động không được vượt quá 255 ký tự.")]
        public string CoSoHoatDong { get; set; }

        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        // Navigation properties
        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; } = new List<NhomXuong>();
        public virtual ICollection<DuAn>? DuAns { get; set; } = new List<DuAn>();
        public virtual ICollection<BoMonCoSo>? BoMonCoSos { get; set; } = new List<BoMonCoSo>();
    }
}
