using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using InternalModels = Patient.Core.InternalModels;
using PatientEntity = Patient.Core.InternalModels.Patient;

namespace Patient.Core.Implementation
{
    internal sealed class PatientDBContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public PatientDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<PatientEntity> Patients { get; set; }
        public DbSet<InternalModels.DietType> DietTypes { get; set; }
        public DbSet<InternalModels.Allergy> Allergies { get; set; }
        public DbSet<InternalModels.ClinicalState> ClinicalStates { get; set; }
        public DbSet<InternalModels.PatientAllergy> PatientAllergies { get; set; }
        public DbSet<InternalModels.PatientClinicalState> PatientClinicalStates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration["PatientConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<PatientEntity>(PatientEntity.Configure);
            modelBuilder.Entity<InternalModels.DietType>(InternalModels.DietType.Configure);
            modelBuilder.Entity<InternalModels.Allergy>(InternalModels.Allergy.Configure);
            modelBuilder.Entity<InternalModels.ClinicalState>(InternalModels.ClinicalState.Configure);
            modelBuilder.Entity<InternalModels.PatientAllergy>(InternalModels.PatientAllergy.Configure);
            modelBuilder.Entity<InternalModels.PatientClinicalState>(InternalModels.PatientClinicalState.Configure);
        }
    }
}
