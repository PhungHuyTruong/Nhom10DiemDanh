namespace API.Models
{
    public class ScheduleViewModel
    {
        public string ThoiGian { get; set; }
        public string CaHoc { get; set; }
        public string LopHoc { get; set; }
        public string GiangVien { get; set; }
        public string DiemDanhTre { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public Guid IdNXCH { get; set; }
        public bool CanCheckIn { get; set; }
        public bool CanCheckOut { get; set; }
        public string Status { get; set; }
        public bool HasCheckedIn { get; set; }
        public bool HasCheckedOut { get; set; }
    }
}
