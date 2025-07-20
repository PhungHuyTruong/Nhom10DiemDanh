using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class DiaDiem
    {
        [Key]
        public Guid IdDiaDiem { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên địa điểm không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên địa điểm không được vượt quá 100 ký tự.")]
        public string TenDiaDiem { get; set; }

        public double? ViDo { get; set; }

        public double? KinhDo { get; set; }

        public double? BanKinh { get; set; }

        [Required(ErrorMessage = "Cơ sở liên kết không được để trống.")]
        public Guid IdCoSo { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual CoSo? CoSo { get; set; }  // Đổi tên rõ ràng hơn (CoSos -> CoSo)
        public virtual ICollection<LichGiangDay>? LichGiangDays { get; set; }
    }
}
