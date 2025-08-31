using IDSApi;
using IDSApi.EndPoints;
using IDSApi.Services;
using IDSApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Services.AddLogging();

// Add DbContext with proper configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("IDSWebConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'IDSWebConnection' not found.");
    }
    options.UseSqlServer(connectionString);
}, ServiceLifetime.Scoped); // Changed from Transient to Scoped

// Register Services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IChauffeurService, ChauffeurService>();
builder.Services.AddScoped<ITimePunchService, TimePunchService>();
builder.Services.AddScoped<IVacationRequestService, VacationRequestService>();

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IDS API", Version = "v1" });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IDS API v1");
    c.RoutePrefix = string.Empty; // This makes Swagger UI available at the root
});

// Use CORS
app.UseCors("AllowAllOrigins");

// Use HTTPS redirection
app.UseHttpsRedirection();

// Map endpoints
ClientEndpoint.MapClientRoutes(app);
ChauffeurEndpoint.MapChauffeurRoutes(app);
TimePunchEndpoint.MapTimePunchRoutes(app);
VacationRequestEndpoint.MapVacationRequestRoutes(app);

// Add a simple health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();