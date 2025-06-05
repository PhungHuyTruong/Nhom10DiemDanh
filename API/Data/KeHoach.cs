using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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

        public DateTime NgayDienRa { get; set; }

        public int TrangThai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        public virtual DuAn DuAn { get; set; }
        public virtual ICollection<KeHoachNhomXuong> KeHoachNhomXuongs { get; set; }
    }
}
