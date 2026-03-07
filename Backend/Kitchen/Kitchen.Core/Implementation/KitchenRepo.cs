using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Hospital.Kitchen.Core.Contracts.Enums;

namespace Hospital.Kitchen.Core.Implementation
{
    internal sealed class KitchenRepo : IKitchenRepo
    {
        private readonly KitchenDBContext _context;
        private readonly ILogger _logger;

        public KitchenRepo(KitchenDBContext context, ILogger<KitchenRepo> logger)
        {
            _context = context;
            _logger = logger;
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

        public async Task<bool> AdvanceTrayStateAsync(Guid trayId, TrayState fromState, CancellationToken cancellationToken = default)
        {
            var tray = await _context.Trays
                .FirstOrDefaultAsync(t => t.Id == trayId, cancellationToken)
                .ConfigureAwait(false);

            if (tray == null)
                return false;

            if (tray.State != fromState)
            {
                _logger.LogWarning("Tray {TrayId} state is {CurrentState}, expected {FromState}", trayId, tray.State, fromState);
                return false;
            }

            bool ret = false;

            await ExecuteInTransactionAsync(async ct =>
            {
                var trayToUpdate = await _context.Trays
                    .FromSqlRaw("SELECT * FROM dbo.trays WHERE id = {0} FOR UPDATE", trayId)
                    .AsTracking()
                    .FirstOrDefaultAsync(ct)
                    .ConfigureAwait(false);

                if (trayToUpdate is null)
                {
                    ret = false;
                    return;
                }

                if(trayToUpdate.State == TrayState.Retrieved)
                {
                    ret = false;
                    return;
                }

                TrayState toState = fromState + 1;

                trayToUpdate.State = toState;
                trayToUpdate.LastUpdateDateTime = DateTime.UtcNow;
                _context.TrayStatusHistories.Add(new TrayStatusHistory
                {
                    TrayId = trayId,
                    Status = toState,
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);

                ret = true;
            }, cancellationToken).ConfigureAwait(false);
            return ret;
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
