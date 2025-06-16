using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class KeHoachInputModel
    {
        public Guid IdKeHoach { get; set; }

        [Required(ErrorMessage = "Tên kế hoạch là bắt buộc")]
        public string TenKeHoach { get; set; } = string.Empty;

        [Required(ErrorMessage = "Id dự án là bắt buộc")]
        public Guid? IdDuAn { get; set; }

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        public DateTime ThoiGianBatDau { get; set; }

        [Required]
        public DateTime ThoiGianKetThuc { get; set; }

        public int TrangThai { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        public string? TenDuAn { get; set; }
        public string? TenBoMon { get; set; }
        public string? TenCapDoDuAn { get; set; }
    }

}
