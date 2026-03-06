using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.UIViewModels;
using static Hospital.Meals.Core.Contracts.Enums;
using MealEntity = Hospital.Meals.Core.InternalModels.Meal;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class MealsHandler : IMealsHandler
    {
        private readonly IMealsRepo _repo;

        public MealsHandler(IMealsRepo repo)
        {
            _repo = repo;
        }

        // Meal

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

        // Recipe

        public async Task AddRecipeAsync(RecipeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var recipe = new Recipe
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                DietTypeId = request.DietTypeId
            };
            await _repo.AddRecipeAsync(recipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task<RecipeViewModel?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var recipe = await _repo.GetRecipeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return recipe == null ? null : recipe.ToRecipeViewModel();
        }

        public async Task<RecipeDetailViewModel?> GetRecipeDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var recipe = await _repo.GetRecipeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (recipe == null) return null;

            var ingredients = await _repo.GetRecipeIngredientsByRecipeIdAsync(id, cancellationToken).ConfigureAwait(false);
            return recipe.ToRecipeDetailViewModel(ingredients);
        }

        public async Task<PagedResult<RecipeViewModel>> ListRecipesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListRecipesAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResult<RecipeViewModel>
            {
                Items = paged.Items.Select(r => r.ToRecipeViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        // Ingredient

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

        // Recipe ingredients

        public async Task<IReadOnlyList<RecipeIngredientViewModel>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default)
        {
            var list = await _repo.GetRecipeIngredientsByRecipeIdAsync(recipeId, cancellationToken).ConfigureAwait(false);
            return list.Select(ri => ri.ToRecipeIngredientViewModel()).ToList();
        }

        public async Task SetRecipeIngredientsAsync(string recipeId, SetRecipeIngredientsRequest request, CancellationToken cancellationToken = default)
        {
            var recipeIngredients = (request.Ingredients ?? [])
                .Select(i => new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity,
                    Unit = i.Unit
                })
                .ToList();
            await _repo.SetRecipeIngredientsForRecipeAsync(recipeId, recipeIngredients, cancellationToken).ConfigureAwait(false);
        }

        // Patient meal request

        public async Task<Guid> AddPatientMealRequestAsync(PatientMealRequestCreateRequest request, CancellationToken cancellationToken = default)
        {
            var mealRequest = new PatientMealRequest
            {
                PatientId = request.PatientId,
                PatientName = request.PatientName,
                RecipeId = request.RecipeId,
                RequestedForDate = request.RequestedForDate,
                ApprovalStatus = MealRequestAppprovalStatus.Pending
            };
            await _repo.AddPatientMealRequestAsync(mealRequest, cancellationToken).ConfigureAwait(false);
            return mealRequest.Id;
        }

        public async Task<PatientMealRequestViewModel?> GetPatientMealRequestByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;
            var request = await _repo.GetPatientMealRequestByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            return request == null ? null : request.ToPatientMealRequestViewModel();
        }

        public async Task<PagedResult<PatientMealRequestViewModel>> ListPatientMealRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListPatientMealRequestsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResult<PatientMealRequestViewModel>
            {
                Items = paged.Items.Select(r => r.ToPatientMealRequestViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        // Allergy (reference data)

        public async Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var allergy = new Allergy { Id = request.Id, Name = request.Name };
            await _repo.AddAllergyAsync(allergy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var allergy = await _repo.GetAllergyByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return allergy == null ? null : allergy.ToAllergyViewModel();
        }

        public async Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListAllergiesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(a => a.ToAllergyViewModel()).ToList();
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetAllergyIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetAllergyExclusionsForIngredientAsync(string ingredientId, SetIngredientAllergyExclusionsRequest request, CancellationToken cancellationToken = default)
        {
            await _repo.SetAllergyExclusionsForIngredientAsync(ingredientId, request.AllergyIds ?? [], cancellationToken).ConfigureAwait(false);
        }

        // Clinical state (reference data)

        public async Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
        {
            var clinicalState = new ClinicalState { Id = request.Id, Name = request.Name };
            await _repo.AddClinicalStateAsync(clinicalState, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var clinicalState = await _repo.GetClinicalStateByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return clinicalState == null ? null : clinicalState.ToClinicalStateViewModel();
        }

        public async Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListClinicalStatesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(c => c.ToClinicalStateViewModel()).ToList();
        }

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetClinicalStateIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, SetIngredientClinicalStateExclusionsRequest request, CancellationToken cancellationToken = default)
        {
            await _repo.SetClinicalStateExclusionsForIngredientAsync(ingredientId, request.ClinicalStateIds ?? [], cancellationToken).ConfigureAwait(false);
        }

        // Diet type (reference data)

        public async Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var dietType = new DietType { Id = request.Id, Name = request.Name };
            await _repo.AddDietTypeAsync(dietType, cancellationToken).ConfigureAwait(false);
        }

        public async Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var dietType = await _repo.GetDietTypeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return dietType == null ? null : dietType.ToDietTypeViewModel();
        }

        public async Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListDietTypesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(d => d.ToDietTypeViewModel()).ToList();
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
