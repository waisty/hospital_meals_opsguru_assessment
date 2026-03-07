using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ServiceViewModels;

namespace Hospital.Meals.WebApi.Tests;

internal sealed class MockKitchenApiClient : IKitchenApiClient
{
    private readonly List<KitchenPublishTrayRequest> _publishedTrays = new();

    public IReadOnlyList<KitchenPublishTrayRequest> PublishedTrays => _publishedTrays;

    public void Clear() => _publishedTrays.Clear();

    public Task<Guid> PublishTrayAsync(KitchenPublishTrayRequest request, CancellationToken cancellationToken = default)
    {
        _publishedTrays.Add(request);
        return Task.FromResult(Guid.NewGuid());
    }
}
