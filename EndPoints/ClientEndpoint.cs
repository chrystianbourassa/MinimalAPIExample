using IDSApi.Services.Interfaces;
namespace IDSApi.EndPoints
{
    public static class ClientEndpoint
    {
        public static void MapClientRoutes(WebApplication app)
        {
            app.MapGet("/clients/GetAllAsync", async (IClientService clientService, ILogger<Program> logger) =>
            {
                try
                {
                    return Results.Ok(await clientService.GetAllAsync());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erreur lors de la récupération des clients.");
                    return Results.Problem("Une erreur s'est produite lors de la récupération des clients.", statusCode: 500);
                }
            });

            app.MapGet("/clients/GetAllActifAsync", async (IClientService clientService, ILogger<Program> logger) =>
            {
                try
                {
                    return Results.Ok(await clientService.GetAllActifAsync());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erreur lors de la récupération des clients.");
                    return Results.Problem("Une erreur s'est produite lors de la récupération des clients.", statusCode: 500);
                }
            });

            app.MapGet("/clients/GetByIdAsync/{id:int}", async (int id, IClientService clientService, ILogger<Program> logger) =>
            {
                try
                {
                    var client = await clientService.GetByIdAsync(id);
                    return client is not null ? Results.Ok(client) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Erreur lors de la récupération du client avec l'ID {id}.");
                    return Results.Problem("Une erreur s'est produite lors de la récupération du client." + ex.Message, statusCode: 500);
                }
            });


        }
    }
}
