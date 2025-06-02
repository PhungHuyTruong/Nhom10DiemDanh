using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichGiangDay
    {
        [Key]
        public Guid IdLichDay { get; set; } = Guid.NewGuid();
        public Guid IdNXCH { get; set; }
        public bool TTDiemDanhMuon { get; set; } = false;
        public Guid IdNhomXuong { get; set; }
        public Guid? IdDuAn { get; set; }
        public Guid IdDiaDiem { get; set; }
        public string HTGiangDay { get; set; }
        public bool TTDiemDanh { get; set; } = false;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public int TrangThai { get; set; } = 1;

        // Navigation properties
        public virtual KHNXCaHoc KHNXCaHoc { get; set; }
        public virtual NhomXuong NhomXuong { get; set; }
        public virtual DuAn DuAn { get; set; }
        public virtual DiaDiem DiaDiem { get; set; }
    }
}
