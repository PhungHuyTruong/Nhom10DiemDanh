using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace API.Data
{
    public class KHNXCaHoc
    {
        [Key]
        public Guid IdNXCH { get; set; } = Guid.NewGuid();

        public int Buoi { get; set; }

        public DateTime NgayHoc { get; set; }

        [MaxLength(100)]
        public string ThoiGian { get; set; }

        public Guid IdKHNX { get; set; }

        public Guid IdCaHoc { get; set; }

        public string NoiDung { get; set; }

        [MaxLength(500)]
        public string LinkOnline { get; set; }

        public bool DiemDanhTre { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        public int TrangThai { get; set; }

        // Navigation properties
        public virtual KeHoachNhomXuong KeHoachNhomXuong { get; set; }
        public virtual CaHoc CaHoc { get; set; }
        public virtual ICollection<LichGiangDay> LichGiangDays { get; set; }
        public virtual ICollection<LichHoc> LichHocs { get; set; }
        public ICollection<LichSuDiemDanh> LichSuDiemDanhs { get; set; }
    }
}
