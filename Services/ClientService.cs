using IDSApi.Models;
using IDSApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IDSApi.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientService> _logger;

        public ClientService(AppDbContext context, ILogger<ClientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Client>> GetAllActifAsync()
        {
            try
            {

                return await _context.Clients
                    .FromSqlInterpolated($"exec Client_GetActif")
                    .ToListAsync();

            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de la récupération des clients actifs.");
                throw new Exception("Une erreur s'est produite lors de la récupération des clients actifs.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inconnue lors de la récupération des clients actifs.");
                throw;
            }
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            try
            {
                return await _context.Clients
                    .FromSqlInterpolated($"exec Client_Get")
                    .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de la récupération de tous les clients.");
                throw new Exception("Une erreur s'est produite lors de la récupération des données des clients.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inconnue lors de la récupération de tous les clients.");
                throw;
            }
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            try
            {
                var client = _context.Clients
                    .FromSqlInterpolated($"EXEC Client_GetById {id}")
                    .AsEnumerable()  // Force l'exécution sur le serveur avant toute manipulation
                    .FirstOrDefault();

                return client;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur inconnue lors de la récupération du client avec l'ID {id}.");
                throw;
            }
        }
    }
}