using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.WebApi.Tests;

internal sealed class MockMealsRepo : IMealRepo, IRecipeRepo, IIngredientRepo, IPatientRequestRepo, IReferenceDataRepo
{
    private readonly Dictionary<string, Meal> _meals = new();
    private readonly Dictionary<string, Recipe> _recipes = new();
    private readonly Dictionary<string, Ingredient> _ingredients = new();
    private readonly List<RecipeIngredient> _recipeIngredients = new();
    private readonly Dictionary<Guid, PatientRequest> _patientRequests = new();
    private readonly Dictionary<string, Allergy> _allergies = new();
    private readonly Dictionary<string, ClinicalState> _clinicalStates = new();
    private readonly Dictionary<string, DietType> _dietTypes = new();
    private readonly List<(string IngredientId, string AllergyId)> _ingredientAllergyExclusions = new();
    private readonly List<(string IngredientId, string ClinicalStateId)> _ingredientClinicalStateExclusions = new();
    private readonly List<(string IngredientId, string DietTypeId)> _ingredientDietTypeExclusions = new();

    public void Clear()
    {
        _meals.Clear();
        _recipes.Clear();
        _ingredients.Clear();
        _recipeIngredients.Clear();
        _patientRequests.Clear();
        _allergies.Clear();
        _clinicalStates.Clear();
        _dietTypes.Clear();
        _ingredientAllergyExclusions.Clear();
        _ingredientClinicalStateExclusions.Clear();
        _ingredientDietTypeExclusions.Clear();
    }

    // ── Seed helpers ─────────────────────────────────────────────────

    public void SeedMeal(string id, string name, string recipeId, string? dietTypeId = null) =>
        _meals[id] = new Meal { Id = id, Name = name, RecipeId = recipeId, DietTypeId = dietTypeId };

    public void SeedRecipe(string id, string name, string? description = null) =>
        _recipes[id] = new Recipe { Id = id, Name = name, Description = description };

    public void SeedIngredient(string id, string name, string? description = null) =>
        _ingredients[id] = new Ingredient { Id = id, Name = name, Description = description };

    public void SeedRecipeIngredient(string recipeId, string ingredientId, decimal quantity = 1, string? unit = null) =>
        _recipeIngredients.Add(new RecipeIngredient { RecipeId = recipeId, IngredientId = ingredientId, Quantity = quantity, Unit = unit });

    public void SeedAllergy(string id, string name) =>
        _allergies[id] = new Allergy { Id = id, Name = name };

    public void SeedClinicalState(string id, string name) =>
        _clinicalStates[id] = new ClinicalState { Id = id, Name = name };

    public void SeedDietType(string id, string name) =>
        _dietTypes[id] = new DietType { Id = id, Name = name };

    public void SeedIngredientAllergyExclusion(string ingredientId, string allergyId) =>
        _ingredientAllergyExclusions.Add((ingredientId, allergyId));

    public void SeedIngredientClinicalStateExclusion(string ingredientId, string clinicalStateId) =>
        _ingredientClinicalStateExclusions.Add((ingredientId, clinicalStateId));

    public void SeedIngredientDietTypeExclusion(string ingredientId, string dietTypeId) =>
        _ingredientDietTypeExclusions.Add((ingredientId, dietTypeId));

    // ── Meal ─────────────────────────────────────────────────────────

    public Task AddMealAsync(Meal meal, CancellationToken ct = default)
    {
        _meals[meal.Id] = meal;
        return Task.CompletedTask;
    }

    public Task<Meal?> GetMealByIdAsync(string id, CancellationToken ct = default)
    {
        _meals.TryGetValue(id, out var meal);
        return Task.FromResult(meal);
    }

    public Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var all = _meals.Values.OrderBy(m => m.Name).ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<Meal> { Items = items, TotalCount = all.Count, Page = page, PageSize = pageSize });
    }

    // ── Recipe ───────────────────────────────────────────────────────

    public Task AddRecipeAsync(Recipe recipe, CancellationToken ct = default)
    {
        _recipes[recipe.Id] = recipe;
        return Task.CompletedTask;
    }

    public Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken ct = default)
    {
        _recipes.TryGetValue(id, out var recipe);
        return Task.FromResult(recipe);
    }

    public Task<PagedResult<Recipe>> ListRecipesAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var all = _recipes.Values.OrderBy(r => r.Name).ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<Recipe> { Items = items, TotalCount = all.Count, Page = page, PageSize = pageSize });
    }

    // ── Ingredient ───────────────────────────────────────────────────

    public Task AddIngredientAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        _ingredients[ingredient.Id] = ingredient;
        return Task.CompletedTask;
    }

    public Task<Ingredient?> GetIngredientByIdAsync(string id, CancellationToken ct = default)
    {
        _ingredients.TryGetValue(id, out var ingredient);
        return Task.FromResult(ingredient);
    }

    public Task<PagedResult<Ingredient>> ListIngredientsAsync(int page, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var query = _ingredients.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search) && search.Length >= 2)
        {
            var words = search.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                var w = word;
                query = query.Where(i =>
                    (i.Name?.Contains(w, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (i.Description?.Contains(w, StringComparison.OrdinalIgnoreCase) ?? false));
            }
        }
        var all = query.OrderBy(i => i.Name).ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<Ingredient> { Items = items, TotalCount = all.Count, Page = page, PageSize = pageSize });
    }

    // ── Recipe ingredients ───────────────────────────────────────────

    public Task<IReadOnlyList<RecipeIngredientWithName>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken ct = default)
    {
        var list = _recipeIngredients
            .Where(ri => ri.RecipeId == recipeId)
            .Select(ri =>
            {
                _ingredients.TryGetValue(ri.IngredientId, out var ing);
                return new RecipeIngredientWithName
                {
                    RecipeId = ri.RecipeId,
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit,
                    IngredientName = ing?.Name ?? ri.IngredientId
                };
            })
            .OrderBy(ri => ri.IngredientName)
            .ToList();
        return Task.FromResult<IReadOnlyList<RecipeIngredientWithName>>(list);
    }

    public Task SetRecipeIngredientsForRecipeAsync(string recipeId, IReadOnlyList<RecipeIngredient> recipeIngredients, CancellationToken ct = default)
    {
        _recipeIngredients.RemoveAll(ri => ri.RecipeId == recipeId);
        _recipeIngredients.AddRange(recipeIngredients);
        return Task.CompletedTask;
    }

    // ── Patient request ──────────────────────────────────────────────

    public Task AddPatientRequestAsync(PatientRequest request, CancellationToken ct = default)
    {
        if (request.Id == Guid.Empty)
            request.Id = Guid.NewGuid();
        _patientRequests[request.Id] = request;
        return Task.CompletedTask;
    }

    public Task UpdatePatientRequestAsync(PatientRequest request, CancellationToken ct = default)
    {
        _patientRequests[request.Id] = request;
        return Task.CompletedTask;
    }

    public Task<PatientRequest?> GetPatientRequestByIdAsync(Guid id, CancellationToken ct = default)
    {
        _patientRequests.TryGetValue(id, out var pr);
        return Task.FromResult(pr);
    }

    public Task<PagedResult<PatientRequest>> ListPatientRequestsAsync(int page, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var all = _patientRequests.Values.OrderBy(r => r.RequestedDateTime).ThenBy(r => r.PatientId).ToList();
        if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length >= 2)
        {
            var terms = search!.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            all = all.Where(r =>
                terms.All(t => r.FirstName.Contains(t, StringComparison.OrdinalIgnoreCase) || r.LastName.Contains(t, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<PatientRequest> { Items = items, TotalCount = all.Count, Page = page, PageSize = pageSize });
    }

    // ── Allergy ──────────────────────────────────────────────────────

    public Task AddAllergyAsync(Allergy allergy, CancellationToken ct = default)
    {
        _allergies[allergy.Id] = allergy;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAllergyAsync(string id, string name, CancellationToken ct = default)
    {
        if (!_allergies.TryGetValue(id, out var a)) return Task.FromResult(false);
        a.Name = name;
        return Task.FromResult(true);
    }

    public Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken ct = default)
    {
        _allergies.TryGetValue(id, out var a);
        return Task.FromResult(a);
    }

    public Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Allergy>>(_allergies.Values.OrderBy(a => a.Id).ToList());

    public Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(_ingredientAllergyExclusions.Where(x => x.IngredientId == ingredientId).Select(x => x.AllergyId).ToList());

    public Task SetAllergyExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> allergyIds, CancellationToken ct = default)
    {
        _ingredientAllergyExclusions.RemoveAll(x => x.IngredientId == ingredientId);
        foreach (var aid in allergyIds)
            _ingredientAllergyExclusions.Add((ingredientId, aid));
        return Task.CompletedTask;
    }

    // ── Clinical state ───────────────────────────────────────────────

    public Task AddClinicalStateAsync(ClinicalState cs, CancellationToken ct = default)
    {
        _clinicalStates[cs.Id] = cs;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateClinicalStateAsync(string id, string name, CancellationToken ct = default)
    {
        if (!_clinicalStates.TryGetValue(id, out var cs)) return Task.FromResult(false);
        cs.Name = name;
        return Task.FromResult(true);
    }

    public Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken ct = default)
    {
        _clinicalStates.TryGetValue(id, out var cs);
        return Task.FromResult(cs);
    }

    public Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ClinicalState>>(_clinicalStates.Values.OrderBy(c => c.Id).ToList());

    public Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(_ingredientClinicalStateExclusions.Where(x => x.IngredientId == ingredientId).Select(x => x.ClinicalStateId).ToList());

    public Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> clinicalStateIds, CancellationToken ct = default)
    {
        _ingredientClinicalStateExclusions.RemoveAll(x => x.IngredientId == ingredientId);
        foreach (var cid in clinicalStateIds)
            _ingredientClinicalStateExclusions.Add((ingredientId, cid));
        return Task.CompletedTask;
    }

    // ── Diet type ────────────────────────────────────────────────────

    public Task AddDietTypeAsync(DietType dt, CancellationToken ct = default)
    {
        _dietTypes[dt.Id] = dt;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateDietTypeAsync(string id, string name, CancellationToken ct = default)
    {
        if (!_dietTypes.TryGetValue(id, out var dt)) return Task.FromResult(false);
        dt.Name = name;
        return Task.FromResult(true);
    }

    public Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken ct = default)
    {
        _dietTypes.TryGetValue(id, out var dt);
        return Task.FromResult(dt);
    }

    public Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<DietType>>(_dietTypes.Values.OrderBy(d => d.Id).ToList());

    public Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(_ingredientDietTypeExclusions.Where(x => x.IngredientId == ingredientId).Select(x => x.DietTypeId).ToList());

    public Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> dietTypeIds, CancellationToken ct = default)
    {
        _ingredientDietTypeExclusions.RemoveAll(x => x.IngredientId == ingredientId);
        foreach (var did in dietTypeIds)
            _ingredientDietTypeExclusions.Add((ingredientId, did));
        return Task.CompletedTask;
    }

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetAllergyExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken ct = default)
    {
        var idSet = ingredientIds.Distinct().ToHashSet();
        var dict = _ingredientAllergyExclusions
            .Where(x => idSet.Contains(x.IngredientId))
            .GroupBy(x => x.IngredientId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.AllergyId).ToList());
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(dict);
    }

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetClinicalStateExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken ct = default)
    {
        var idSet = ingredientIds.Distinct().ToHashSet();
        var dict = _ingredientClinicalStateExclusions
            .Where(x => idSet.Contains(x.IngredientId))
            .GroupBy(x => x.IngredientId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.ClinicalStateId).ToList());
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(dict);
    }

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDietTypeExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken ct = default)
    {
        var idSet = ingredientIds.Distinct().ToHashSet();
        var dict = _ingredientDietTypeExclusions
            .Where(x => idSet.Contains(x.IngredientId))
            .GroupBy(x => x.IngredientId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.DietTypeId).ToList());
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(dict);
    }
}
