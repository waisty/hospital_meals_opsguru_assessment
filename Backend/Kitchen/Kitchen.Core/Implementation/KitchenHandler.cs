using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.Core.InternalModels;

namespace Hospital.Kitchen.Core.Implementation
{
    internal sealed class KitchenHandler : IKitchenHandler
    {
        private readonly IKitchenRepo _repo;

        public KitchenHandler(IKitchenRepo repo)
        {
            _repo = repo;
        }

        public async Task<Guid> CreateTrayAsync(CreateTrayRequest request, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var tray = new Tray
            {
                PatientMealRequestId = request.PatientMealRequestId,
                PatientId = request.PatientId,
                PatientName = request.PatientName,
                RecipeName = request.RecipeName,
                State = request.State,
                ReceivedDateTime = now,
                LastUpdateDateTime = now,
                TrayIngredients = (request.Ingredients ?? [])
                    .Select(i => new TrayIngredient
                    {
                        IngredientName = i.IngredientName,
                        Qty = i.Qty,
                        Unit = i.Unit
                    })
                    .ToList()
            };

            await _repo.AddTrayWithIngredientsAsync(tray, cancellationToken).ConfigureAwait(false);
            return tray.Id;
        }

        public Task<bool> AdvanceTrayStateAsync(Guid trayId, Enums.TrayState fromState, CancellationToken cancellationToken = default)
        {
            return _repo.AdvanceTrayStateAsync(trayId, fromState, cancellationToken);
        }
    }
}
