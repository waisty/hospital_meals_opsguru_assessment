using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Hospital.Patient.ServiceViewModels;

namespace Hospital.Patient.WebApi.Tests;

public sealed class MockPatientHandler : IPatientHandler, IAllergyHandler, IClinicalStateHandler, IDietTypeHandler
{
    private readonly Dictionary<Guid, PatientViewModel> _patients = new();
    private readonly Dictionary<string, AllergyViewModel> _allergies = new();
    private readonly Dictionary<string, ClinicalStateViewModel> _clinicalStates = new();
    private readonly Dictionary<string, DietTypeViewModel> _dietTypes = new();
    private readonly Dictionary<Guid, List<string>> _patientAllergies = new();
    private readonly Dictionary<Guid, List<string>> _patientClinicalStates = new();

    public void Clear()
    {
        _patients.Clear();
        _allergies.Clear();
        _clinicalStates.Clear();
        _dietTypes.Clear();
        _patientAllergies.Clear();
        _patientClinicalStates.Clear();
    }

    public void SeedPatient(Guid id, string firstName, string lastName, string mobileNumber, string dietTypeId)
    {
        _patients[id] = new PatientViewModel { Id = id.ToString(), FirstName = firstName, LastName = lastName, MobileNumber = mobileNumber, DietTypeId = dietTypeId };
    }

    public void SeedAllergy(string id, string name) => _allergies[id] = new AllergyViewModel { Id = id, Name = name };
    public void SeedClinicalState(string id, string name) => _clinicalStates[id] = new ClinicalStateViewModel { Id = id, Name = name };
    public void SeedDietType(string id, string name) => _dietTypes[id] = new DietTypeViewModel { Id = id, Name = name };

    // Patient

    public Task<Guid> AddPatientAsync(PatientCreateRequest request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        _patients[id] = new PatientViewModel { Id = id.ToString(), FirstName = request.FirstName, LastName = request.LastName, MobileNumber = request.MobileNumber, DietTypeId = request.DietTypeId };
        return Task.FromResult(id);
    }

    public Task<PatientViewModel?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var guid)) return Task.FromResult<PatientViewModel?>(null);
        _patients.TryGetValue(guid, out var vm);
        return Task.FromResult(vm);
    }

    public Task<PagedResult<PatientWithDietTypeNameViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = _patients.Values
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatientWithDietTypeNameViewModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MobileNumber = p.MobileNumber,
                DietTypeId = p.DietTypeId,
                DietTypeName = _dietTypes.TryGetValue(p.DietTypeId, out var dt) ? dt.Name : "-"
            }).ToList();
        return Task.FromResult(new PagedResult<PatientWithDietTypeNameViewModel> { Items = items, TotalCount = _patients.Count, Page = page, PageSize = pageSize });
    }

    public Task<PatientDetailViewModel?> GetPatientDetailByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var guid) || !_patients.TryGetValue(guid, out var p))
            return Task.FromResult<PatientDetailViewModel?>(null);

        _patientAllergies.TryGetValue(guid, out var allergyIds);
        _patientClinicalStates.TryGetValue(guid, out var csIds);

        return Task.FromResult<PatientDetailViewModel?>(new PatientDetailViewModel
        {
            Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, MobileNumber = p.MobileNumber, DietTypeId = p.DietTypeId,
            Allergies = (allergyIds ?? []).Select(id => new PatientAllergyViewModel { AllergyId = id, AllergyName = id }).ToList(),
            ClinicalStates = (csIds ?? []).Select(id => new PatientClinicalStateViewModel { ClinicalStateId = id, ClinicalStateName = id }).ToList()
        });
    }

    public Task<PatientServiceDetailViewModel?> GetPatientServiceDetailByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var guid) || !_patients.TryGetValue(guid, out var p))
            return Task.FromResult<PatientServiceDetailViewModel?>(null);

        _patientAllergies.TryGetValue(guid, out var allergyIds);
        _patientClinicalStates.TryGetValue(guid, out var csIds);

        return Task.FromResult<PatientServiceDetailViewModel?>(new PatientServiceDetailViewModel
        {
            Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, MobileNumber = p.MobileNumber, DietTypeId = p.DietTypeId,
            AllergyIds = allergyIds ?? [], ClinicalStateIds = csIds ?? []
        });
    }

    public Task<bool> UpdatePatientAsync(string id, PatientUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null || !Guid.TryParse(id, out var guid) || !_patients.TryGetValue(guid, out var existing))
            return Task.FromResult(false);
        _patients[guid] = new PatientViewModel
        {
            Id = existing.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobileNumber = request.MobileNumber,
            DietTypeId = request.DietTypeId,
            Notes = request.Notes
        };
        return Task.FromResult(true);
    }

    // Allergy

    public Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
    {
        var id = $"ALLERGY_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        _allergies[id] = new AllergyViewModel { Id = id, Name = request.Name };
        return Task.FromResult(id);
    }

    public Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (!_allergies.ContainsKey(id)) return Task.FromResult(false);
        _allergies[id] = new AllergyViewModel { Id = id, Name = request.Name };
        return Task.FromResult(true);
    }

    public Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _allergies.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AllergyViewModel>>(_allergies.Values.ToList());

    public Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(patientId, out var guid)) return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        _patientAllergies.TryGetValue(guid, out var ids);
        return Task.FromResult<IReadOnlyList<string>>(ids ?? []);
    }

    public Task<BatchPatientAllergiesResponse> GetAllergiesByPatientIdsAsync(BatchPatientAllergiesRequest request, CancellationToken cancellationToken = default)
    {
        if (request?.PatientIds == null || request.PatientIds.Count == 0)
            return Task.FromResult(new BatchPatientAllergiesResponse());

        var items = new List<PatientAllergiesItemViewModel>();
        foreach (var id in request.PatientIds)
        {
            if (!Guid.TryParse(id, out var guid)) continue;
            _patientAllergies.TryGetValue(guid, out var allergyIds);
            var names = (allergyIds ?? []).Select(aid => _allergies.TryGetValue(aid, out var a) ? a.Name : aid).ToList();
            items.Add(new PatientAllergiesItemViewModel { PatientId = id, AllergyNames = names });
        }
        return Task.FromResult(new BatchPatientAllergiesResponse { Items = items });
    }

    public Task<bool> UpdatePatientAllergiesAsync(string patientId, PatientAllergiesUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(patientId, out var guid) || !_patients.ContainsKey(guid)) return Task.FromResult(false);
        _patientAllergies[guid] = request.AllergyIds?.ToList() ?? [];
        return Task.FromResult(true);
    }

    // Clinical state

    public Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
    {
        var id = $"CS_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        _clinicalStates[id] = new ClinicalStateViewModel { Id = id, Name = request.Name };
        return Task.FromResult(id);
    }

    public Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (!_clinicalStates.ContainsKey(id)) return Task.FromResult(false);
        _clinicalStates[id] = new ClinicalStateViewModel { Id = id, Name = request.Name };
        return Task.FromResult(true);
    }

    public Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _clinicalStates.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ClinicalStateViewModel>>(_clinicalStates.Values.ToList());

    public Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(patientId, out var guid)) return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        _patientClinicalStates.TryGetValue(guid, out var ids);
        return Task.FromResult<IReadOnlyList<string>>(ids ?? []);
    }

    public Task<bool> UpdatePatientClinicalStatesAsync(string patientId, PatientClinicalStatesUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(patientId, out var guid) || !_patients.ContainsKey(guid)) return Task.FromResult(false);
        _patientClinicalStates[guid] = request.ClinicalStateIds?.ToList() ?? [];
        return Task.FromResult(true);
    }

    // Diet type

    public Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
    {
        var id = $"DT_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        _dietTypes[id] = new DietTypeViewModel { Id = id, Name = request.Name };
        return Task.FromResult(id);
    }

    public Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (!_dietTypes.ContainsKey(id)) return Task.FromResult(false);
        _dietTypes[id] = new DietTypeViewModel { Id = id, Name = request.Name };
        return Task.FromResult(true);
    }

    public Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _dietTypes.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<DietTypeViewModel>>(_dietTypes.Values.ToList());
}
