using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichHoc
    {
        [Key]
        public Guid IDLichHoc { get; set; }
        public Guid IdNXCH { get; set; }
        public Guid IdNhomXuong { get; set; }
        public Guid IDHocKy { get; set; }
        public int TrangThai { get; set; }

        public KHNXCaHoc KHNXCaHoc { get; set; }
        public HocKy HocKy { get; set; }
        public NhomXuong NhomXuong { get; set; }
        public DuAn DuAn { get; set; }
    }
}
