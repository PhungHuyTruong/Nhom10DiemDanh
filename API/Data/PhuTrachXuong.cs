using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;  // Thêm dòng này nếu converter nằm trong namespace này


namespace API.Data
{
    public class PhuTrachXuong
    {
        [Key]
        public Guid IdNhanVien { get; set; }

        [Required, MaxLength(100)]
        public string TenNhanVien { get; set; }

        [Required, MaxLength(50)]
        public string MaNhanVien { get; set; }

        [MaxLength(100)]
        public string EmailFE { get; set; }

        [MaxLength(100)]
        public string EmailFPT { get; set; }

        public Guid? IdCoSo { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
      
        public bool TrangThai { get; set; } = true;

        // Navigation properties

        public Guid? IdVaiTro { get; set; }

        [JsonIgnore]
        public virtual VaiTro? VaiTro { get; set; }
        [JsonIgnore]
        public virtual CoSo? CoSo { get; set; }
        [JsonIgnore]
        public virtual ICollection<DiemDanh>? DiemDanhs { get; set; }
        [JsonIgnore]
        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; }
    }
}
