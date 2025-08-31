// IChauffeurService.cs
using IDSApi.Models;

namespace IDSApi.Services.Interfaces
{
    public interface IChauffeurService
    {
        Task<IEnumerable<Chauffeur>> GetAllAsync();
        Task<Chauffeur?> AuthenticateChauffeur(string username, string password);

        Task<Chauffeur?> GetByIdAsync(int id);
        Task<Chauffeur> AddAsync(Chauffeur Chauffeur);
        Task<Chauffeur?> UpdateAsync(int id, Chauffeur Chauffeur);
        Task<bool> DeleteAsync(int id);
    }
}