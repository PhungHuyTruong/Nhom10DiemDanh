using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom10ModuleDiemDanh.Models
{
    public class PhuTrachXuongViewModel
    {
        public Guid IdNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string EmailFE { get; set; }
        public string EmailFPT { get; set; }

        public Guid IdCoSo { get; set; }
        public bool TrangThai { get; set; } = true;

        public List<SelectListItem> CoSoList { get; set; } = new List<SelectListItem>();
    }
}
