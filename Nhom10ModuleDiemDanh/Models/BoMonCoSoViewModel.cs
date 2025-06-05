namespace Nhom10ModuleDiemDanh.Models
{
    public class BoMonCoSoViewModel
    {
        public Guid IdBoMonCoSo { get; set; }
        public Guid? IdBoMon { get; set; }
        public string TenBoMon { get; set; } = "Chưa có tên";
        public Guid? IdCoSo { get; set; }
        public string TenCoSo { get; set; } = "Tất cả cơ sở"; // Giá trị mặc định
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
    }
}