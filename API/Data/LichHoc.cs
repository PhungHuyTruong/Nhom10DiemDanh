using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class LichHoc
    {
        [Key]
        public Guid IDLichHoc { get; set; }

        [Required(ErrorMessage = "IdNXCH là bắt buộc.")]
        public Guid IdNXCH { get; set; }

        [Required(ErrorMessage = "IdNhomXuong là bắt buộc.")]
        public Guid IdNhomXuong { get; set; }

        [Required(ErrorMessage = "IDHocKy là bắt buộc.")]
        public Guid IDHocKy { get; set; }
        public int TrangThai { get; set; }

        [ForeignKey("IdNXCH")]
        public KHNXCaHoc KHNXCaHoc { get; set; }

        [ForeignKey("IDHocKy")]
        public HocKy HocKy { get; set; }

        [ForeignKey("IdNhomXuong")]
        public NhomXuong NhomXuong { get; set; }

        public DuAn DuAn { get; set; }
    }
}
