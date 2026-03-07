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

        public async Task<PagedResult<Ingredient>> ListIngredientsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            // Full-text search only runs when search is at least 2 characters
            if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length < 2)
                search = null;

            var query = _context.Ingredients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var words = search.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    var pattern = $"%{word}%";
                    query = query.Where(i =>
                        Microsoft.EntityFrameworkCore.EF.Functions.ILike(i.Name, pattern) ||
                        (i.Description != null && Microsoft.EntityFrameworkCore.EF.Functions.ILike(i.Description, pattern)));
                }
            }

            query = query.OrderBy(i => i.Name);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
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

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetAllergyExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken cancellationToken = default)
        {
            var idList = ingredientIds.Distinct().ToList();
            if (idList.Count == 0) return new Dictionary<string, IReadOnlyList<string>>();

            var pairs = await _context.IngredientAllergyExclusions
                .Where(iae => idList.Contains(iae.IngredientId))
                .Select(iae => new { iae.IngredientId, iae.AllergyId })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return pairs
                .GroupBy(x => x.IngredientId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.AllergyId).ToList());
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetClinicalStateExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken cancellationToken = default)
        {
            var idList = ingredientIds.Distinct().ToList();
            if (idList.Count == 0) return new Dictionary<string, IReadOnlyList<string>>();

            var pairs = await _context.IngredientClinicalStateExclusions
                .Where(ice => idList.Contains(ice.IngredientId))
                .Select(ice => new { ice.IngredientId, ice.ClinicalStateId })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return pairs
                .GroupBy(x => x.IngredientId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.ClinicalStateId).ToList());
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDietTypeExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken cancellationToken = default)
        {
            var idList = ingredientIds.Distinct().ToList();
            if (idList.Count == 0) return new Dictionary<string, IReadOnlyList<string>>();

            var pairs = await _context.IngredientDietTypeExclusions
                .Where(idte => idList.Contains(idte.IngredientId))
                .Select(idte => new { idte.IngredientId, idte.DietTypeId })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return pairs
                .GroupBy(x => x.IngredientId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.DietTypeId).ToList());
        }
    }
}
