using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    public class KeHoach
    {
        [Key]
        public Guid IdKeHoach { get; set; } = Guid.NewGuid();

        [MaxLength(255)]
        public string TenKeHoach { get; set; }

        public Guid? IdDuAn { get; set; }

        public string NoiDung { get; set; }

        public DateTime ThoiGianBatDau { get; set; }

        public DateTime ThoiGianKetThuc { get; set; }

        public int TrangThai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        [ForeignKey(nameof(IdDuAn))]
        public virtual DuAn DuAn { get; set; }
        public virtual ICollection<KeHoachNhomXuong> KeHoachNhomXuongs { get; set; }

        public string? TenDuAn { get; set; }
        public string? TenBoMon { get; set; }
        public string? TenCapDoDuAn { get; set; }
    }
}
