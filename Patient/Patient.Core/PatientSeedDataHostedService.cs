using Hospital.Patient.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using PatientEntity = Hospital.Patient.Core.InternalModels.Patient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Patient.Core.Implementation
{
    /// <summary>
    /// Seeds initial reference data (diet types, allergies, clinical states) and sample patients
    /// when the database is empty or when SeedData:Enabled is true.
    /// Duplicate or failed inserts are logged as warnings and do not stop seeding.
    /// </summary>
    internal sealed class PatientSeedDataHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PatientSeedDataHostedService> _logger;

        public PatientSeedDataHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<PatientSeedDataHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var enabledByConfig = string.Equals(
                _configuration["SeedData:Enabled"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<PatientDBContext>();

            bool databaseEmpty = false;
            try
            {
                databaseEmpty = await db.DietTypes.CountAsync(cancellationToken).ConfigureAwait(false) == 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not check if patient database is empty; skipping seed.");
                return;
            }

            if (!enabledByConfig && !databaseEmpty)
            {
                _logger.LogDebug("Patient seed data skipped: database has data and SeedData:Enabled is not set.");
                return;
            }

            await SeedDietTypesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedAllergiesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedClinicalStatesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedPatientsAsync(db, cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task SeedDietTypesAsync(PatientDBContext db, CancellationToken cancellationToken)
        {
            var seedDietTypes = new[]
            {
                new DietType { Id = "REGULAR", Name = "Regular" },
                new DietType { Id = "VEGETARIAN", Name = "Vegetarian" },
                new DietType { Id = "DIABETIC", Name = "Diabetic" },
                new DietType { Id = "LOW-SODIUM", Name = "Low Sodium" },
            };

            foreach (var dietType in seedDietTypes)
            {
                try
                {
                    db.DietTypes.Add(dietType);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed diet type added: {Id} ({Name})", dietType.Id, dietType.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for diet type {Id}; continuing with next.", dietType.Id);
                }
            }
        }

        private async Task SeedAllergiesAsync(PatientDBContext db, CancellationToken cancellationToken)
        {
            var seedAllergies = new[]
            {
                new Allergy { Id = "NUTS", Name = "Tree Nuts" },
                new Allergy { Id = "DAIRY", Name = "Dairy" },
                new Allergy { Id = "GLUTEN", Name = "Gluten" },
                new Allergy { Id = "SHELLFISH", Name = "Shellfish" },
            };

            foreach (var allergy in seedAllergies)
            {
                try
                {
                    db.Allergies.Add(allergy);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed allergy added: {Id} ({Name})", allergy.Id, allergy.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for allergy {Id}; continuing with next.", allergy.Id);
                }
            }
        }

        private async Task SeedClinicalStatesAsync(PatientDBContext db, CancellationToken cancellationToken)
        {
            var seedClinicalStates = new[]
            {
                new ClinicalState { Id = "DIABETIC", Name = "Diabetic" },
                new ClinicalState { Id = "HYPERTENSION", Name = "Hypertension" },
                new ClinicalState { Id = "CARDIAC", Name = "Cardiac" },
                new ClinicalState { Id = "RENAL", Name = "Renal" },
            };

            foreach (var clinicalState in seedClinicalStates)
            {
                try
                {
                    db.ClinicalStates.Add(clinicalState);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed clinical state added: {Id} ({Name})", clinicalState.Id, clinicalState.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for clinical state {Id}; continuing with next.", clinicalState.Id);
                }
            }
        }

        private async Task SeedPatientsAsync(PatientDBContext db, CancellationToken cancellationToken)
        {
            var seedPatients = new[]
            {
                (Name: "John Doe", MobileNumber: "+15551234001", DietTypeId: "REGULAR", Notes: "Sample patient 1"),
                (Name: "Jane Smith", MobileNumber: "+15551234002", DietTypeId: "VEGETARIAN", Notes: "Sample patient 2"),
            };

            foreach (var (name, mobileNumber, dietTypeId, notes) in seedPatients)
            {
                try
                {
                    var patient = new PatientEntity
                    {
                        Name = name,
                        MobileNumber = mobileNumber,
                        DietTypeId = dietTypeId,
                        Notes = notes ?? ""
                    };
                    db.Patients.Add(patient);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed patient added: {Name} (Id: {Id})", name, patient.Id);

                    // Link first patient to some allergies and clinical states
                    if (name == "John Doe")
                    {
                        await SeedPatientAllergiesAsync(db, patient.Id, new[] { "NUTS" }, cancellationToken).ConfigureAwait(false);
                        await SeedPatientClinicalStatesAsync(db, patient.Id, new[] { "HYPERTENSION" }, cancellationToken).ConfigureAwait(false);
                    }
                    else if (name == "Jane Smith")
                    {
                        await SeedPatientAllergiesAsync(db, patient.Id, new[] { "DAIRY", "GLUTEN" }, cancellationToken).ConfigureAwait(false);
                        await SeedPatientClinicalStatesAsync(db, patient.Id, new[] { "DIABETIC" }, cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for patient {Name}; continuing with next.", name);
                }
            }
        }

        private async Task SeedPatientAllergiesAsync(PatientDBContext db, Guid patientId, string[] allergyIds, CancellationToken cancellationToken)
        {
            foreach (var allergyId in allergyIds)
            {
                try
                {
                    db.PatientAllergies.Add(new PatientAllergy { PatientId = patientId, AllergyId = allergyId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed patient_allergy failed for patient {PatientId}, allergy {AllergyId}.", patientId, allergyId);
                }
            }
        }

        private async Task SeedPatientClinicalStatesAsync(PatientDBContext db, Guid patientId, string[] clinicalStateIds, CancellationToken cancellationToken)
        {
            foreach (var clinicalStateId in clinicalStateIds)
            {
                try
                {
                    db.PatientClinicalStates.Add(new PatientClinicalState { PatientId = patientId, ClinicalStateId = clinicalStateId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed patient_clinical_state failed for patient {PatientId}, clinical state {ClinicalStateId}.", patientId, clinicalStateId);
                }
            }
        }
    }
}
