using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class LichHoc
    {
        [Key]
        public Guid IDLichHoc { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Id kế hoạch nhóm xưởng - ca học (NXCH) không được để trống.")]
        public Guid IdNXCH { get; set; }

        [Required(ErrorMessage = "Id nhóm xưởng không được để trống.")]
        public Guid IdNhomXuong { get; set; }

        [Required(ErrorMessage = "Id học kỳ không được để trống.")]
        public Guid IDHocKy { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; }

        // Navigation properties
        [ForeignKey(nameof(IdNXCH))]
        public virtual KHNXCaHoc KHNXCaHoc { get; set; }

        [ForeignKey(nameof(IDHocKy))]
        public virtual HocKy HocKy { get; set; }

        [ForeignKey(nameof(IdNhomXuong))]
        public virtual NhomXuong NhomXuong { get; set; }

        // Optional: Nếu cần liên kết dự án
        public virtual DuAn DuAn { get; set; }
    }
}
