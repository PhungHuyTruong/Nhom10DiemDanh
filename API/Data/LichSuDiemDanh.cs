using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichSuDiemDanh
    {
        [Key]
        public Guid IdLSDD { get; set; } = Guid.NewGuid();
        public Guid IdDiemDanh { get; set; }
        public Guid IdNXCH { get; set; }
        public DateTime ThoiGianDiemDanh { get; set; }
        public string NoiDungBuoiHoc { get; set; }
        public string HinhThuc { get; set; }
        public string DiaDiem { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; } = 1;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public int TrangThaiDuyet { get; set; } = 1;

        // Navigation properties
        public virtual DiemDanh DiemDanh { get; set; }
        public virtual KHNXCaHoc KHNXCaHoc { get; set; }
    }
}
