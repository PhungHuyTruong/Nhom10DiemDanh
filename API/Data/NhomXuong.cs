using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class NhomXuong
    {
        [Key]
        public Guid IdNhomXuong { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên nhóm xưởng không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên nhóm xưởng không được vượt quá 100 ký tự.")]
        public string TenNhomXuong { get; set; }

        [ForeignKey("DuAn")]
        public Guid? IdDuAn { get; set; }

        public Guid? IdBoMon { get; set; }

        [Required(ErrorMessage = "Người phụ trách xưởng là bắt buộc.")]
        [ForeignKey("PhuTrachXuong")]
        public Guid IdPhuTrachXuong { get; set; }

        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string MoTa { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; } = 1;

        // Navigation properties
        public virtual DuAn? DuAn { get; set; }

        [ForeignKey("IdBoMon")]
        public virtual QuanLyBoMon? QuanLyBoMon { get; set; }

        public virtual PhuTrachXuong? PhuTrachXuong { get; set; }

        public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();

        public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

        public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

        public virtual ICollection<LichGiangDay> LichGiangDays { get; set; } = new List<LichGiangDay>();

        public virtual ICollection<KeHoachNhomXuong> KeHoachNhomXuongs { get; set; } = new List<KeHoachNhomXuong>();
    }
}
