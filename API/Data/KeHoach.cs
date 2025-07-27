using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class KeHoach
    {
        [Key]
        public Guid IdKeHoach { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên kế hoạch không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên kế hoạch không được vượt quá 255 ký tự.")]
        public string TenKeHoach { get; set; }

        public Guid? IdDuAn { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống.")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống.")]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống.")]
        
        public DateTime ThoiGianKetThuc { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public int TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        [ForeignKey(nameof(IdDuAn))]
        public virtual DuAn DuAn { get; set; }

        public virtual ICollection<KeHoachNhomXuong> KeHoachNhomXuongs { get; set; } = new List<KeHoachNhomXuong>();

        // Dùng cho mục đích hiển thị, không nên ghi vào DB
        [NotMapped]
        public string? TenDuAn { get; set; }

        [NotMapped]
        public string? TenBoMon { get; set; }

        [NotMapped]
        public string? TenCapDoDuAn { get; set; }
    }
}
