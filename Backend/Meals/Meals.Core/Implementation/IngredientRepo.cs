using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class IngredientRepo : IIngredientRepo
    {
        private readonly MealsDBContext _context;

        public IngredientRepo(MealsDBContext context)
        {
            _context = context;
        }

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
