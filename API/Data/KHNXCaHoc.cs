using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class KHNXCaHoc
    {
        [Key]
        public Guid IdNXCH { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Buổi học không được để trống.")]
        [StringLength(50, ErrorMessage = "Buổi học không được vượt quá 50 ký tự.")]
        public string Buoi { get; set; }

        [Required(ErrorMessage = "Ngày học không được để trống.")]
        public DateTime NgayHoc { get; set; }

        [Required(ErrorMessage = "Thời gian học không được để trống.")]
        [MaxLength(100, ErrorMessage = "Thời gian học không được vượt quá 100 ký tự.")]
        public string ThoiGian { get; set; }

        [Required(ErrorMessage = "Kế hoạch nhóm xưởng không được để trống.")]
        public Guid? IdKHNX { get; set; }

        [Required(ErrorMessage = "Ca học không được để trống.")]
        public Guid? IdCaHoc { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống.")]
        public string NoiDung { get; set; }

        [MaxLength(500, ErrorMessage = "Link online không được vượt quá 500 ký tự.")]
        public string LinkOnline { get; set; }

        [StringLength(100, ErrorMessage = "Điểm danh trễ không được vượt quá 100 ký tự.")]
        public string DiemDanhTre { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual KeHoachNhomXuong? KeHoachNhomXuong { get; set; }

        public virtual CaHoc? CaHoc { get; set; }

        [JsonIgnore]
        public virtual ICollection<LichGiangDay> LichGiangDays { get; set; } = new List<LichGiangDay>();

        [JsonIgnore]
        public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

        [JsonIgnore]
        public virtual ICollection<LichSuDiemDanh> LichSuDiemDanhs { get; set; } = new List<LichSuDiemDanh>();
    }
}
