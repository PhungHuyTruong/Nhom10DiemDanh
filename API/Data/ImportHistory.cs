using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class ImportHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên file không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên file không được vượt quá 255 ký tự.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Ngày import không được để trống.")]
        public DateTime ImportDate { get; set; }

        [StringLength(255, ErrorMessage = "Tên người import không được vượt quá 255 ký tự.")]
        public string ImportedBy { get; set; }

        [Required(ErrorMessage = "Trạng thái import không được để trống.")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự.")]
        public string Status { get; set; }

        [StringLength(1000, ErrorMessage = "Thông báo không được vượt quá 1000 ký tự.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Loại import không được để trống.")]
        [StringLength(100, ErrorMessage = "Loại import không được vượt quá 100 ký tự.")]
        public string Type { get; set; } // Ví dụ: "KHNXCaHoc"

        public Guid? IdKHNX { get; set; } // Liên kết đến bảng khác nếu có
    }
}
