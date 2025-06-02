using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class BoMonCoSo
    {
        [Key]
        public Guid IdBoMonCoSo { get; set; } = Guid.NewGuid();
        public Guid? IdBoMon { get; set; }
        public Guid? IdCoSo { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual QuanLyBoMon QuanLyBoMon { get; set; }
        public virtual CoSo CoSo { get; set; }
    }
}
