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
    /// Uses deterministic GUIDs for seeded patients so Meals/Kitchen can reference the same IDs.
    /// Duplicate or failed inserts are logged as warnings and do not stop seeding.
    /// </summary>
    internal sealed class PatientSeedDataHostedService : IHostedService
    {
        /// <summary>Deterministic patient count; same constant used by Meals seed for matching PatientRequests.</summary>
        internal const int SeedPatientCount = 1000;

        private static readonly string[] FirstNames =
        {
            "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
            "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen",
            "Christopher", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra",
            "Donald", "Ashley", "Steven", "Kimberly", "Paul", "Emily", "Andrew", "Donna", "Joshua", "Michelle",
            "Kenneth", "Dorothy", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa", "Timothy", "Deborah",
        };

        private static readonly string[] LastNames =
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
            "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
            "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
        };

        private static readonly string[] DietTypeIds = { "REGULAR", "VEGETARIAN", "DIABETIC", "LOW-SODIUM" };
        private static readonly string[] AllergyIds = { "NUTS", "DAIRY", "GLUTEN", "SHELLFISH" };
        private static readonly string[] ClinicalStateIds = { "DIABETIC", "HYPERTENSION", "CARDIAC", "RENAL" };

        /// <summary>Creates a deterministic GUID for seed patient index so Meals/Kitchen can reference the same IDs.</summary>
        internal static Guid CreateSeedPatientId(int index)
        {
            var bytes = new byte[16];
            bytes[15] = (byte)(index & 0xFF);
            bytes[14] = (byte)((index >> 8) & 0xFF);
            bytes[13] = (byte)((index >> 16) & 0xFF);
            bytes[12] = (byte)((index >> 24) & 0xFF);
            return new Guid(bytes);
        }

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
            const int batchSize = 100;
            for (var batchStart = 0; batchStart < SeedPatientCount; batchStart += batchSize)
            {
                var batchEnd = Math.Min(batchStart + batchSize, SeedPatientCount);
                for (var i = batchStart; i < batchEnd; i++)
                {
                    var patientId = CreateSeedPatientId(i);
                    var firstName = FirstNames[i % FirstNames.Length];
                    var lastName = LastNames[i % LastNames.Length];
                    var middleIndex = (i / FirstNames.Length) % LastNames.Length;
                    var middleName = middleIndex == 0 ? "" : LastNames[middleIndex];
                    var dietTypeId = DietTypeIds[i % DietTypeIds.Length];
                    var mobileNumber = $"+1555{i + 1000000:D7}";
                    var patient = new PatientEntity
                    {
                        Id = patientId,
                        FirstName = firstName,
                        MiddleName = middleName ?? "",
                        LastName = lastName,
                        MobileNumber = mobileNumber,
                        DietTypeId = dietTypeId,
                        Notes = $"Seed patient {i + 1}"
                    };
                    db.Patients.Add(patient);
                }

                try
                {
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed patients added: batch {Start}-{End} of {Total}", batchStart + 1, batchEnd, SeedPatientCount);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed patient batch {Start}-{End} failed; continuing.", batchStart + 1, batchEnd);
                    continue;
                }

                // Every third patient (3rd, 6th, 9th, ...) gets 1, 2 or 3 allergies and 1, 2 or 3 clinical states.
                for (var i = batchStart; i < batchEnd; i++)
                {
                    var patientId = CreateSeedPatientId(i);
                    var isEveryThird = (i + 1) % 3 == 0; // 1-based: patient 3, 6, 9, ...
                    var allergyCount = isEveryThird ? 1 + (i / 3) % 3 : 0;   // 1, 2 or 3
                    var clinicalStateCount = isEveryThird ? 1 + (i / 3) % 3 : 0;

                    for (var a = 0; a < allergyCount; a++)
                    {
                        var allergyId = AllergyIds[(i + a) % AllergyIds.Length];
                        try
                        {
                            db.PatientAllergies.Add(new PatientAllergy { PatientId = patientId, AllergyId = allergyId });
                            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Seed patient_allergy failed for patient index {Index}, allergy {AllergyId}.", i, allergyId);
                        }
                    }

                    for (var c = 0; c < clinicalStateCount; c++)
                    {
                        var clinicalStateId = ClinicalStateIds[(i + c) % ClinicalStateIds.Length];
                        try
                        {
                            db.PatientClinicalStates.Add(new PatientClinicalState { PatientId = patientId, ClinicalStateId = clinicalStateId });
                            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Seed patient_clinical_state failed for patient index {Index}, clinical state {ClinicalStateId}.", i, clinicalStateId);
                        }
                    }
                }
            }

            _logger.LogInformation("Seed patients completed: {Count} patients with associated allergies and clinical states.", SeedPatientCount);
        }
    }
}
