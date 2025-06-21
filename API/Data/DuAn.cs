using API.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class DuAn
    {
        [Key]
        public Guid IdDuAn { get; set; }

        [Required, StringLength(255)]
        public string TenDuAn { get; set; }

        [StringLength(1000)]
        public string MoTa { get; set; }

        public Guid? IdCDDA { get; set; }
    
        public Guid? IdBoMon { get; set; }
        public Guid? IdHocKy { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        [ForeignKey("IdCDDA")]
        public virtual CapDoDuAn? CapDoDuAn { get; set; }
        public virtual QuanLyBoMon? QuanLyBoMon { get; set; }

        [ForeignKey("IdHocKy")]
        public virtual HocKy? HocKy { get; set; }

        public virtual ICollection<NhomXuong>? NhomXuongs { get; set; }
        public virtual ICollection<LichGiangDay>? LichGiangDays { get; set; }
        public virtual ICollection<LichHoc>? LichHocs { get; set; } = new List<LichHoc>();
    }
}
