namespace API.Models
{
    public class KHNXCaHocUpdateDto
    {
        public Guid IdNXCH { get; set; }
        public string Buoi { get; set; }
        public DateTime NgayHoc { get; set; }
        public Guid? IdCaHoc { get; set; }
        public string NoiDung { get; set; }
        public string LinkOnline { get; set; }
        public string DiemDanhTre { get; set; }
       // public int TrangThai { get; set; }
    }
}
