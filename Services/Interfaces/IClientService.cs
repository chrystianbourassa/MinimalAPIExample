// IClientService.cs
using IDSApi.Models;

namespace IDSApi.Services.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<IEnumerable<Client>> GetAllActifAsync();
        Task<Client?> GetByIdAsync(int id);

    }
}