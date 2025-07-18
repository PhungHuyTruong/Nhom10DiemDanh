using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AttendanceCheckDto
    {
        public Guid IdNXCH { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        public bool IsCheckIn { get; set; }
        
        // Make IP and location optional for backward compatibility
        public string? IPAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
