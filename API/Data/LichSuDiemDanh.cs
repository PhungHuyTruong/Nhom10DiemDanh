using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichSuDiemDanh
    {
        [Key]
        public Guid IdLSDD { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Id điểm danh không được để trống.")]
        public Guid IdDiemDanh { get; set; }

        [Required(ErrorMessage = "Id kế hoạch nhóm xưởng - ca học không được để trống.")]
        public Guid IdNXCH { get; set; }

        [Required(ErrorMessage = "Thời gian điểm danh không được để trống.")]
        public DateTime ThoiGianDiemDanh { get; set; }

        [Required(ErrorMessage = "Nội dung buổi học không được để trống.")]
        [StringLength(500, ErrorMessage = "Nội dung buổi học không được vượt quá 500 ký tự.")]
        public string NoiDungBuoiHoc { get; set; }

        [Required(ErrorMessage = "Hình thức không được để trống.")]
        [StringLength(100, ErrorMessage = "Hình thức không được vượt quá 100 ký tự.")]
        public string HinhThuc { get; set; }

        [Required(ErrorMessage = "Địa điểm không được để trống.")]
        [StringLength(255, ErrorMessage = "Địa điểm không được vượt quá 255 ký tự.")]
        public string DiaDiem { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự.")]
        public string GhiChu { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; } = 1;

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái duyệt là bắt buộc.")]
        public int TrangThaiDuyet { get; set; } = 1;

        // Navigation properties
        public virtual DiemDanh DiemDanh { get; set; }
        public virtual KHNXCaHoc KHNXCaHoc { get; set; }
    }
}
