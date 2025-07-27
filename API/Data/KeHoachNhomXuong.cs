using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class KeHoachNhomXuong
    {
        [Key]
        public Guid IdKHNX { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Nhóm xưởng không được để trống.")]
        public Guid IdNhomXuong { get; set; }

        [Required(ErrorMessage = "Kế hoạch không được để trống.")]
        public Guid IdKeHoach { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        [Range(0, 100, ErrorMessage = "Số buổi phải từ 0 đến 100.")]
        public int? SoBuoi { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số sinh viên phải là số dương.")]
        public int SoSinhVien { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int TrangThai { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ValidateNever]
        public virtual KeHoach KeHoach { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual NhomXuong NhomXuong { get; set; }

        [NotMapped]
        public string? TenNhomXuong { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<KHNXCaHoc> KHNXCaHocs { get; set; } = new List<KHNXCaHoc>();

        [NotMapped]
        public string? TenPhuTrachXuong { get; set; }
    }
}
