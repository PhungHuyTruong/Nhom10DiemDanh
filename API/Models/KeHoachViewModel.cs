namespace API.Models
{
    public class KeHoachViewModel
    {
        public Guid IdKeHoach { get; set; }
        public string TenKeHoach { get; set; }
        public Guid? IdDuAn { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string TenDuAn { get; set; }
        public string TenBoMon { get; set; }
        public string TenCapDoDuAn { get; set; }
    }
}
