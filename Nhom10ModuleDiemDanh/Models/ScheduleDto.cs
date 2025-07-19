namespace Nhom10ModuleDiemDanh.Models
{
    public class ScheduleDto
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

        public Guid IdSinhVien { get; set; }
    }
}