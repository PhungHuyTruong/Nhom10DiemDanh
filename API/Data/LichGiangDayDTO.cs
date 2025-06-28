namespace API.Data
{
    public class LichGiangDayDTO
    {
        public Guid IdLichDay { get; set; }
        public Guid IdNXCH { get; set; }
        public Guid IdNhomXuong { get; set; }
        public Guid? IdDuAn { get; set; }
        public Guid IdDiaDiem { get; set; }
        public string HTGiangDay { get; set; }
        public bool TTDiemDanh { get; set; }
        public bool TTDiemDanhMuon { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int TrangThai { get; set; }

        public string TenNhomXuong { get; set; }
        public string TenDiaDiem { get; set; }
        public string TenDuAn { get; set; }
        public string TenCaHoc { get; set; }
    }
}
