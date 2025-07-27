using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class CoSo
    {
        [Key]
        public Guid IdCoSo { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên cơ sở không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên cơ sở không được vượt quá 255 ký tự.")]
        public string TenCoSo { get; set; }

        [Required(ErrorMessage = "Mã cơ sở không được để trống.")]
        [StringLength(50, ErrorMessage = "Mã cơ sở không được vượt quá 50 ký tự.")]
        public string MaCoSo { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        public string Email { get; set; }

        public Guid? IdDiaDiem { get; set; }
        public Guid? IdIP { get; set; }
        public Guid? IdCaHoc { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual ICollection<DiaDiem> DiaDiems { get; set; }
        public virtual ICollection<IP> IPs { get; set; } = new List<IP>();
        public virtual ICollection<CaHoc> CaHocs { get; set; } = new List<CaHoc>();
        public virtual ICollection<PhuTrachXuong> PhuTrachXuongs { get; set; }
        public virtual ICollection<BoMonCoSo> BoMonCoSos { get; set; }
        public virtual ICollection<KHNXCaHoc> KHNXCaHocs { get; set; }
    }
}
