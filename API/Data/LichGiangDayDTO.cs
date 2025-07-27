using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LichGiangDayDTO
    {
        public Guid IdLichDay { get; set; }

        [Required(ErrorMessage = "IdNXCH (Kế hoạch nhóm xưởng - ca học) không được để trống.")]
        public Guid IdNXCH { get; set; }

        [Required(ErrorMessage = "Id nhóm xưởng không được để trống.")]
        public Guid IdNhomXuong { get; set; }

        public Guid? IdDuAn { get; set; }

        [Required(ErrorMessage = "Id địa điểm không được để trống.")]
        public Guid IdDiaDiem { get; set; }

        [Required(ErrorMessage = "Hình thức giảng dạy không được để trống.")]
        [StringLength(100, ErrorMessage = "Hình thức giảng dạy không được vượt quá 100 ký tự.")]
        public string HTGiangDay { get; set; }

        public bool TTDiemDanh { get; set; } = false;
        public bool TTDiemDanhMuon { get; set; } = false;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; }

        // Thông tin hiển thị, không ghi vào DB
        public string TenNhomXuong { get; set; }
        public string TenDiaDiem { get; set; }
        public string TenDuAn { get; set; }
        public string TenCaHoc { get; set; }
    }
}
