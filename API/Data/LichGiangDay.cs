using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichGiangDay
    {
        [Key]
        public Guid IdLichDay { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Id Kế hoạch nhóm xưởng - ca học (IdNXCH) không được để trống.")]
        public Guid IdNXCH { get; set; }

        public bool TTDiemDanhMuon { get; set; } = false;

        [Required(ErrorMessage = "Id Nhóm xưởng không được để trống.")]
        public Guid IdNhomXuong { get; set; }

        public Guid? IdDuAn { get; set; }

        [Required(ErrorMessage = "Id địa điểm không được để trống.")]
        public Guid IdDiaDiem { get; set; }

        [Required(ErrorMessage = "Hình thức giảng dạy không được để trống.")]
        [StringLength(100, ErrorMessage = "Hình thức giảng dạy không được vượt quá 100 ký tự.")]
        public string HTGiangDay { get; set; }

        public bool TTDiemDanh { get; set; } = false;

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; } = 1;

        // Navigation properties
        public virtual KHNXCaHoc KHNXCaHoc { get; set; }
        public virtual NhomXuong NhomXuong { get; set; }
        public virtual DuAn DuAn { get; set; }
        public virtual DiaDiem DiaDiem { get; set; }
    }
}
