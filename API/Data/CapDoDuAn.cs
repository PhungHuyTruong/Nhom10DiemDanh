using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class CapDoDuAn
    {
        [Key]
        public Guid IdCDDA { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên học kỳ không được để trống")]
        [StringLength(100, ErrorMessage = "Tên cấp độ không được vượt quá 100 ký tự")]
        public string TenCapDoDuAn { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Mã cấp độ không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Mã cấp độ không được chứa khoảng trắng hoặc ký tự đặc biệt")]
        public string MaCapDoDuAn { get; set; }

        public string MoTa { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        public virtual ICollection<DuAn>? DuAns { get; set; }
    }
}
