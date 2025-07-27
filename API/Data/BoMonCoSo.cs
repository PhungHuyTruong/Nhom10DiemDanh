using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class BoMonCoSo
    {
        [Key]
        public Guid IdBoMonCoSo { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Vui lòng chọn Bộ Môn.")]
        public Guid? IdBoMon { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Cơ Sở.")]
        public Guid? IdCoSo { get; set; }

        [Required(ErrorMessage = "Ngày tạo là bắt buộc.")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        [ForeignKey("IdBoMon")]
        public virtual QuanLyBoMon QuanLyBoMon { get; set; }

        [ForeignKey("IdCoSo")]
        public virtual CoSo CoSo { get; set; }
    }
}
