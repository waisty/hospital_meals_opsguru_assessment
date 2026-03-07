using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.WebApi.Tests;

public sealed class MockMealsHandler : IMealsHandler
{
    private readonly Dictionary<string, MealViewModel> _meals = new();
    private readonly Dictionary<string, RecipeViewModel> _recipes = new();
    private readonly Dictionary<string, IngredientViewModel> _ingredients = new();
    private readonly Dictionary<string, List<RecipeIngredientViewModel>> _recipeIngredients = new();
    private readonly Dictionary<Guid, PatientRequestViewModel> _patientRequests = new();
    private readonly Dictionary<string, AllergyViewModel> _allergies = new();
    private readonly Dictionary<string, ClinicalStateViewModel> _clinicalStates = new();
    private readonly Dictionary<string, DietTypeViewModel> _dietTypes = new();
    private readonly Dictionary<string, List<string>> _ingredientAllergyExclusions = new();
    private readonly Dictionary<string, List<string>> _ingredientClinicalStateExclusions = new();
    private readonly Dictionary<string, List<string>> _ingredientDietTypeExclusions = new();

    private MealRequestAppprovalStatus _nextRequestStatus = MealRequestAppprovalStatus.Accepted;
    private string? _nextRequestStatusReason;

    public void Clear()
    {
        _meals.Clear(); _recipes.Clear(); _ingredients.Clear();
        _recipeIngredients.Clear(); _patientRequests.Clear();
        _allergies.Clear(); _clinicalStates.Clear(); _dietTypes.Clear();
        _ingredientAllergyExclusions.Clear(); _ingredientClinicalStateExclusions.Clear(); _ingredientDietTypeExclusions.Clear();
        _nextRequestStatus = MealRequestAppprovalStatus.Accepted;
        _nextRequestStatusReason = null;
    }

    public void SeedMeal(string id, string name, string recipeId) =>
        _meals[id] = new MealViewModel { Id = id, Name = name, RecipeId = recipeId };

    public void SeedRecipe(string id, string name) =>
        _recipes[id] = new RecipeViewModel { Id = id, Name = name };

    public void SeedIngredient(string id, string name) =>
        _ingredients[id] = new IngredientViewModel { Id = id, Name = name };

    public void SeedAllergy(string id, string name) =>
        _allergies[id] = new AllergyViewModel { Id = id, Name = name };

    public void SeedClinicalState(string id, string name) =>
        _clinicalStates[id] = new ClinicalStateViewModel { Id = id, Name = name };

    public void SeedDietType(string id, string name) =>
        _dietTypes[id] = new DietTypeViewModel { Id = id, Name = name };

    public void SetNextPatientRequestOutcome(MealRequestAppprovalStatus status, string? reason = null)
    {
        _nextRequestStatus = status;
        _nextRequestStatusReason = reason;
    }

    // Meal

    public Task AddMealAsync(MealCreateRequest request, CancellationToken ct = default)
    {
        _meals[request.Id] = new MealViewModel { Id = request.Id, Name = request.Name, RecipeId = request.RecipeId, DietTypeId = request.DietTypeId };
        return Task.CompletedTask;
    }

    public Task<MealViewModel?> GetMealByIdAsync(string id, CancellationToken ct = default)
    {
        _meals.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<PagedResult<MealViewModel>> ListMealsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var items = _meals.Values.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<MealViewModel> { Items = items, TotalCount = _meals.Count, Page = page, PageSize = pageSize });
    }

    // Recipe

    public Task AddRecipeAsync(RecipeCreateRequest request, CancellationToken ct = default)
    {
        _recipes[request.Id] = new RecipeViewModel { Id = request.Id, Name = request.Name, Description = request.Description };
        return Task.CompletedTask;
    }

    public Task<RecipeViewModel?> GetRecipeByIdAsync(string id, CancellationToken ct = default)
    {
        _recipes.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<RecipeDetailViewModel?> GetRecipeDetailByIdAsync(string id, CancellationToken ct = default)
    {
        if (!_recipes.TryGetValue(id, out var r)) return Task.FromResult<RecipeDetailViewModel?>(null);
        _recipeIngredients.TryGetValue(id, out var ings);
        return Task.FromResult<RecipeDetailViewModel?>(new RecipeDetailViewModel
        {
            Id = r.Id, Name = r.Name, Description = r.Description,
            Ingredients = ings ?? []
        });
    }

    public Task<PagedResult<RecipeViewModel>> ListRecipesAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var items = _recipes.Values.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<RecipeViewModel> { Items = items, TotalCount = _recipes.Count, Page = page, PageSize = pageSize });
    }

    // Ingredient

    public Task AddIngredientAsync(IngredientCreateRequest request, CancellationToken ct = default)
    {
        _ingredients[request.Id] = new IngredientViewModel { Id = request.Id, Name = request.Name, Description = request.Description };
        return Task.CompletedTask;
    }

    public Task<IngredientViewModel?> GetIngredientByIdAsync(string id, CancellationToken ct = default)
    {
        _ingredients.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IngredientDetailViewModel?> GetIngredientDetailByIdAsync(string id, CancellationToken ct = default)
    {
        if (!_ingredients.TryGetValue(id, out var i)) return Task.FromResult<IngredientDetailViewModel?>(null);
        _ingredientAllergyExclusions.TryGetValue(id, out var a);
        _ingredientClinicalStateExclusions.TryGetValue(id, out var cs);
        _ingredientDietTypeExclusions.TryGetValue(id, out var dt);
        return Task.FromResult<IngredientDetailViewModel?>(new IngredientDetailViewModel
        {
            Id = i.Id, Name = i.Name, Description = i.Description,
            AllergyExclusionIds = a ?? [], ClinicalStateExclusionIds = cs ?? [], DietTypeExclusionIds = dt ?? []
        });
    }

    public Task<PagedResult<IngredientViewModel>> ListIngredientsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var items = _ingredients.Values.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<IngredientViewModel> { Items = items, TotalCount = _ingredients.Count, Page = page, PageSize = pageSize });
    }

    // Recipe ingredients

    public Task<IReadOnlyList<RecipeIngredientViewModel>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken ct = default)
    {
        _recipeIngredients.TryGetValue(recipeId, out var list);
        return Task.FromResult<IReadOnlyList<RecipeIngredientViewModel>>(list ?? []);
    }

    public Task SetRecipeIngredientsAsync(string recipeId, SetRecipeIngredientsRequest request, CancellationToken ct = default)
    {
        _recipeIngredients[recipeId] = request.Ingredients?.ToList() ?? [];
        return Task.CompletedTask;
    }

    // Patient request

    public Task<(Guid requestId, MealRequestAppprovalStatus status, string? statusReason, string unsafeIngredientId)> AddPatientRequestAsync(
        PatientRequestCreateRequest request, CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        _patientRequests[id] = new PatientRequestViewModel
        {
            Id = id, PatientId = request.PatientId, RecipeId = request.RecipeId,
            ApprovalStatus = _nextRequestStatus == MealRequestAppprovalStatus.Accepted ? MealRequestApprovalStatus.Accepted : MealRequestApprovalStatus.Rejected,
            StatusReason = _nextRequestStatusReason
        };
        return Task.FromResult((id, _nextRequestStatus, _nextRequestStatusReason, ""));
    }

    public Task<PatientRequestViewModel?> GetPatientRequestByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return Task.FromResult<PatientRequestViewModel?>(null);
        _patientRequests.TryGetValue(guid, out var vm);
        return Task.FromResult(vm);
    }

    public Task<PagedResult<PatientRequestViewModel>> ListPatientRequestsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var items = _patientRequests.Values.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<PatientRequestViewModel> { Items = items, TotalCount = _patientRequests.Count, Page = page, PageSize = pageSize });
    }

    // Allergy reference data

    public Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken ct = default)
    {
        _allergies[request.Id] = new AllergyViewModel { Id = request.Id, Name = request.Name };
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken ct = default)
    {
        if (!_allergies.ContainsKey(id)) return Task.FromResult(false);
        _allergies[id] = new AllergyViewModel { Id = id, Name = request.Name };
        return Task.FromResult(true);
    }

    public Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken ct = default)
    {
        _allergies.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<AllergyViewModel>>(_allergies.Values.ToList());

    public Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken ct = default)
    {
        _ingredientAllergyExclusions.TryGetValue(ingredientId, out var ids);
        return Task.FromResult<IReadOnlyList<string>>(ids ?? []);
    }

    public Task SetAllergyExclusionsForIngredientAsync(string ingredientId, SetIngredientAllergyExclusionsRequest request, CancellationToken ct = default)
    {
        _ingredientAllergyExclusions[ingredientId] = request.AllergyIds?.ToList() ?? [];
        return Task.CompletedTask;
    }

    // Clinical state reference data

    public Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken ct = default)
    {
        _clinicalStates[request.Id] = new ClinicalStateViewModel { Id = request.Id, Name = request.Name };
        return Task.CompletedTask;
    }

    public Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken ct = default)
    {
        if (!_clinicalStates.ContainsKey(id)) return Task.FromResult(false);
        _clinicalStates[id] = new ClinicalStateViewModel { Id = id, Name = request.Name };
        return Task.FromResult(true);
    }

    public Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken ct = default)
    {
        _clinicalStates.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ClinicalStateViewModel>>(_clinicalStates.Values.ToList());

    public Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken ct = default)
    {
        _ingredientClinicalStateExclusions.TryGetValue(ingredientId, out var ids);
        return Task.FromResult<IReadOnlyList<string>>(ids ?? []);
    }

    public Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, SetIngredientClinicalStateExclusionsRequest request, CancellationToken ct = default)
    {
        _ingredientClinicalStateExclusions[ingredientId] = request.ClinicalStateIds?.ToList() ?? [];
        return Task.CompletedTask;
    }

    // Diet type reference data

    public Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken ct = default)
    {
        _dietTypes[request.Id] = new DietTypeViewModel { Id = request.Id, Name = request.Name };
        return Task.CompletedTask;
    }

    public Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken ct = default)
    {
        if (!_dietTypes.ContainsKey(id)) return Task.FromResult(false);
        _dietTypes[id] = new DietTypeViewModel { Id = id, Name = request.Name };
        return Task.FromResult(true);
    }

    public Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken ct = default)
    {
        _dietTypes.TryGetValue(id, out var vm);
        return Task.FromResult(vm);
    }

    public Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<DietTypeViewModel>>(_dietTypes.Values.ToList());

    public Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken ct = default)
    {
        _ingredientDietTypeExclusions.TryGetValue(ingredientId, out var ids);
        return Task.FromResult<IReadOnlyList<string>>(ids ?? []);
    }

    public Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, SetIngredientDietTypeExclusionsRequest request, CancellationToken ct = default)
    {
        _ingredientDietTypeExclusions[ingredientId] = request.DietTypeIds?.ToList() ?? [];
        return Task.CompletedTask;
    }
}
