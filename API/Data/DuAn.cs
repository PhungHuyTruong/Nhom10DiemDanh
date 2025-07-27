using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class DuAn
    {
        [Key]
        public Guid IdDuAn { get; set; }

        [Required(ErrorMessage = "Tên dự án không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên dự án không được vượt quá 255 ký tự.")]
        public string TenDuAn { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string MoTa { get; set; }

        public Guid? IdCDDA { get; set; }

        public Guid? IdBoMon { get; set; }

        public Guid? IdHocKy { get; set; }

        [Required(ErrorMessage = "Ngày tạo là bắt buộc.")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public bool TrangThai { get; set; } = true;

        // Navigation properties

        [ForeignKey("IdCDDA")]
        public virtual CapDoDuAn? CapDoDuAn { get; set; }

        [ForeignKey("IdBoMon")]
        public virtual QuanLyBoMon? QuanLyBoMon { get; set; }

        [ForeignKey("IdHocKy")]
        public virtual HocKy? HocKy { get; set; }

        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; } = new List<NhomXuong>();

        public virtual ICollection<LichGiangDay>? LichGiangDays { get; set; } = new List<LichGiangDay>();

        public virtual ICollection<LichHoc>? LichHocs { get; set; } = new List<LichHoc>();
    }
}
