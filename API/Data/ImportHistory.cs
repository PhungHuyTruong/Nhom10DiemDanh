using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class ImportHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime ImportDate { get; set; }
        public string ImportedBy { get; set; } // Có thể null nếu không có thông tin người dùng
        public string Status { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // Ví dụ: "KHNXCaHoc"
        public Guid? IdKHNX { get; set; } // <-- Thêm dòng này
    }
}
