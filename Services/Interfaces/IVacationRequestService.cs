using IDSApi.Models;

namespace IDSApi.Services.Interfaces
{
    public interface IVacationRequestService
    {
        Task<IEnumerable<VacationRequest>> GetAllAsync();
        Task<VacationRequest?> GetByIdAsync(int id);
        Task<IEnumerable<VacationRequest>> GetByChauffeurIdAsync(int chauffeurId);

        Task<int> AddAsync(VacationRequest vacationRequest);
        Task<bool> UpdateAsync(int id, VacationRequest vacationRequest);
        Task<bool> DeleteAsync(int id);
    }
}