namespace Nhom10ModuleDiemDanh.Models
{
    public class ScheduleDto
    {
        /// <summary>
        /// Thời gian bắt đầu và kết thúc của ca học (định dạng: "HH:mm - HH:mm")
        /// </summary>
        public string ThoiGian { get; set; }

        /// <summary>
        /// Tên của ca học
        /// </summary>
        public string CaHoc { get; set; }

        /// <summary>
        /// Tên lớp học (Nhóm xưởng)
        /// </summary>
        public string LopHoc { get; set; }

        /// <summary>
        /// Tên giảng viên phụ trách
        /// </summary>
        public string GiangVien { get; set; }

        /// <summary>
        /// Thời gian cho phép điểm danh trễ (chuỗi số phút)
        /// </summary>
        public string DiemDanhTre { get; set; }

        /// <summary>
        /// Thời gian cho phép check-in (sớm 5 phút so với ThoiGianBatDau)
        /// </summary>
        public TimeSpan? CheckInTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc của ca học
        /// </summary>
        public TimeSpan? CheckOutTime { get; set; }

        /// <summary>
        /// ID của KHNXCaHoc để sử dụng trong điểm danh
        /// </summary>
        public Guid IdNXCH { get; set; }

        /// <summary>
        /// Cờ chỉ định có thể check-in hay không
        /// </summary>
        public bool CanCheckIn { get; set; }

        /// <summary>
        /// Cờ chỉ định có thể check-out hay không
        /// </summary>
        public bool CanCheckOut { get; set; }
    }
}
