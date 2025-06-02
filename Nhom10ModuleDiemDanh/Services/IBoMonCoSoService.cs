using API.Data;
using Nhom10ModuleDiemDanh.Models;

namespace Nhom10ModuleDiemDanh.Services
{
    public interface IBoMonCoSoService
    {
        Task<List<BoMonCoSoViewModel>> GetAllAsync(string tenBoMon = null, Guid? idCoSo = null);
        Task<BoMonCoSoViewModel> GetByIdAsync(Guid id);
        Task<BoMonCoSoViewModel> CreateAsync(BoMonCoSoViewModel model);
        Task UpdateAsync(Guid id, BoMonCoSoViewModel model);
        Task DeleteAsync(Guid id);
        Task<List<CoSo>> GetCoSosAsync();
        Task<List<QuanLyBoMon>> GetBoMonsAsync();
    }
}
