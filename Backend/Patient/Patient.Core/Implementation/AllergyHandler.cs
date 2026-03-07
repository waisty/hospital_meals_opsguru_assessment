using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Hospital.Patient.ViewModels;
using System.Text.RegularExpressions;

namespace Hospital.Patient.Core.Implementation;

internal sealed class AllergyHandler : IAllergyHandler
{
    private readonly IAllergyRepo _repo;

    public AllergyHandler(IAllergyRepo repo)
    {
        _repo = repo;
    }

    public async Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
    {
        var allergy = new Allergy { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
        await _repo.AddAllergyAndPublishAsync(allergy, cancellationToken).ConfigureAwait(false);
        return allergy.Id;
    }

    public async Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) return false;
        return await _repo.UpdateAllergyAndPublishAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var allergy = await _repo.GetAllergyByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return allergy == null ? null : allergy.ToAllergyViewModel();
    }

    public async Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repo.ListAllergiesAsync(cancellationToken).ConfigureAwait(false);
        return list.Select(a => a.ToAllergyViewModel()).ToList();
    }

    private static string CreateReadableIdFromName(string name)
    {
        var cleaned = Regex.Replace(name, @"[^A-Za-z0-9]", "").ToUpperInvariant();
        var truncated = cleaned.Length > 20 ? cleaned[..20] : cleaned;
        var suffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^5..];
        return $"{truncated}_{suffix}";
    }
}
