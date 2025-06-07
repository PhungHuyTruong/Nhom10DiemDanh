namespace Nhom10ModuleDiemDanh.Models
{
    public class CoSoViewModel
    {
        public Guid IdCoSo { get; set; }
        public string TenCoSo { get; set; }
        public string MaCoSo { get; set; }
        public string? DiaChi { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string TrangThai { get; set; }
        public Guid? IdDiaDiem { get; set; }
        public Guid? IdIP { get; set; }
        
    }
}
