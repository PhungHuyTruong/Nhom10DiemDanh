using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class DuAn
    {
        [Key]
        public Guid IdDuAn { get; set; } = Guid.NewGuid();
        public string TenDuAn { get; set; }
        public string MaDuAn { get; set; }
        public string MoTa { get; set; }
        public Guid? IdCDDA { get; set; }
        public Guid? IdBoMon { get; set; }
        public Guid? IdHocKy { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual CapDoDuAn? CapDoDuAn { get; set; }
        public virtual QuanLyBoMon? QuanLyBoMon { get; set; }
        public virtual HocKy? HocKy { get; set; }
        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; }
        public virtual ICollection<LichGiangDay>? LichGiangDays { get; set; }
        public virtual ICollection<LichHoc>? LichHocs { get; set; } = new List<LichHoc>();
    }
}
