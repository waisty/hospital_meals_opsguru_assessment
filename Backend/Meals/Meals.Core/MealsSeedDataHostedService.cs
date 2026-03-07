using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Meals.Core.Implementation
{
    /// <summary>
    /// Seeds reference data (diet types, allergies, clinical states, ingredients, allergy/clinical/diet-type exclusions,
    /// recipes, recipe ingredients, and meals) when the database is empty or when SeedData:Enabled is true.
    /// Uses the same IDs for allergies, clinical states, and diet types as the Patient WebApi.
    /// </summary>
    internal sealed class MealsSeedDataHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MealsSeedDataHostedService> _logger;

        public MealsSeedDataHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<MealsSeedDataHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var enabledByConfig = string.Equals(
                _configuration["SeedData:Enabled"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<MealsDBContext>();

            bool databaseEmpty = false;
            try
            {
                databaseEmpty = await db.Allergies.CountAsync(cancellationToken).ConfigureAwait(false) == 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not check if meals database is empty; skipping seed.");
                return;
            }

            if (!enabledByConfig && !databaseEmpty)
            {
                _logger.LogDebug("Meals seed data skipped: database has data and SeedData:Enabled is not set.");
                return;
            }

            await SeedDietTypesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedAllergiesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedClinicalStatesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientAllergyExclusionsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientClinicalStateExclusionsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientDietTypeExclusionsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedRecipesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedRecipeIngredientsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedMealsAsync(db, cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task SeedDietTypesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                new DietType { Id = "REGULAR", Name = "Regular" },
                new DietType { Id = "VEGETARIAN", Name = "Vegetarian" },
                new DietType { Id = "DIABETIC", Name = "Diabetic" },
                new DietType { Id = "LOW-SODIUM", Name = "Low Sodium" },
            };
            foreach (var entity in seed)
            {
                try
                {
                    db.DietTypes.Add(entity);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed diet type added: {Id} ({Name})", entity.Id, entity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for diet type {Id}; continuing.", entity.Id);
                }
            }
        }

        private async Task SeedAllergiesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                new Allergy { Id = "NUTS", Name = "Tree Nuts" },
                new Allergy { Id = "DAIRY", Name = "Dairy" },
                new Allergy { Id = "GLUTEN", Name = "Gluten" },
                new Allergy { Id = "SHELLFISH", Name = "Shellfish" },
            };
            foreach (var entity in seed)
            {
                try
                {
                    db.Allergies.Add(entity);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed allergy added: {Id} ({Name})", entity.Id, entity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for allergy {Id}; continuing.", entity.Id);
                }
            }
        }

        private async Task SeedClinicalStatesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                new ClinicalState { Id = "DIABETIC", Name = "Diabetic" },
                new ClinicalState { Id = "HYPERTENSION", Name = "Hypertension" },
                new ClinicalState { Id = "CARDIAC", Name = "Cardiac" },
                new ClinicalState { Id = "RENAL", Name = "Renal" },
            };
            foreach (var entity in seed)
            {
                try
                {
                    db.ClinicalStates.Add(entity);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed clinical state added: {Id} ({Name})", entity.Id, entity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for clinical state {Id}; continuing.", entity.Id);
                }
            }
        }

        private async Task SeedIngredientsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                new Ingredient { Id = "MILK", Name = "Milk", Description = "Whole or skim milk" },
                new Ingredient { Id = "FLOUR", Name = "Wheat Flour", Description = "All-purpose flour" },
                new Ingredient { Id = "ALMOND", Name = "Almonds", Description = "Tree nuts" },
                new Ingredient { Id = "SHRIMP", Name = "Shrimp", Description = "Shellfish" },
                new Ingredient { Id = "SALT", Name = "Salt", Description = "Table salt" },
                new Ingredient { Id = "SUGAR", Name = "Sugar", Description = "Granulated sugar" },
                new Ingredient { Id = "CHICKEN", Name = "Chicken Breast", Description = "Skinless chicken" },
                new Ingredient { Id = "RICE", Name = "Rice", Description = "White or brown rice" },
                new Ingredient { Id = "BUTTER", Name = "Butter", Description = "Dairy butter" },
                new Ingredient { Id = "BREAD", Name = "Bread", Description = "Wheat bread" },
                new Ingredient { Id = "EGG", Name = "Egg", Description = "Chicken egg" },
                new Ingredient { Id = "TOMATO", Name = "Tomato", Description = "Fresh tomato" },
                new Ingredient { Id = "LETTUCE", Name = "Lettuce", Description = "Leaf lettuce" },
                new Ingredient { Id = "BEANS", Name = "Black Beans", Description = "Canned or cooked" },
            };
            foreach (var entity in seed)
            {
                try
                {
                    db.Ingredients.Add(entity);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed ingredient added: {Id} ({Name})", entity.Id, entity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for ingredient {Id}; continuing.", entity.Id);
                }
            }
        }

        private async Task SeedIngredientAllergyExclusionsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                (IngredientId: "MILK", AllergyId: "DAIRY"),
                (IngredientId: "BUTTER", AllergyId: "DAIRY"),
                (IngredientId: "FLOUR", AllergyId: "GLUTEN"),
                (IngredientId: "BREAD", AllergyId: "GLUTEN"),
                (IngredientId: "ALMOND", AllergyId: "NUTS"),
                (IngredientId: "SHRIMP", AllergyId: "SHELLFISH"),
            };
            foreach (var (ingredientId, allergyId) in seed)
            {
                try
                {
                    db.IngredientAllergyExclusions.Add(new IngredientAllergyExclusions { IngredientId = ingredientId, AllergyId = allergyId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed ingredient_allergy_exclusion failed for {IngredientId}+{AllergyId}.", ingredientId, allergyId);
                }
            }
        }

        private async Task SeedIngredientClinicalStateExclusionsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                (IngredientId: "SALT", ClinicalStateId: "HYPERTENSION"),
                (IngredientId: "SALT", ClinicalStateId: "RENAL"),
                (IngredientId: "SUGAR", ClinicalStateId: "DIABETIC"),
            };
            foreach (var (ingredientId, clinicalStateId) in seed)
            {
                try
                {
                    db.IngredientClinicalStateExclusions.Add(new IngredientClinicalStateExclusions { IngredientId = ingredientId, ClinicalStateId = clinicalStateId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed ingredient_clinical_state_exclusion failed for {IngredientId}+{ClinicalStateId}.", ingredientId, clinicalStateId);
                }
            }
        }

        private async Task SeedIngredientDietTypeExclusionsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            // Diet type exclusions: ingredients that a diet type cannot have (e.g. vegetarians exclude eggs, chicken, fish).
            var seed = new[]
            {
                (IngredientId: "EGG", DietTypeId: "VEGETARIAN"),
                (IngredientId: "CHICKEN", DietTypeId: "VEGETARIAN"),
                (IngredientId: "SHRIMP", DietTypeId: "VEGETARIAN"),
            };
            foreach (var (ingredientId, dietTypeId) in seed)
            {
                try
                {
                    db.IngredientDietTypeExclusions.Add(new IngredientDietTypeExclusions { IngredientId = ingredientId, DietTypeId = dietTypeId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed ingredient_diet_type_exclusion failed for {IngredientId}+{DietTypeId}.", ingredientId, dietTypeId);
                }
            }
        }

        private async Task SeedRecipesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                new Recipe { Id = "REGULAR-BREAKFAST", Name = "Breakfast Scramble", Description = "Eggs with toast" },
                new Recipe { Id = "VEGETARIAN-LUNCH", Name = "Garden Salad Bowl", Description = "Mixed greens with beans" },
                new Recipe { Id = "DIABETIC-DINNER", Name = "Grilled Chicken & Rice", Description = "Lean protein with rice" },
                new Recipe { Id = "LOW-SODIUM-SOUP", Name = "Vegetable Soup", Description = "Low-sodium vegetable soup" },
            };
            foreach (var entity in seed)
            {
                try
                {
                    db.Recipes.Add(entity);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed recipe added: {Id} ({Name})", entity.Id, entity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for recipe {Id}; continuing.", entity.Id);
                }
            }
        }

        private async Task SeedRecipeIngredientsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                (RecipeId: "REGULAR-BREAKFAST", IngredientId: "EGG", Quantity: 2m, Unit: "each"),
                (RecipeId: "REGULAR-BREAKFAST", IngredientId: "BREAD", Quantity: 2m, Unit: "slices"),
                (RecipeId: "REGULAR-BREAKFAST", IngredientId: "BUTTER", Quantity: 10m, Unit: "g"),
                (RecipeId: "VEGETARIAN-LUNCH", IngredientId: "LETTUCE", Quantity: 50m, Unit: "g"),
                (RecipeId: "VEGETARIAN-LUNCH", IngredientId: "TOMATO", Quantity: 1m, Unit: "each"),
                (RecipeId: "VEGETARIAN-LUNCH", IngredientId: "BEANS", Quantity: 100m, Unit: "g"),
                (RecipeId: "DIABETIC-DINNER", IngredientId: "CHICKEN", Quantity: 150m, Unit: "g"),
                (RecipeId: "DIABETIC-DINNER", IngredientId: "RICE", Quantity: 100m, Unit: "g"),
                (RecipeId: "LOW-SODIUM-SOUP", IngredientId: "TOMATO", Quantity: 2m, Unit: "each"),
                (RecipeId: "LOW-SODIUM-SOUP", IngredientId: "LETTUCE", Quantity: 30m, Unit: "g"),
                (RecipeId: "LOW-SODIUM-SOUP", IngredientId: "BEANS", Quantity: 50m, Unit: "g"),
            };
            foreach (var (recipeId, ingredientId, quantity, unit) in seed)
            {
                try
                {
                    db.RecipeIngredients.Add(new RecipeIngredient { RecipeId = recipeId, IngredientId = ingredientId, Quantity = quantity, Unit = unit });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed recipe_ingredient failed for {RecipeId}+{IngredientId}.", recipeId, ingredientId);
                }
            }
        }

        private async Task SeedMealsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seed = new[]
            {
                new Meal { Id = "MEAL-BREAKFAST", Name = "Breakfast Scramble", RecipeId = "REGULAR-BREAKFAST" },
                new Meal { Id = "MEAL-SALAD", Name = "Garden Salad Bowl", RecipeId = "VEGETARIAN-LUNCH" },
                new Meal { Id = "MEAL-CHICKEN-RICE", Name = "Grilled Chicken & Rice", RecipeId = "DIABETIC-DINNER" },
                new Meal { Id = "MEAL-VEG-SOUP", Name = "Vegetable Soup", RecipeId = "LOW-SODIUM-SOUP" },
            };
            foreach (var entity in seed)
            {
                try
                {
                    db.Meals.Add(entity);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed meal added: {Id} ({Name})", entity.Id, entity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for meal {Id}; continuing.", entity.Id);
                }
            }
        }

    }
}
