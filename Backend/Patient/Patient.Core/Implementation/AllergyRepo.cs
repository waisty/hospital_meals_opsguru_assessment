using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Patient.Core.Implementation;

internal sealed class AllergyRepo : IAllergyRepo
{
    private readonly PatientDBContext _context;
    private readonly IMealsApiClient _mealsApiClient;

    public AllergyRepo(PatientDBContext context, IMealsApiClient mealsApiClient)
    {
        _context = context;
        _mealsApiClient = mealsApiClient;
    }

    public async Task AddAllergyAndPublishAsync(Allergy allergy, CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async ct =>
        {
            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishAllergyAsync(allergy.Id, allergy.Name, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpdateAllergyAndPublishAsync(string id, string name, CancellationToken cancellationToken = default)
    {
        var allergy = await _context.Allergies.FirstOrDefaultAsync(a => a.Id == id, cancellationToken).ConfigureAwait(false);
        if (allergy is null) return false;

        await ExecuteInTransactionAsync(async ct =>
        {
            allergy.Name = name;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishAllergyUpdateAsync(id, name, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Allergies
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Allergies
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await work(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}
