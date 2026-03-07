using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.Core.InternalModels;
using Hospital.Kitchen.ServiceViewModels;
using Hospital.Kitchen.ViewModels;

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
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
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

        public async Task<PagedResult<TrayViewModel>> ListTraysAsync(int page, int pageSize, Enums.TrayState? state, bool uncompletedOnly, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListTraysAsync(page, pageSize, state, uncompletedOnly, cancellationToken).ConfigureAwait(false);
            return new PagedResult<TrayViewModel>
            {
                Items = paged.Items.Select(t => new TrayViewModel
                {
                    Id = t.Id,
                    PatientMealRequestId = t.PatientMealRequestId,
                    PatientId = t.PatientId,
                    PatientName = string.Join(" ", new[] { t.FirstName, t.MiddleName, t.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim(),
                    RecipeName = t.RecipeName,
                    State = (int)t.State,
                    StateName = t.State.ToString(),
                    ReceivedDateTime = t.ReceivedDateTime,
                    LastUpdateDateTime = t.LastUpdateDateTime
                }).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }
    }
}
