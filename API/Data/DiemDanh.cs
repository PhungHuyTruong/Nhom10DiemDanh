using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class DiemDanh
    {
        [Key]
        public Guid IdDiemDanh { get; set; } = Guid.NewGuid();
        public Guid IdSinhVien { get; set; }
        public Guid IdCaHoc { get; set; }
        public Guid IdNhomXuong { get; set; }
        public Guid IdNhanVien { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual SinhVien SinhVien { get; set; }
        public virtual CaHoc CaHoc { get; set; }
        public virtual NhomXuong NhomXuong { get; set; }
        public virtual PhuTrachXuong PhuTrachXuong { get; set; }
        public virtual ICollection<LichSuDiemDanh> LichSuDiemDanhs { get; set; }
    }
}
