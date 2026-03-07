using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using MealEntity = Hospital.Meals.Core.InternalModels.Meal;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class MealHandler : IMealHandler
    {
        private readonly IMealRepo _repo;

        public MealHandler(IMealRepo repo)
        {
            _repo = repo;
        }

        public async Task AddMealAsync(MealCreateRequest request, CancellationToken cancellationToken = default)
        {
            var meal = new MealEntity
            {
                Id = request.Id,
                Name = request.Name,
                RecipeId = request.RecipeId,
                DietTypeId = request.DietTypeId
            };
            await _repo.AddMealAsync(meal, cancellationToken).ConfigureAwait(false);
        }

        public async Task<MealViewModel?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var meal = await _repo.GetMealByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return meal == null ? null : meal.ToMealViewModel();
        }

        public async Task<PagedResult<MealViewModel>> ListMealsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListMealsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResult<MealViewModel>
            {
                Items = paged.Items.Select(m => m.ToMealViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }
    }
}
