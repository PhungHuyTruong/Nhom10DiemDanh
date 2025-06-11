using System;

namespace API.Models
{
    public class IPViewModel
    {
        public Guid IdIP { get; set; }
        public string KieuIP { get; set; }
        public string IP_DaiIP { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }
        public Guid IdCoSo { get; set; }
    }
}