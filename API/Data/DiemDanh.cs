using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class DiemDanh
    {
        [Key]
        public Guid IdDiemDanh { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Sinh viên không được để trống.")]
        public Guid IdSinhVien { get; set; }

        [Required(ErrorMessage = "Ca học không được để trống.")]
        public Guid IdCaHoc { get; set; }

        [Required(ErrorMessage = "Nhóm xưởng không được để trống.")]
        public Guid IdNhomXuong { get; set; }

        [Required(ErrorMessage = "Nhân viên điểm danh không được để trống.")]
        public Guid IdNhanVien { get; set; }

        [Required(ErrorMessage = "Ngày tạo là bắt buộc.")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual SinhVien SinhVien { get; set; }
        public virtual CaHoc CaHoc { get; set; }
        public virtual NhomXuong NhomXuong { get; set; }

        // PhuTrachXuong ở đây đặt tên hơi gây nhầm lẫn, nếu đại diện cho nhân viên phụ trách thì nên rename
        public virtual PhuTrachXuong PhuTrachXuong { get; set; }

        public virtual ICollection<LichSuDiemDanh> LichSuDiemDanhs { get; set; }
    }
}
