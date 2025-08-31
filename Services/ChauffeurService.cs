using IDSApi.Models;
using IDSApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IDSApi.Services
{
    public class ChauffeurService : IChauffeurService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChauffeurService> _logger;

        public ChauffeurService(AppDbContext context, ILogger<ChauffeurService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Chauffeur?> AuthenticateChauffeur(string username, string password)
        {
            try
            {
                var chauffeur = await _context.Chauffeurs
                    .Where(c => c.Username == username && c.Password == password)
                    .Select(c => new Chauffeur
                    {
                        ChauffeurId = c.ChauffeurId,
                        Nom = c.Nom,
                        Email = c.Email,
                        Online = c.Online,
                        PhotoUrl = c.PhotoUrl,
                        Actif = c.Actif,
                        Username = c.Username,
                        Password = c.Password
                    })
                    .FirstOrDefaultAsync();

                return chauffeur;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'authentification du chauffeur avec username: {Username}",
                    username);
                throw;
            }
        }

        public async Task<IEnumerable<Chauffeur>> GetAllAsync()
        {
            try
            {
                return await _context.Chauffeurs.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les chauffeurs.");
                throw;
            }
        }

        public async Task<Chauffeur?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Chauffeurs.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du chauffeur avec ID: {Id}", id);
                throw;
            }
        }

        public async Task<Chauffeur> AddAsync(Chauffeur chauffeur)
        {
            try
            {
                _context.Chauffeurs.Add(chauffeur);
                await _context.SaveChangesAsync();
                return chauffeur;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de l'ajout du chauffeur: {Chauffeur}", chauffeur);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de l'ajout du chauffeur: {Chauffeur}", chauffeur);
                throw;
            }
        }

        public async Task<Chauffeur?> UpdateAsync(int id, Chauffeur chauffeur)
        {
            try
            {
                var existing = await _context.Chauffeurs.FindAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Aucun chauffeur trouvé avec ID: {Id}", id);
                    return null;
                }

                _context.Entry(existing).CurrentValues.SetValues(chauffeur);
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de la mise à jour du chauffeur avec ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la mise à jour du chauffeur avec ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existing = await _context.Chauffeurs.FindAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Aucun chauffeur trouvé avec ID: {Id}", id);
                    return false;
                }

                _context.Chauffeurs.Remove(existing);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de la suppression du chauffeur avec ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la suppression du chauffeur avec ID: {Id}", id);
                throw;
            }
        }
    }
}