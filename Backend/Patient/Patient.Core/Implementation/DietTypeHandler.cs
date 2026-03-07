using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Hospital.Patient.ViewModels;
using System.Text.RegularExpressions;

namespace Hospital.Patient.Core.Implementation;

internal sealed class DietTypeHandler : IDietTypeHandler
{
    private readonly IDietTypeRepo _repo;

    public DietTypeHandler(IDietTypeRepo repo)
    {
        _repo = repo;
    }

    public async Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
    {
        var dietType = new DietType { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
        await _repo.AddDietTypeAndPublishAsync(dietType, cancellationToken).ConfigureAwait(false);
        return dietType.Id;
    }

    public async Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) return false;
        return await _repo.UpdateDietTypeAndPublishAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
    }

    public async Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var dietType = await _repo.GetDietTypeByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return dietType == null ? null : dietType.ToDietTypeViewModel();
    }

    public async Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repo.ListDietTypesAsync(cancellationToken).ConfigureAwait(false);
        return list.Select(d => d.ToDietTypeViewModel()).ToList();
    }

    private static string CreateReadableIdFromName(string name)
    {
        var cleaned = Regex.Replace(name, @"[^A-Za-z0-9]", "").ToUpperInvariant();
        var truncated = cleaned.Length > 20 ? cleaned[..20] : cleaned;
        var suffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^5..];
        return $"{truncated}_{suffix}";
    }
}
