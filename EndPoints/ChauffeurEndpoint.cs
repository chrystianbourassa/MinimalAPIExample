using IDSApi.Models;
using IDSApi.Services.Interfaces;

namespace IDSApi.EndPoints
{
    public static class ChauffeurEndpoint
    {
        public static void MapChauffeurRoutes(WebApplication app)
        {
            app.MapPost("/chauffeurs/authenticate", async (IChauffeurService service, string username, string password) =>
            {
                try
                {
                    var chauffeur = await service.AuthenticateChauffeur(username, password);
                    if (chauffeur == null)
                        return Results.NotFound(new { message = "Authentification échouée. Chauffeur non trouvé ou identifiants invalides." });

                    return Results.Ok(chauffeur);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            app.MapGet("/chauffeurs/GetAllAsync", async (IChauffeurService service) =>
            {
                try
                {
                    var chauffeurs = await service.GetAllAsync();
                    return Results.Ok(chauffeurs);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            app.MapGet("/chauffeurs/GetByIdAsync/{id:int}", async (IChauffeurService service, int id) =>
            {
                try
                {
                    var chauffeur = await service.GetByIdAsync(id);
                    if (chauffeur == null)
                        return Results.NotFound(new { message = "Chauffeur introuvable avec l'identifiant fourni." });

                    return Results.Ok(chauffeur);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            app.MapPost("/chauffeurs/AddAsync", async (IChauffeurService service, Chauffeur chauffeur) =>
            {
                try
                {
                    var newChauffeur = await service.AddAsync(chauffeur);
                    return Results.Created($"/chauffeurs/{newChauffeur.ChauffeurId}", newChauffeur);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            app.MapPut("/chauffeurs/UpdateAsync/{id:int}", async (IChauffeurService service, int id, Chauffeur chauffeur) =>
            {
                try
                {
                    var updatedChauffeur = await service.UpdateAsync(id, chauffeur);
                    if (updatedChauffeur == null)
                        return Results.NotFound(new { message = "Impossible de mettre à jour. Chauffeur introuvable." });

                    return Results.Ok(updatedChauffeur);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            app.MapDelete("/chauffeurs/DeleteAsync{id:int}", async (IChauffeurService service, int id) =>
            {
                try
                {
                    var success = await service.DeleteAsync(id);
                    if (!success)
                        return Results.NotFound(new { message = "Impossible de supprimer. Chauffeur introuvable." });

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

        }
    }
}
