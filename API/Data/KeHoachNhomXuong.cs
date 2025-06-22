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

        public Guid IdNhomXuong { get; set; }

        public Guid IdKeHoach { get; set; }


        public DateTime? NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        public int? SoBuoi { get; set; }

        public int SoSinhVien { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

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
        public virtual ICollection<KHNXCaHoc> KHNXCaHocs { get; set; }

        [NotMapped]
        public string? TenPhuTrachXuong { get; set; }

    }
}
