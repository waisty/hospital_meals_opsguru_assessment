using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Hospital.Patient.ViewModels;
using System.Text.RegularExpressions;

namespace Hospital.Patient.Core.Implementation;

internal sealed class ClinicalStateHandler : IClinicalStateHandler
{
    private readonly IClinicalStateRepo _repo;

    public ClinicalStateHandler(IClinicalStateRepo repo)
    {
        _repo = repo;
    }

    public async Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
    {
        var clinicalState = new ClinicalState { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
        await _repo.AddClinicalStateAndPublishAsync(clinicalState, cancellationToken).ConfigureAwait(false);
        return clinicalState.Id;
    }

    public async Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) return false;
        return await _repo.UpdateClinicalStateAndPublishAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var clinicalState = await _repo.GetClinicalStateByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return clinicalState == null ? null : clinicalState.ToClinicalStateViewModel();
    }

    public async Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repo.ListClinicalStatesAsync(cancellationToken).ConfigureAwait(false);
        return list.Select(c => c.ToClinicalStateViewModel()).ToList();
    }

    private static string CreateReadableIdFromName(string name)
    {
        var cleaned = Regex.Replace(name, @"[^A-Za-z0-9]", "").ToUpperInvariant();
        var truncated = cleaned.Length > 20 ? cleaned[..20] : cleaned;
        var suffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^5..];
        return $"{truncated}_{suffix}";
    }
}
