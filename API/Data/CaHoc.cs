using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class CaHoc : IValidatableObject
    {
        [Key]
        public Guid IdCaHoc { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên ca học không được để trống")]
        [StringLength(100, ErrorMessage = "Tên ca học không được vượt quá 100 ký tự")]
        [Display(Name = "Tên ca học")]
        public string TenCaHoc { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
        [Display(Name = "Thời gian bắt đầu")]
        public TimeSpan? ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
        [Display(Name = "Thời gian kết thúc")]
        public TimeSpan? ThoiGianKetThuc { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        [Range(0, 1, ErrorMessage = "Trạng thái chỉ có thể là 0 (Tạm ngưng) hoặc 1 (Hoạt động)")]
        public int TrangThai { get; set; } = 1;
        public Guid? CoSoId { get; set; }

        [JsonIgnore]
        public virtual CoSo? CoSo { get; set; }

        [JsonIgnore]
        public virtual ICollection<DiemDanh>? DiemDanhs { get; set; }

        [JsonIgnore]
        public virtual ICollection<LichHoc>? LichHocs { get; set; }

        [JsonIgnore]
        public virtual ICollection<KHNXCaHoc>? KHNXCaHocs { get; set; }



        // ✅ Validate logic nghiệp vụ: thời gian bắt đầu < kết thúc
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ThoiGianBatDau.HasValue && ThoiGianKetThuc.HasValue)
            {
                if (ThoiGianKetThuc <= ThoiGianBatDau)
                {
                    yield return new ValidationResult(
                        "Thời gian kết thúc phải sau thời gian bắt đầu.",
                        new[] { nameof(ThoiGianKetThuc) }
                    );
                }
            }
        }
    }
}