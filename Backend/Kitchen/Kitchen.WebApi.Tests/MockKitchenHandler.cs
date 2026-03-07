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
}
