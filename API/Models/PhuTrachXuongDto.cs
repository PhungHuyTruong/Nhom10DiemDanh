using API.Data;

namespace API.Models
{
    public class PhuTrachXuongDto
    {
        public Guid IdNhanVien { get; set; }         // ✅ Dùng cho sửa, đổi trạng thái
        public string TenNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string EmailFE { get; set; }
        public string EmailFPT { get; set; }
        public Guid? IdCoSo { get; set; }
        public string? TenCoSo { get; set; }

        public bool TrangThai { get; set; }          // ✅ Đã thêm trước
        public DateTime NgayTao { get; set; }        // ✅ Hiển thị ngày tạo
        public DateTime? NgayCapNhat { get; set; }
        public CoSo? CoSo { get; set; }

        public Guid? IdVaiTro { get; set; }
        public string TenVaiTro { get; set; }
        public VaiTro? VaiTro { get; set; }
    }
}
