using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    public interface IMealHandler
    {
        Task AddMealAsync(MealCreateRequest request, CancellationToken cancellationToken = default);
        Task<MealViewModel?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<MealViewModel>> ListMealsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateMealAsync(string id, MealUpdateRequest request, CancellationToken cancellationToken = default);
    }
}
