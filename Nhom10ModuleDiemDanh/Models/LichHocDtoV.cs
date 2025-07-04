using System;

namespace Nhom10ModuleDiemDanh.Models
{
    public class LichHocDtoV
    {
        public Guid IdKHNXCH { get; set; }
        public string TenKeHoach { get; set; }
        public string TenNhomXuong { get; set; }
        public string TenCaHoc { get; set; }
        public DateTime NgayHoc { get; set; }
        public int TrangThai { get; set; }
    }
}
