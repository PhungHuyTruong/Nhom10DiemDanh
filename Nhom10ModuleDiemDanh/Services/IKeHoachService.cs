using Nhom10ModuleDiemDanh.Models;

namespace Nhom10ModuleDiemDanh.Services
{
    public interface IKeHoachService
    {
        Task<IEnumerable<KeHoachViewModel>> GetAllKeHoachs(string tuKhoa = "", string trangThai = "", string idBoMon = "", string idCapDoDuAn = "", string idHocKy = "", string namHoc = "");
        Task<KeHoachViewModel> GetKeHoachById(Guid id);
        Task CreateKeHoach(KeHoachViewModel model);
        Task UpdateKeHoach(KeHoachViewModel model);
        Task DeleteKeHoach(Guid id);
        Task ToggleStatus(Guid id);
    }
}
