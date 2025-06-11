using System;
using System.ComponentModel.DataAnnotations;

namespace Nhom10ModuleDiemDanh.Models
{
    public class IPDto
    {
        public Guid IdIP { get; set; }

        [Required(ErrorMessage = "Kiểu IP là bắt buộc.")]
        public string KieuIP { get; set; }

        [Required(ErrorMessage = "Địa chỉ IP là bắt buộc.")]
        public string IP_DaiIP { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }

        [Required(ErrorMessage = "Cơ sở là bắt buộc.")]
        public Guid IdCoSo { get; set; }
    }
}