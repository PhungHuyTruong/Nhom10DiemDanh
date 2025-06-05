using Nhom10ModuleDiemDanh.Models;

namespace Nhom10ModuleDiemDanh.Services
{
    public interface ICoSoService
    {
        Task<List<CoSoViewModel>> GetCoSosAsync(string tenCoSo, string trangThai);
        Task<CoSoViewModel> GetCoSoAsync(Guid id);
        Task CreateCoSoAsync(CoSoViewModel model);
        Task UpdateCoSoAsync(Guid id, CoSoViewModel model);
        Task DeleteCoSoAsync(Guid id);
        Task ToggleStatusAsync(Guid id);
    }
}
