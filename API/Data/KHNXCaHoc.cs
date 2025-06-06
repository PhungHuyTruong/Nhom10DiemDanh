using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class KHNXCaHoc
    {
        [Key]
        public Guid IdNXCH { get; set; } = Guid.NewGuid();

        public string Buoi { get; set; }

        public DateTime NgayHoc { get; set; }

        [MaxLength(100)]
        public string ThoiGian { get; set; }

        public Guid? IdKHNX { get; set; }

        public Guid? IdCaHoc { get; set; }

        public string NoiDung { get; set; }

        [MaxLength(500)]
        public string LinkOnline { get; set; }

        public string DiemDanhTre { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        public int TrangThai { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual KeHoachNhomXuong? KeHoachNhomXuong { get; set; }
        [JsonIgnore]
        public virtual CaHoc? CaHoc { get; set; }
        [JsonIgnore]
        public virtual ICollection<LichGiangDay> LichGiangDays { get; set; } = new List<LichGiangDay>();
        [JsonIgnore]
        public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();
        [JsonIgnore]
        public ICollection<LichSuDiemDanh> LichSuDiemDanhs { get; set; } = new List<LichSuDiemDanh>();
    }
}