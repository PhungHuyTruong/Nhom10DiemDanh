namespace Nhom10ModuleDiemDanh.Models
{
    public class LichHocViewDto
    {
        public DateTime NgayHoc { get; set; }

        public string Buoi { get; set; }              // VD: "Buổi 1"
        public string TenCaHoc { get; set; }          // VD: "Ca 1"
        public string ThoiGian { get; set; }          // VD: "21:00 - 23:00"

        public string Ca => $"{TenCaHoc} - {(string.IsNullOrEmpty(ThoiGian) ? "Offline" : ThoiGian)}";

        public string TenNhomXuong { get; set; }
        public string TenDuAn { get; set; }
        public string LinkOnline { get; set; }
        public string DiaDiem { get; set; }
        public string GiangVienPhuTrach { get; set; }
        public string MoTa { get; set; } = "Chi tiết";
    }

}
