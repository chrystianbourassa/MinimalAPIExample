using Microsoft.EntityFrameworkCore;
using IDSApi.Models;

namespace IDSApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Chauffeur> Chauffeurs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<TimePunch> TimePunches { get; set; }
        public DbSet<VacationRequest> VacationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Chauffeur entity
            modelBuilder.Entity<Chauffeur>(entity =>
            {
                entity.HasKey(e => e.ChauffeurId);
                entity.Property(e => e.ChauffeurId).ValueGeneratedOnAdd();
            });

            // Configure Client entity
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId);
                entity.Property(e => e.ClientId).ValueGeneratedOnAdd();
            });

            // Configure TimePunch entity
            modelBuilder.Entity<TimePunch>(entity =>
            {
                entity.HasKey(e => e.TimePunchId);
                entity.Property(e => e.TimePunchId).ValueGeneratedOnAdd();
            });

            // Configure VacationRequest entity
            modelBuilder.Entity<VacationRequest>(entity =>
            {
                entity.HasKey(e => e.VacationRequestId);
                entity.Property(e => e.VacationRequestId).ValueGeneratedOnAdd();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}