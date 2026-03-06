using Microsoft.EntityFrameworkCore;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class MealsRepo : IMealsRepo
    {
        private readonly MealsDBContext _context;

        public MealsRepo(MealsDBContext context)
        {
            _context = context;
        }

        // Meal
        public async Task AddMealAsync(Meal meal, CancellationToken cancellationToken = default)
        {
            _context.Meals.Add(meal);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Meal?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Meals.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.Meals
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<Meal>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // Recipe
        public async Task AddRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<Recipe>> ListRecipesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Recipes.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.Recipes
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<Recipe>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // Ingredient
        public async Task AddIngredientAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<Ingredient>> ListIngredientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Ingredients.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.Ingredients
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<Ingredient>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // Recipe ingredient
        public async Task<IReadOnlyList<RecipeIngredient>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default)
        {
            return await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipeId)
                .OrderBy(ri => ri.IngredientId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetRecipeIngredientsForRecipeAsync(string recipeId, IReadOnlyList<RecipeIngredient> recipeIngredients, CancellationToken cancellationToken = default)
        {
            await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipeId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var ri in recipeIngredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // Patient meal request
        public async Task AddPatientMealRequestAsync(PatientMealRequest request, CancellationToken cancellationToken = default)
        {
            _context.PatientMealRequests.Add(request);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<PatientMealRequest?> GetPatientMealRequestByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.PatientMealRequests
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<PatientMealRequest>> ListPatientMealRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.PatientMealRequests.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.PatientMealRequests
                .OrderBy(r => r.RequestedForDate)
                .ThenBy(r => r.PatientId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<PatientMealRequest>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // Allergy (reference data)
        public async Task AddAllergyAsync(Allergy allergy, CancellationToken cancellationToken = default)
        {
            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Allergies
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Allergies
                .OrderBy(a => a.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _context.IngredientAllergyExclusions
                .Where(iae => iae.IngredientId == ingredientId)
                .Select(iae => iae.AllergyId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetAllergyExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default)
        {
            await _context.IngredientAllergyExclusions
                .Where(iae => iae.IngredientId == ingredientId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var allergyId in allergyIds)
            {
                _context.IngredientAllergyExclusions.Add(new IngredientAllergyExclusions { IngredientId = ingredientId, AllergyId = allergyId });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // Clinical state (reference data)
        public async Task AddClinicalStateAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default)
        {
            _context.ClinicalStates.Add(clinicalState);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.ClinicalStates
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClinicalStates
                .OrderBy(c => c.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _context.IngredientClinicalStateExclusions
                .Where(ice => ice.IngredientId == ingredientId)
                .Select(ice => ice.ClinicalStateId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default)
        {
            await _context.IngredientClinicalStateExclusions
                .Where(ice => ice.IngredientId == ingredientId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var clinicalStateId in clinicalStateIds)
            {
                _context.IngredientClinicalStateExclusions.Add(new IngredientClinicalStateExclusions { IngredientId = ingredientId, ClinicalStateId = clinicalStateId });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // Diet type (reference data)
        public async Task AddDietTypeAsync(DietType dietType, CancellationToken cancellationToken = default)
        {
            _context.DietTypes.Add(dietType);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.DietTypes
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DietTypes
                .OrderBy(d => d.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default)
        {
            return await _context.IngredientDietTypeExclusions
                .Where(idte => idte.IngredientId == ingredientId)
                .Select(idte => idte.DietTypeId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> dietTypeIds, CancellationToken cancellationToken = default)
        {
            await _context.IngredientDietTypeExclusions
                .Where(idte => idte.IngredientId == ingredientId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var dietTypeId in dietTypeIds)
            {
                _context.IngredientDietTypeExclusions.Add(new IngredientDietTypeExclusions { IngredientId = ingredientId, DietTypeId = dietTypeId });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
