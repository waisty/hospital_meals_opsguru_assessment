using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IMealRepo
    {
        Task AddMealAsync(Meal meal, CancellationToken cancellationToken = default);
        Task<Meal?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
