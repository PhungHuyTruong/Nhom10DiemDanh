namespace Nhom10ModuleDiemDanh.Models
{
    public class PhuTrachXuongDto
    {
        public string TenNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string EmailFE { get; set; }
        public string EmailFPT { get; set; }
        public Guid? IdCoSo { get; set; }

        public string? TenCoSo { get; set; }
    }
}
