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

        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }

        public CoSo? CoSo { get; set; }

        // ✅ THAY THẾ phần này bằng danh sách vai trò
        public List<Guid> IdVaiTros { get; set; } = new();   // Danh sách Id vai trò được chọn
        public List<string>? TenVaiTros { get; set; }        // Danh sách tên vai trò (dùng để hiển thị nếu cần)
        public List<VaiTro>? VaiTros { get; set; }
    }
}