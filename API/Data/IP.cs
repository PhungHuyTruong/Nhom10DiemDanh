using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Data
{
    public class IP
    {
        [Key]
        public Guid IdIP { get; set; } = Guid.NewGuid();

        [Required]
        public string KieuIP { get; set; }

        [Required]
        public string IP_DaiIP { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;
        // FK bắt buộc để biết IP thuộc CoSo nào
        [Required]
        public Guid IdCoSo { get; set; }

        // Navigation property
        [JsonIgnore]
        public virtual CoSo CoSo { get; set; }
    }
}