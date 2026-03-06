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
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _context.Trays.Add(tray);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
