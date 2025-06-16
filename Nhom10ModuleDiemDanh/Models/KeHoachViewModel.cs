using System.ComponentModel.DataAnnotations;

namespace Nhom10ModuleDiemDanh.Models
{
    public class KeHoachViewModel
    {
        public Guid IdKeHoach { get; set; }

        [Required(ErrorMessage = "Tên kế hoạch không được để trống")]
        [MaxLength(255, ErrorMessage = "Tên kế hoạch không được vượt quá 255 ký tự")]
        public string? TenKeHoach { get; set; }

        public Guid? IdDuAn { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string? NoiDung { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
        public DateTime ThoiGianKetThuc { get; set; }

        public int TrangThai { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public string? TenDuAn { get; set; }
        public string? TenBoMon { get; set; }
        public string? TenCapDoDuAn { get; set; }
    }
}
