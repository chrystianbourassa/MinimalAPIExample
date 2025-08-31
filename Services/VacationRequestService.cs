using System.Data;
using IDSApi.Models;
using IDSApi.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IDSApi.Services
{
    public class VacationRequestService : IVacationRequestService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VacationRequestService> _logger;

        public VacationRequestService(AppDbContext context, ILogger<VacationRequestService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get all VacationRequests
        public async Task<IEnumerable<VacationRequest>> GetAllAsync()
        {
            try
            {
                return await _context.VacationRequests.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les demandes de congé.");
                throw new Exception("Une erreur est survenue lors de la récupération des demandes de congé. Veuillez réessayer plus tard.", ex);
            }
        }

        // Get a specific VacationRequest by ChauffeurID
        public async Task<IEnumerable<VacationRequest>> GetByChauffeurIdAsync(int chauffeurId)
        {
            try
            {
                var vacationRequests = await _context.VacationRequests
                    .FromSqlInterpolated($"EXEC VacationRequest_GetByChauffeurId {chauffeurId}")
                    .ToListAsync();

                if (!vacationRequests.Any())
                {
                    _logger.LogWarning("Aucune demande de congé trouvée avec le ChauffeurId {chauffeurId}", chauffeurId);
                }

                return vacationRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes de congé pour le ChauffeurId {chauffeurId}", chauffeurId);
                throw new Exception("Une erreur est survenue. Veuillez contacter le support.", ex);
            }
        }

        // Get a specific VacationRequest by ID
        public async Task<VacationRequest?> GetByIdAsync(int id)
        {
            try
            {
                var vacationRequest = await _context.VacationRequests
                    .FromSqlInterpolated($"EXEC VacationRequest_GetById {id}")
                    .ToListAsync()
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (vacationRequest == null)
                {
                    _logger.LogWarning("Aucune demande de congé trouvée avec l'ID {Id}", id);
                    return null;
                }

                return vacationRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la demande de congé avec l'ID {Id}", id);
                throw new Exception("Une erreur est survenue. Veuillez contacter le support.", ex);
            }
        }

        // Insert a new VacationRequest



        public async Task<int> AddAsync(VacationRequest vacationRequest)
        {
            try
            {
                var vacationRequestId = new SqlParameter
                {
                    ParameterName = "@VacationRequestId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC VacationRequest_Add @StartDate, @EndDate, @Reason, @ChauffeurId, @IsApproved, @VacationRequestId OUTPUT",
                    new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = vacationRequest.StartDate },
                    new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = vacationRequest.EndDate },
                    new SqlParameter("@Reason", SqlDbType.NVarChar) { Value = vacationRequest.Reason ?? (object)DBNull.Value },
                    new SqlParameter("@ChauffeurId", SqlDbType.Int) { Value = vacationRequest.ChauffeurId },
                    new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = vacationRequest.IsApproved },
                    vacationRequestId
                );


                var newId = (int)vacationRequestId.Value;
                vacationRequest.VacationRequestId = newId;
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout d'une nouvelle demande de congé.");
                throw new Exception("Impossible d'ajouter cette demande de congé. Veuillez réessayer plus tard.", ex);
            }
        }

        // Update an existing VacationRequest
        public async Task<bool> UpdateAsync(int id, VacationRequest vacationRequest)
        {
            try
            {
                // Validate input
                if (vacationRequest == null)
                {
                    _logger.LogWarning("VacationRequest is null for ID {Id}", id);
                    throw new ArgumentNullException(nameof(vacationRequest));
                }

                // Execute the update stored procedure
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC VacationRequest_Update @VacationRequestId, @StartDate, @EndDate, @Reason, @ChauffeurId, @IsApproved",
                    new SqlParameter("@VacationRequestId", id),
                    new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = vacationRequest.StartDate },
                    new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = vacationRequest.EndDate },
                    new SqlParameter("@Reason", SqlDbType.NVarChar) { Value = vacationRequest.Reason ?? (object)DBNull.Value },
                    new SqlParameter("@ChauffeurId", SqlDbType.Int) { Value = vacationRequest.ChauffeurId },
                    new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = vacationRequest.IsApproved }
                );

                // Check if the update affected any rows
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No update performed for VacationRequest with ID {Id}", id);
                    throw new KeyNotFoundException($"No VacationRequest found with ID {id}.");
                }

                _logger.LogInformation("Successfully updated VacationRequest with ID {Id}", id);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while updating VacationRequest with ID {Id}", id);
                throw new Exception("Failed to update the VacationRequest due to a database error. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating VacationRequest with ID {Id}", id);
                throw new Exception("Failed to update the VacationRequest. Please try again later.", ex);
            }
        }
        // Delete a VacationRequest by ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC VacationRequest_Delete {id}");

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("Aucune demande de congé supprimée avec l'ID {Id}", id);
                    throw new KeyNotFoundException($"Aucune demande de congé trouvée avec l'ID {id}.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la demande de congé avec l'ID {Id}", id);
                throw new Exception("Impossible de supprimer la demande de congé. Veuillez contacter le support.", ex);
            }
        }
    }
}