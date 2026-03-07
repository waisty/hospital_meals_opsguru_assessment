using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Patient.Core.Implementation;

internal sealed class DietTypeRepo : IDietTypeRepo
{
    private readonly PatientDBContext _context;
    private readonly IMealsApiClient _mealsApiClient;

    public DietTypeRepo(PatientDBContext context, IMealsApiClient mealsApiClient)
    {
        _context = context;
        _mealsApiClient = mealsApiClient;
    }

    public async Task AddDietTypeAndPublishAsync(DietType dietType, CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async ct =>
        {
            _context.DietTypes.Add(dietType);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishDietTypeAsync(dietType.Id, dietType.Name, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpdateDietTypeAndPublishAsync(string id, string name, CancellationToken cancellationToken = default)
    {
        var dietType = await _context.DietTypes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken).ConfigureAwait(false);
        if (dietType is null) return false;

        await ExecuteInTransactionAsync(async ct =>
        {
            dietType.Name = name;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishDietTypeUpdateAsync(id, name, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.DietTypes
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DietTypes
            .OrderBy(d => d.Id)
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
