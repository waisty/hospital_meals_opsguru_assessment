using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.Core.Implementation
{
    internal static class MealsViewModelExtensions
    {
        public static MealViewModel ToMealViewModel(this Meal meal) => new()
        {
            Id = meal.Id,
            Name = meal.Name,
            RecipeId = meal.RecipeId,
            DietTypeId = meal.DietTypeId,
            Disabled = meal.Disabled
        };

        public static RecipeViewModel ToRecipeViewModel(this Recipe recipe) => new()
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            DietTypeId = recipe.DietTypeId,
            Disabled = recipe.Disabled
        };

        public static RecipeDetailViewModel ToRecipeDetailViewModel(this Recipe recipe, IReadOnlyList<RecipeIngredient> ingredients) => new()
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            DietTypeId = recipe.DietTypeId,
            Disabled = recipe.Disabled,
            Ingredients = ingredients.Select(ri => ri.ToRecipeIngredientViewModel()).ToList()
        };

        public static RecipeIngredientViewModel ToRecipeIngredientViewModel(this RecipeIngredient ri) => new()
        {
            IngredientId = ri.IngredientId,
            Quantity = ri.Quantity,
            Unit = ri.Unit
        };

        public static IngredientViewModel ToIngredientViewModel(this Ingredient ingredient) => new()
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description
        };

        public static IngredientDetailViewModel ToIngredientDetailViewModel(
            this Ingredient ingredient,
            IReadOnlyList<string> allergyIds,
            IReadOnlyList<string> clinicalStateIds,
            IReadOnlyList<string> dietTypeIds) => new()
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            AllergyExclusionIds = allergyIds,
            ClinicalStateExclusionIds = clinicalStateIds,
            DietTypeExclusionIds = dietTypeIds
        };

        public static PatientRequestViewModel ToPatientRequestViewModel(this PatientRequest request) => new()
        {
            Id = request.Id,
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            RecipeId = request.RecipeId,
            RequestedForDate = request.RequestedForDate,
            ApprovalStatus = (MealRequestApprovalStatus)(int)request.ApprovalStatus,
            StatusReason = request.StatusReason,
            UnsafeIngredientId = request.UnsafeIngredientId
        };

        public static AllergyViewModel ToAllergyViewModel(this Allergy allergy) => new()
        {
            Id = allergy.Id,
            Name = allergy.Name
        };

        public static ClinicalStateViewModel ToClinicalStateViewModel(this ClinicalState clinicalState) => new()
        {
            Id = clinicalState.Id,
            Name = clinicalState.Name
        };

        public static DietTypeViewModel ToDietTypeViewModel(this DietType dietType) => new()
        {
            Id = dietType.Id,
            Name = dietType.Name
        };
    }
}
