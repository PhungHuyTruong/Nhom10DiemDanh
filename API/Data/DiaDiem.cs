using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class DiaDiem
    {
        [Key]
        public Guid IdDiaDiem { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string TenDiaDiem { get; set; }

        public double? ViDo { get; set; }
        public double? KinhDo { get; set; }
        public double? BanKinh { get; set; }
        public Guid IdCoSo { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual CoSo? CoSos { get; set; }
        public virtual ICollection<LichGiangDay>? LichGiangDays { get; set; }
    }
}
