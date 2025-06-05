using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace API.Data
{
    public class KeHoachNhomXuong
    {
        [Key]
        public Guid IdKHNX { get; set; } = Guid.NewGuid();

        public Guid IdNhomXuong { get; set; }

        public Guid IdKeHoach { get; set; }

        [MaxLength(100)]
        public string ThoiGianThucTe { get; set; }

        public int SoBuoi { get; set; }

        public int SoSinhVien { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        public int TrangThai { get; set; }

        // Navigation properties
        public virtual KeHoach KeHoach { get; set; }
        public virtual NhomXuong NhomXuong { get; set; }
        public virtual ICollection<KHNXCaHoc> KHNXCaHocs { get; set; }
    }
}
