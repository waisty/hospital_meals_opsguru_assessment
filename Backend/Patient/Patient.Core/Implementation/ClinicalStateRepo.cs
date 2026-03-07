using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Patient.Core.Implementation;

internal sealed class ClinicalStateRepo : IClinicalStateRepo
{
    private readonly PatientDBContext _context;
    private readonly IMealsApiClient _mealsApiClient;

    public ClinicalStateRepo(PatientDBContext context, IMealsApiClient mealsApiClient)
    {
        _context = context;
        _mealsApiClient = mealsApiClient;
    }

    public async Task AddClinicalStateAndPublishAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async ct =>
        {
            _context.ClinicalStates.Add(clinicalState);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishClinicalStateAsync(clinicalState.Id, clinicalState.Name, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpdateClinicalStateAndPublishAsync(string id, string name, CancellationToken cancellationToken = default)
    {
        var clinicalState = await _context.ClinicalStates.FirstOrDefaultAsync(c => c.Id == id, cancellationToken).ConfigureAwait(false);
        if (clinicalState is null) return false;

        await ExecuteInTransactionAsync(async ct =>
        {
            clinicalState.Name = name;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishClinicalStateUpdateAsync(id, name, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.ClinicalStates
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ClinicalStates
            .OrderBy(c => c.Id)
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
