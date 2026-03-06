using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.Core.InternalModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Kitchen.Core.Implementation
{
    internal sealed class KitchenRepo : IKitchenRepo
    {
        private readonly KitchenDBContext _context;

        public KitchenRepo(KitchenDBContext context)
        {
            _context = context;
        }

        public async Task AddTrayWithIngredientsAsync(Tray tray, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async ct =>
            {
                _context.Trays.Add(tray);
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);

                _context.TrayStatusHistories.Add(new TrayStatusHistory
                {
                    TrayId = tray.Id,
                    Status = tray.State,
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateTrayStateAsync(Guid trayId, string newState, CancellationToken cancellationToken = default)
        {
            var tray = await _context.Trays
                .FirstOrDefaultAsync(t => t.Id == trayId, cancellationToken)
                .ConfigureAwait(false);
            if (tray is null)
                return false;

            await ExecuteInTransactionAsync(async ct =>
            {
                tray.State = newState;
                _context.Trays.Update(tray);
                _context.TrayStatusHistories.Add(new TrayStatusHistory
                {
                    TrayId = trayId,
                    Status = newState,
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
            return true;
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
}
