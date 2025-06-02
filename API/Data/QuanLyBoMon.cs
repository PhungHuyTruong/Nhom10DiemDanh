using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class QuanLyBoMon
    {
        [Key]
        public Guid IDBoMon { get; set; }

        public string MaBoMon { get; set; }
        public string TenBoMon { get; set; }
        public string CoSoHoatDong { get; set; }

        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }

        // Navigation properties
        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; }
        public virtual ICollection<DuAn>? DuAns { get; set; }
        public virtual ICollection<BoMonCoSo>? BoMonCoSos { get; set; }
    }
}
