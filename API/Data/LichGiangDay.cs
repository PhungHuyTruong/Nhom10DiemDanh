using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichGiangDay
    {
        [Key]
        public Guid IdLichDay { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "IdNXCH là bắt buộc.")]
        public Guid IdNXCH { get; set; }

        public bool TTDiemDanhMuon { get; set; } = false;

        [Required(ErrorMessage = "IdNhomXuong là bắt buộc.")]
        public Guid IdNhomXuong { get; set; }

        public Guid? IdDuAn { get; set; } // Có thể null

        [Required(ErrorMessage = "IdDiaDiem là bắt buộc.")]
        public Guid IdDiaDiem { get; set; }

        [Required(ErrorMessage = "Hình thức giảng dạy là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Hình thức giảng dạy không được vượt quá 100 ký tự.")]
        public string HTGiangDay { get; set; }

        public bool TTDiemDanh { get; set; } = false;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Range(0, 2, ErrorMessage = "Trạng thái chỉ nhận giá trị từ 0 đến 2.")]
        public int TrangThai { get; set; } = 1;

        // Navigation properties
        public virtual KHNXCaHoc KHNXCaHoc { get; set; }
        public virtual NhomXuong NhomXuong { get; set; }
        public virtual DuAn DuAn { get; set; }
        public virtual DiaDiem DiaDiem { get; set; }
    }
}
