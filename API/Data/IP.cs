using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class IP
    {
        [Key]
        public Guid IdIP { get; set; } = Guid.NewGuid();

        [Required]
        public string KieuIP { get; set; }

        [Required]
        public string IP_DaiIP { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        public virtual ICollection<CoSo>? CoSos { get; set; }
    }
}
