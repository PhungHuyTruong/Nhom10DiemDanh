using System.ComponentModel.DataAnnotations;

namespace Nhom10ModuleDiemDanh.Models
{
    public class DiaDiemCoSoViewModel
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
    }
}
