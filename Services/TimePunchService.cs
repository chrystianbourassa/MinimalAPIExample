using System.Data;
using IDSApi.Models;
using IDSApi.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IDSApi.Services
{
    public class TimePunchService : ITimePunchService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TimePunchService> _logger;

        public TimePunchService(AppDbContext context, ILogger<TimePunchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get all TimePunches
        public async Task<IEnumerable<TimePunch>> GetAllAsync()
        {
            try
            {
                return await _context.TimePunches.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les timepunchs.");
                throw new Exception("Une erreur est survenue lors de la récupération des timepunchs. Veuillez réessayer plus tard.", ex);
            }
        }

        // Get a specific TimePunch by ID
        public async Task<TimePunch?> GetByIdAsync(int id)
        {
            try
            {
                // Execute stored procedure directly
                var timePunch = await _context.TimePunches
                    .FromSqlInterpolated($"EXEC TimePunch_GetById {id}")
                    .ToListAsync()
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (timePunch == null)
                {
                    _logger.LogWarning("Aucune timepunch trouvée avec l'ID {Id}", id);
                    return null;
                }

                return timePunch;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la timepunch avec l'ID {Id}", id);
                throw new Exception("Une erreur est survenue. Veuillez contacter le support.", ex);
            }
        }

        // Insert a new TimePunch
        public async Task<int> AddAsync(TimePunch timePunch)
        {
            try
            {
                var timePunchId = new SqlParameter
                {
                    ParameterName = "@TimePunchId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var punchInParam = new SqlParameter("@PunchIn", SqlDbType.DateTime)
                {
                    Value = timePunch.PunchIn ?? (object)DBNull.Value
                };

                var punchOutParam = new SqlParameter("@PunchOut", SqlDbType.DateTime)
                {
                    Value = timePunch.PunchOut ?? (object)DBNull.Value
                };

                var punchInLocationParam = new SqlParameter("@PunchInLocation", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(timePunch.PunchInlocation)
                        ? (object)DBNull.Value
                        : timePunch.PunchInlocation
                };

                var punchOutLocationParam = new SqlParameter("@PunchOutLocation", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(timePunch.PunchOutlocation)
                        ? (object)DBNull.Value
                        : timePunch.PunchOutlocation
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC TimePunch_Add @ChauffeurId, @PunchIn, @PunchInLocation, @PunchOut, @PunchOutLocation, @TimePunchId OUTPUT",
                    new SqlParameter("@ChauffeurId", timePunch.ChauffeurId),
                    punchInParam,
                    punchInLocationParam,
                    punchOutParam,
                    punchOutLocationParam,
                    timePunchId
                );

                var newId = (int)timePunchId.Value;
                timePunch.TimePunchId = newId;
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout d'un nouveau timepunch.");
                throw new Exception("Impossible d'ajouter ce timepunch. Veuillez réessayer plus tard.", ex);
            }
        }
        // Update an existing TimePunch
        public async Task<bool> UpdateAsync(int id, TimePunch timePunch)
        {
            try
            {
                var punchInParam = new SqlParameter("@PunchIn", SqlDbType.DateTime)
                {
                    Value = timePunch.PunchIn ?? (object)DBNull.Value
                };

                var punchOutParam = new SqlParameter("@PunchOut", SqlDbType.DateTime)
                {
                    Value = timePunch.PunchOut ?? (object)DBNull.Value
                };

                var punchInLocationParam = new SqlParameter("@PunchInLocation", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(timePunch.PunchInlocation)
                        ? (object)DBNull.Value
                        : timePunch.PunchInlocation
                };

                var punchOutLocationParam = new SqlParameter("@PunchOutLocation", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(timePunch.PunchOutlocation)
                        ? (object)DBNull.Value
                        : timePunch.PunchOutlocation
                };

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC TimePunch_Update @TimePunchId, @ChauffeurId, @PunchIn, @PunchInLocation, @PunchOut, @PunchOutLocation",
                    new SqlParameter("@TimePunchId", id),
                    new SqlParameter("@ChauffeurId", timePunch.ChauffeurId),
                    punchInParam,
                    punchInLocationParam,
                    punchOutParam,
                    punchOutLocationParam
                );

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("Aucune mise à jour effectuée pour la timepunch avec l'ID {Id}", id);
                    throw new KeyNotFoundException($"Aucune timepunch trouvée avec l'ID {id}.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la timepunch avec l'ID {Id}", id);
                throw new Exception("Impossible de mettre à jour la timepunch. Veuillez réessayer plus tard.", ex);
            }
        }

        // Delete a TimePunch by ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC TimePunch_Delete {id}");

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("Aucune TimePunch supprimée avec l'ID {Id}", id);
                    throw new KeyNotFoundException($"Aucune TimePunch trouvée avec l'ID {id}.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la TimePunch avec l'ID {Id}", id);
                throw new Exception("Impossible de supprimer la TimePunch. Veuillez contacter le support.", ex);
            }
        }
    }
}