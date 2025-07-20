using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class IP
    {
        [Key]
        public Guid IdIP { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Kiểu IP không được để trống.")]
        [StringLength(50, ErrorMessage = "Kiểu IP không được vượt quá 50 ký tự.")]
        public string KieuIP { get; set; }

        [Required(ErrorMessage = "Địa chỉ IP không được để trống.")]
        [StringLength(100, ErrorMessage = "Địa chỉ IP không được vượt quá 100 ký tự.")]
        public string IP_DaiIP { get; set; }

        [Required(ErrorMessage = "Ngày tạo là bắt buộc.")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public bool TrangThai { get; set; } = true;

        // FK: IP bắt buộc phải thuộc 1 Cơ sở
        [Required(ErrorMessage = "Cơ sở sử dụng IP không được để trống.")]
        public Guid IdCoSo { get; set; }

        // Navigation property
        [JsonIgnore]
        [ForeignKey("IdCoSo")]
        public virtual CoSo? CoSo { get; set; }
    }
}
