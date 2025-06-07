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

        [Required(ErrorMessage = "Buổi học là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Buổi học phải lớn hơn 0.")]
        public int Buoi { get; set; }

        [Required(ErrorMessage = "Ngày học là bắt buộc.")]
        public DateTime NgayHoc { get; set; }

        [Required(ErrorMessage = "Thời gian học là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Thời gian học không vượt quá 100 ký tự.")]
        public string ThoiGian { get; set; } = string.Empty;

        
        public Guid? IdKHNX { get; set; }

        
        public Guid? IdCaHoc { get; set; }

        [MaxLength(1000, ErrorMessage = "Nội dung không vượt quá 1000 ký tự.")]
        public string? NoiDung { get; set; }

        [MaxLength(500, ErrorMessage = "Link online không vượt quá 500 ký tự.")]
        [Url(ErrorMessage = "Link online không đúng định dạng URL.")]
        public string? LinkOnline { get; set; }

        public bool DiemDanhTre { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; } 

        [Range(0, 3, ErrorMessage = "Trạng thái phải nằm trong khoảng từ 0 đến 3.")]
        public int TrangThai { get; set; }

        // Navigation properties - bỏ validate khi binding API
        [JsonIgnore]
        public virtual KeHoachNhomXuong? KeHoachNhomXuong { get; set; }

        [JsonIgnore]
        public virtual CaHoc? CaHoc { get; set; }

        [JsonIgnore]
        public virtual ICollection<LichGiangDay>? LichGiangDays { get; set; }

        [JsonIgnore]
        public virtual ICollection<LichHoc>? LichHocs { get; set; }

        [JsonIgnore]
        public virtual ICollection<LichSuDiemDanh>? LichSuDiemDanhs { get; set; }
    }
}
