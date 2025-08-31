using IDSApi.Models;

namespace IDSApi.Services.Interfaces
{
    public interface ITimePunchService
    {
        Task<IEnumerable<TimePunch>> GetAllAsync();
        Task<TimePunch?> GetByIdAsync(int id);
        Task<int> AddAsync(TimePunch timePunch);
        Task<bool> UpdateAsync(int id, TimePunch timePunch);
        Task<bool> DeleteAsync(int id);
    }
}