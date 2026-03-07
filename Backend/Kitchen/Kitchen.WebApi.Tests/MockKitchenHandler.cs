using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.ServiceViewModels;
using Hospital.Kitchen.ViewModels;

namespace Hospital.Kitchen.WebApi.Tests;

public sealed class MockKitchenHandler : IKitchenHandler
{
    private readonly Dictionary<Guid, Enums.TrayState> _trays = new();
    private Guid _nextTrayId = Guid.NewGuid();

    public void SetNextTrayId(Guid id) => _nextTrayId = id;

    public void AddTray(Guid id, Enums.TrayState state) => _trays[id] = state;

    public void Clear()
    {
        _trays.Clear();
        _nextTrayId = Guid.NewGuid();
    }

    public Task<Guid> CreateTrayAsync(CreateTrayRequest request, CancellationToken cancellationToken = default)
    {
        _trays[_nextTrayId] = request.State;
        return Task.FromResult(_nextTrayId);
    }

    public Task<bool> AdvanceTrayStateAsync(Guid trayId, Enums.TrayState fromState, CancellationToken cancellationToken = default)
    {
        if (!_trays.TryGetValue(trayId, out var current) || current != fromState)
            return Task.FromResult(false);

        if (current == Enums.TrayState.Retrieved)
            return Task.FromResult(false);

        _trays[trayId] = (Enums.TrayState)((int)current + 1);
        return Task.FromResult(true);
    }

    public Task<PagedResult<TrayViewModel>> ListTraysAsync(int page, int pageSize, Enums.TrayState? state, bool uncompletedOnly, CancellationToken cancellationToken = default)
    {
        var query = _trays.AsEnumerable();
        if (state.HasValue)
            query = query.Where(kv => kv.Value == state.Value);
        if (uncompletedOnly)
            query = query.Where(kv => kv.Value != Enums.TrayState.Retrieved);
        var ordered = query.OrderByDescending(kv => kv.Key).ToList(); // stable order by id
        var totalCount = ordered.Count;
        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(kv => new TrayViewModel
            {
                Id = kv.Key,
                PatientMealRequestId = Guid.Empty,
                PatientId = "P1",
                PatientName = "Patient 1",
                RecipeName = "Recipe 1",
                State = (int)kv.Value,
                ReceivedDateTime = DateTime.UtcNow,
                LastUpdateDateTime = null
            })
            .ToList();
        return Task.FromResult(new PagedResult<TrayViewModel>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }
}
