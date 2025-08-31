using IDSApi.Models;
using IDSApi.Services;
using IDSApi.Services.Interfaces;

namespace IDSApi.EndPoints
{
    public static class TimePunchEndpoint
    {
        public static void MapTimePunchRoutes(WebApplication app)
        {
            app.MapGet("/timepunches/GetAllAsync",
                async (ITimePunchService service, ILogger<TimePunchService> logger) =>
                {
                    try
                    {
                        var timepunches = await service.GetAllAsync();
                        return Results.Ok(timepunches);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erreur lors de la récupération de toutes les timepunches." + ex.Message);
                        return Results.Problem(
                            "Une erreur inattendue est survenue lors de la récupération des timepunches." + ex.Message,
                            statusCode: 500);
                    }
                });

            // Endpoint pour récupérer une timepunch par son ID
            app.MapGet("/timepunches/GetByIdAsync/{id}",
                    async (int id, ITimePunchService service, ILogger<TimePunchService> logger) =>
                    {
                        try
                        {
                            var timePunch = await service.GetByIdAsync(id);
                            return timePunch is not null
                                ? Results.Ok(timePunch)
                                : Results.NotFound($"Aucune timepunch trouvée avec l'ID {id}.");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Erreur lors de la récupération de la timepunch avec l'ID {Id}", id);
                            return Results.Problem(
                                detail: "Une erreur inattendue est survenue lors de la récupération de la timepunch.",
                                statusCode: StatusCodes.Status500InternalServerError
                            );
                        }
                    })
                .WithName("GetTimePunchById")
                .WithOpenApi();

            // Endpoint pour ajouter une nouvelle timepunch
            app.MapPost("/timepunches/AddAsync",
                    async (TimePunch timePunch, ITimePunchService service, ILogger<TimePunchService> logger) =>
                    {
                        try
                        {
                            var newId = await service.AddAsync(timePunch);
                            var addedTimePunch = await service.GetByIdAsync(newId);
                            if (addedTimePunch == null)
                            {
                                logger.LogWarning("Timepunch ajouté avec l'ID {Id} mais non retrouvé après ajout.", newId);
                                return Results.Problem(
                                    detail: "Timepunch ajouté mais non retrouvé.",
                                    statusCode: StatusCodes.Status500InternalServerError
                                );
                            }

                            logger.LogInformation("Timepunch ajouté avec succès avec l'ID {Id}.", newId);
                            return Results.Created($"/timepunches/{addedTimePunch.TimePunchId}", addedTimePunch);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Erreur lors de l'ajout du timepunch pour ChauffeurId {ChauffeurId}.", timePunch.ChauffeurId);
                            return Results.Problem(
                                detail: "Une erreur inattendue est survenue lors de l'ajout du timepunch.",
                                statusCode: StatusCodes.Status500InternalServerError
                            );
                        }
                    })
                .WithName("AddTimePunch")
                .WithOpenApi()
                .Produces<TimePunch>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status500InternalServerError);

            // Endpoint pour mettre à jour une timepunch existante
            app.MapPut("/timepunches/UpdateAsync/{id}", async (int id, TimePunch TimePunch,
                ITimePunchService service, ILogger<TimePunchService> logger) =>
            {
                try
                {
                    var updatedTimePunch = await service.UpdateAsync(id, TimePunch);
                    return updatedTimePunch == true
                        ? Results.Ok(updatedTimePunch)
                        : Results.NotFound($"Aucun timepunch trouvée avec l'ID {id}.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Erreur lors de la mise à jour du timepunch avec l'ID {id}." + ex.Message);
                    return Results.Problem(
                        $"Une erreur inattendue est survenue lors de la mise à jour du timepunch avec l'ID {id}." + ex.Message,
                        statusCode: 500);
                }
            });

            // Endpoint pour supprimer une timepunch
            app.MapDelete("/timepunches/DeleteAsync/{id}",
                async (int id, ITimePunchService service, ILogger<TimePunchService> logger) =>
                {
                    try
                    {
                        var success = await service.DeleteAsync(id);
                        return success
                            ? Results.NoContent()
                            : Results.NotFound($"Aucun timepunch trouvée avec l'ID {id}.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Erreur lors de la suppression du timepunch avec l'ID {id}." + ex.Message);
                        return Results.Problem(
                            $"Une erreur inattendue est survenue lors de la suppression de la timepunch avec l'ID {id}." + ex.Message,
                            statusCode: 500);
                    }
                });

        }
    }
}