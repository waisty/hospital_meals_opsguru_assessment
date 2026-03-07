using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class IngredientHandler : IIngredientHandler
    {
        private readonly IIngredientRepo _repo;
        private readonly IReferenceDataHandler _referenceData;

        public IngredientHandler(IIngredientRepo repo, IReferenceDataHandler referenceData)
        {
            _repo = repo;
            _referenceData = referenceData;
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

        public async Task<bool> UpdateIngredientAsync(string id, IngredientUpdateRequest request, CancellationToken cancellationToken = default)
        {
            return await _repo.UpdateIngredientAsync(id, request.Name, request.Description, cancellationToken).ConfigureAwait(false);
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

        public async Task<PagedResult<IngredientViewModel>> ListIngredientsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListIngredientsAsync(page, pageSize, search, cancellationToken).ConfigureAwait(false);
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

        public async Task<IngredientExclusionNamesResponse> GetExclusionNamesByIngredientIdsAsync(IngredientExclusionNamesRequest request, CancellationToken cancellationToken = default)
        {
            var ingredientIds = request.IngredientIds ?? [];
            if (ingredientIds.Count == 0)
                return new IngredientExclusionNamesResponse { Items = [] };

            var allergyIdsByIngredient = await _repo.GetAllergyExclusionIdsByIngredientIdsAsync(ingredientIds, cancellationToken).ConfigureAwait(false);
            var clinicalStateIdsByIngredient = await _repo.GetClinicalStateExclusionIdsByIngredientIdsAsync(ingredientIds, cancellationToken).ConfigureAwait(false);
            var dietTypeIdsByIngredient = await _repo.GetDietTypeExclusionIdsByIngredientIdsAsync(ingredientIds, cancellationToken).ConfigureAwait(false);

            var allergies = await _referenceData.ListAllergiesAsync(cancellationToken).ConfigureAwait(false);
            var clinicalStates = await _referenceData.ListClinicalStatesAsync(cancellationToken).ConfigureAwait(false);
            var dietTypes = await _referenceData.ListDietTypesAsync(cancellationToken).ConfigureAwait(false);

            var allergyNameById = allergies.ToDictionary(a => a.Id, a => a.Name);
            var clinicalStateNameById = clinicalStates.ToDictionary(c => c.Id, c => c.Name);
            var dietTypeNameById = dietTypes.ToDictionary(d => d.Id, d => d.Name);

            var items = ingredientIds.Distinct().Select(id =>
            {
                allergyIdsByIngredient.TryGetValue(id, out var aIds);
                clinicalStateIdsByIngredient.TryGetValue(id, out var cIds);
                dietTypeIdsByIngredient.TryGetValue(id, out var dIds);
                return new IngredientExclusionNamesItem
                {
                    IngredientId = id,
                    AllergyNames = (aIds ?? []).Select(aid => allergyNameById.GetValueOrDefault(aid, aid)).ToList(),
                    ClinicalStateNames = (cIds ?? []).Select(cid => clinicalStateNameById.GetValueOrDefault(cid, cid)).ToList(),
                    DietTypeNames = (dIds ?? []).Select(did => dietTypeNameById.GetValueOrDefault(did, did)).ToList(),
                };
            }).ToList();

            return new IngredientExclusionNamesResponse { Items = items };
        }
    }
}
