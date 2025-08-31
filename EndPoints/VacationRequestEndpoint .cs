using IDSApi.Models;
using IDSApi.Services;
using IDSApi.Services.Interfaces;

namespace IDSApi.EndPoints
{
    public static class VacationRequestEndpoint
    {
        public static void MapVacationRequestRoutes(WebApplication app)
        {
            // Get all vacation requests
            app.MapGet("/vacationrequests/GetAllAsync",
                async (IVacationRequestService service, ILogger<VacationRequest> logger) =>
                {
                    try
                    {
                        var vacationRequests = await service.GetAllAsync();
                        return Results.Ok(vacationRequests);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erreur lors de la récupération de toutes les demandes de congé." + ex.Message);
                        return Results.Problem("Une erreur inattendue est survenue lors de la récupération des demandes de congé." + ex.Message, statusCode: 500);
                    }
                });

            // Get vacation request by ID
            app.MapGet("/vacationrequests/GetByIdAsync/{id}",
                async (int id, IVacationRequestService service, ILogger<VacationRequest> logger) =>
                {
                    try
                    {
                        var vacationRequest = await service.GetByIdAsync(id);
                        return vacationRequest is not null
                            ? Results.Ok(vacationRequest)
                            : Results.NotFound($"Aucune demande de congé trouvée avec l'ID {id}.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erreur lors de la récupération de la demande de congé avec l'ID {Id}", id);
                        return Results.Problem("Une erreur inattendue est survenue lors de la récupération de la demande de congé.", statusCode: 500);
                    }
                })
                .WithName("GetVacationRequestById")
                .WithOpenApi();

            // Get vacation requests by ChauffeurId
            app.MapGet("/vacationrequests/GetByChauffeurIdAsync/{chauffeurId}",
                async (int chauffeurId, IVacationRequestService service, ILogger<VacationRequest> logger) =>
                {
                    try
                    {
                        var vacationRequests = await service.GetByChauffeurIdAsync(chauffeurId);
                        return vacationRequests.Any()
                            ? Results.Ok(vacationRequests)
                            : Results.NotFound($"Aucune demande de congé trouvée pour le chauffeur ID {chauffeurId}.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erreur lors de la récupération des demandes de congé pour le chauffeur ID {ChauffeurId}", chauffeurId);
                        return Results.Problem("Une erreur inattendue est survenue lors de la récupération des demandes de congé.", statusCode: 500);
                    }
                })
                .WithName("GetVacationRequestsByChauffeurId")
                .WithOpenApi();

            // Add a vacation request
            app.MapPost("/vacationrequests/AddAsync",
                    async (VacationRequest vacationRequest, IVacationRequestService service, ILogger<VacationRequestService> logger) =>
                    {
                        try
                        {
                            var newId = await service.AddAsync(vacationRequest);
                            var addedvacationRequest = await service.GetByIdAsync(newId);
                            if (addedvacationRequest == null)
                            {
                                logger.LogWarning("VacationRequest ajouté avec l'ID {Id} mais non retrouvé après ajout.", newId);
                                return Results.Problem(
                                    detail: "VacationRequest ajouté mais non retrouvé.",
                                    statusCode: StatusCodes.Status500InternalServerError
                                );
                            }

                            logger.LogInformation("VacationRequest ajouté avec succès avec l'ID {Id}.", newId);
                            return Results.Created($"/VacationRequestes/{addedvacationRequest.VacationRequestId}", addedvacationRequest);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Erreur lors de l'ajout du VacationRequest pour ChauffeurId {ChauffeurId}." + ex.Message, vacationRequest.ChauffeurId);
                            return Results.Problem(
                                detail: "Une erreur inattendue est survenue lors de l'ajout du VacationRequest." + ex.Message,
                                statusCode: StatusCodes.Status500InternalServerError
                            );
                        }
                    })
                .WithName("AddVacationRequest")
                .WithOpenApi()
                .Produces<VacationRequest>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status500InternalServerError);
            // Update a vacation request
            app.MapPut("/vacationrequests/UpdateAsync/{id}",
                async (int id, VacationRequest request, IVacationRequestService service, ILogger<VacationRequest> logger) =>
                {
                    try
                    {
                        var updated = await service.UpdateAsync(id, request);
                        return updated
                            ? Results.Ok(updated)
                            : Results.NotFound($"Aucune demande de congé trouvée avec l'ID {id}.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Erreur lors de la mise à jour de la demande de congé avec l'ID {id}." + ex.Message);
                        return Results.Problem($"Une erreur inattendue est survenue lors de la mise à jour de la demande de congé." + ex.Message, statusCode: 500);
                    }
                });

            // Delete a vacation request
            app.MapDelete("/vacationrequests/DeleteAsync/{id}",
                async (int id, IVacationRequestService service, ILogger<VacationRequest> logger) =>
                {
                    try
                    {
                        var deleted = await service.DeleteAsync(id);
                        return deleted
                            ? Results.NoContent()
                            : Results.NotFound($"Aucune demande de congé trouvée avec l'ID {id}.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Erreur lors de la suppression de la demande de congé avec l'ID {id}." + ex.Message);
                        return Results.Problem($"Une erreur inattendue est survenue lors de la suppression de la demande de congé." + ex.Message, statusCode: 500);
                    }
                });
        }
    }
}
