namespace API.Models
{
    public class GanNhomXuongRequest
    {
        public Guid IdNhomXuong { get; set; }
        public List<Guid> IdSinhVien { get; set; } = new();
    }
}
