namespace API.Dtos
{
    public class DuAnDto
    {
        public Guid IdDuAn { get; set; }
        public string TenDuAn { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }

        public Guid? IdCDDA { get; set; }
        public string? TenCapDo { get; set; }

        public Guid? IdBoMon { get; set; }
        public string? TenBoMon { get; set; }

        public Guid? IdHocKy { get; set; }
        public string? TenHocKy { get; set; }
    }
}
