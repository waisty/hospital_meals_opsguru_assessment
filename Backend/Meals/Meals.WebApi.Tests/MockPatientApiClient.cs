using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.WebApi.Tests;

internal sealed class MockPatientApiClient : IPatientApiClient
{
    private readonly Dictionary<string, PatientDetailViewModel> _patients = new();

    public void Clear() => _patients.Clear();

    public void SeedPatient(string id, string name, IReadOnlyList<string>? allergyIds = null, IReadOnlyList<string>? clinicalStateIds = null, string? dietTypeId = null)
    {
        _patients[id] = new PatientDetailViewModel
        {
            Id = id,
            Name = name,
            AllergyIds = allergyIds ?? [],
            ClinicalStateIds = clinicalStateIds ?? [],
            DietTypeId = dietTypeId ?? ""
        };
    }

    public Task<PatientDetailViewModel?> GetPatientDetailAsync(string patientId, CancellationToken cancellationToken = default)
    {
        _patients.TryGetValue(patientId, out var patient);
        return Task.FromResult(patient);
    }
}
