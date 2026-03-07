using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class IngredientHandler : IIngredientHandler
    {
        private readonly IIngredientRepo _repo;

        public IngredientHandler(IIngredientRepo repo)
        {
            _repo = repo;
        }

        public async Task AddIngredientAsync(IngredientCreateRequest request, CancellationToken cancellationToken = default)
        {
            var ingredient = new Ingredient
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };
            await _repo.AddIngredientAsync(ingredient, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IngredientViewModel?> GetIngredientByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var ingredient = await _repo.GetIngredientByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return ingredient == null ? null : ingredient.ToIngredientViewModel();
        }

        public async Task<IngredientDetailViewModel?> GetIngredientDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var ingredient = await _repo.GetIngredientByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (ingredient == null) return null;

            var allergyIds = await _repo.GetAllergyIdsByIngredientIdAsync(id, cancellationToken).ConfigureAwait(false);
            var clinicalStateIds = await _repo.GetClinicalStateIdsByIngredientIdAsync(id, cancellationToken).ConfigureAwait(false);
            var dietTypeIds = await _repo.GetDietTypeExclusionIdsByIngredientIdAsync(id, cancellationToken).ConfigureAwait(false);

            return ingredient.ToIngredientDetailViewModel(allergyIds, clinicalStateIds, dietTypeIds);
        }

        public async Task<PagedResult<IngredientViewModel>> ListIngredientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListIngredientsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResult<IngredientViewModel>
            {
                Items = paged.Items.Select(i => i.ToIngredientViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetAllergyIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetAllergyExclusionsForIngredientAsync(string ingredientId, SetIngredientAllergyExclusionsRequest request, CancellationToken cancellationToken = default)
        {
            await _repo.SetAllergyExclusionsForIngredientAsync(ingredientId, request.AllergyIds ?? [], cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetClinicalStateIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, SetIngredientClinicalStateExclusionsRequest request, CancellationToken cancellationToken = default)
        {
            await _repo.SetClinicalStateExclusionsForIngredientAsync(ingredientId, request.ClinicalStateIds ?? [], cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, SetIngredientDietTypeExclusionsRequest request, CancellationToken cancellationToken = default)
        {
            await _repo.SetDietTypeExclusionsForIngredientAsync(ingredientId, request.DietTypeIds ?? [], cancellationToken).ConfigureAwait(false);
        }
    }
}
