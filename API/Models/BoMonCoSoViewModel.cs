namespace API.Models
{
    public class BoMonCoSoViewModel
    {
        public Guid IdBoMonCoSo { get; set; }
        public Guid? IdBoMon { get; set; }
        public string TenBoMon { get; set; }
        public Guid? IdCoSo { get; set; }
        public string TenCoSo { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}
