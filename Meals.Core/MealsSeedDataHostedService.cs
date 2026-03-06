using Hospital.Meals.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Hospital.Meals.Core.Implementation;

namespace Hospital.Meals.Core
{
    /// <summary>
    /// Seeds initial reference data for meals (allergies, clinical states, diet types matching Patient.WebApi IDs),
    /// ingredients, ingredient_allergy_exclusions, ingredient_clinical_state_exclusions, ingredient_diet_type_exclusions, recipes, recipe_ingredients, and meals
    /// when the database is empty or when SeedData:Enabled is true.
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
                databaseEmpty = await db.DietTypes.CountAsync(cancellationToken).ConfigureAwait(false) == 0;
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

            // Same IDs as Patient.WebApi seed data
            await SeedAllergiesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedClinicalStatesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedDietTypesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientAllergyExclusionsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientClinicalStateExclusionsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedIngredientDietTypeExclusionsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedRecipesAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedRecipeIngredientsAsync(db, cancellationToken).ConfigureAwait(false);
            await SeedMealsAsync(db, cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task SeedAllergiesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seedAllergies = new[]
            {
                new Allergy { Id = "NUTS", Name = "Tree Nuts" },
                new Allergy { Id = "DAIRY", Name = "Dairy" },
                new Allergy { Id = "GLUTEN", Name = "Gluten" },
                new Allergy { Id = "SHELLFISH", Name = "Shellfish" },
            };

            foreach (var allergy in seedAllergies)
            {
                try
                {
                    db.Allergies.Add(allergy);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed allergy added: {Id} ({Name})", allergy.Id, allergy.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for allergy {Id}; continuing.", allergy.Id);
                }
            }
        }

        private async Task SeedClinicalStatesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seedClinicalStates = new[]
            {
                new ClinicalState { Id = "DIABETIC", Name = "Diabetic" },
                new ClinicalState { Id = "HYPERTENSION", Name = "Hypertension" },
                new ClinicalState { Id = "CARDIAC", Name = "Cardiac" },
                new ClinicalState { Id = "RENAL", Name = "Renal" },
            };

            foreach (var clinicalState in seedClinicalStates)
            {
                try
                {
                    db.ClinicalStates.Add(clinicalState);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed clinical state added: {Id} ({Name})", clinicalState.Id, clinicalState.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for clinical state {Id}; continuing.", clinicalState.Id);
                }
            }
        }

        private async Task SeedDietTypesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seedDietTypes = new[]
            {
                new DietType { Id = "REGULAR", Name = "Regular" },
                new DietType { Id = "VEGETARIAN", Name = "Vegetarian" },
                new DietType { Id = "DIABETIC", Name = "Diabetic" },
                new DietType { Id = "LOW-SODIUM", Name = "Low Sodium" },
            };

            foreach (var dietType in seedDietTypes)
            {
                try
                {
                    db.DietTypes.Add(dietType);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed diet type added: {Id} ({Name})", dietType.Id, dietType.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for diet type {Id}; continuing.", dietType.Id);
                }
            }
        }

        private async Task SeedIngredientsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seedIngredients = new[]
            {
                new Ingredient { Id = "OATMEAL", Name = "Oatmeal", Description = "Rolled oats" },
                new Ingredient { Id = "MILK", Name = "Milk", Description = "Dairy milk" },
                new Ingredient { Id = "ALMOND", Name = "Almonds", Description = "Tree nuts" },
                new Ingredient { Id = "BREAD", Name = "Wheat Bread", Description = "Contains gluten" },
                new Ingredient { Id = "SHRIMP", Name = "Shrimp", Description = "Shellfish" },
                new Ingredient { Id = "RICE", Name = "Rice", Description = "White or brown rice" },
                new Ingredient { Id = "CHICKEN", Name = "Chicken", Description = "Poultry" },
                new Ingredient { Id = "SALT", Name = "Salt", Description = "Sodium chloride" },
                new Ingredient { Id = "EGG", Name = "Egg", Description = "Chicken egg" },
            };

            foreach (var ingredient in seedIngredients)
            {
                try
                {
                    db.Ingredients.Add(ingredient);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed ingredient added: {Id} ({Name})", ingredient.Id, ingredient.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for ingredient {Id}; continuing.", ingredient.Id);
                }
            }
        }

        private async Task SeedIngredientAllergyExclusionsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var links = new[] // ingredient_id, allergy_id (same IDs as Patient)
            {
                ("MILK", "DAIRY"),
                ("ALMOND", "NUTS"),
                ("BREAD", "GLUTEN"),
                ("SHRIMP", "SHELLFISH"),
                ("EGG", "DAIRY"), // some diets treat egg as dairy-like for allergy purposes; add if you use a different allergy
            };

            foreach (var (ingredientId, allergyId) in links)
            {
                try
                {
                    db.IngredientAllergyExclusions.Add(new IngredientAllergyExclusions { IngredientId = ingredientId, AllergyId = allergyId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed ingredient_allergy_exclusions failed for {IngredientId}, {AllergyId}.", ingredientId, allergyId);
                }
            }
        }

        private async Task SeedIngredientClinicalStateExclusionsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var links = new[] // ingredient_id, clinical_state_id (same IDs as Patient)
            {
                ("SALT", "HYPERTENSION"),
                ("SALT", "CARDIAC"),
                ("SALT", "RENAL"),
                ("ALMOND", "RENAL"), // high potassium example
            };

            foreach (var (ingredientId, clinicalStateId) in links)
            {
                try
                {
                    db.IngredientClinicalStateExclusions.Add(new IngredientClinicalStateExclusions { IngredientId = ingredientId, ClinicalStateId = clinicalStateId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed ingredient_clinical_state_exclusions failed for {IngredientId}, {ClinicalStateId}.", ingredientId, clinicalStateId);
                }
            }
        }

        private async Task SeedIngredientDietTypeExclusionsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var links = new[] // ingredient_id, diet_type_id (same IDs as Patient)
            {
                ("CHICKEN", "VEGETARIAN"),
                ("SHRIMP", "VEGETARIAN"),
                ("EGG", "VEGETARIAN"),
                ("SALT", "LOW-SODIUM"),
            };

            foreach (var (ingredientId, dietTypeId) in links)
            {
                try
                {
                    db.IngredientDietTypeExclusions.Add(new IngredientDietTypeExclusions { IngredientId = ingredientId, DietTypeId = dietTypeId });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed ingredient_diet_type_exclusions failed for {IngredientId}, {DietTypeId}.", ingredientId, dietTypeId);
                }
            }
        }

        private async Task SeedRecipesAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seedRecipes = new[]
            {
                new Recipe { Id = "OATMEAL-BREAKFAST", Name = "Oatmeal Breakfast", Description = "Simple oatmeal", DietTypeId = "REGULAR" },
                new Recipe { Id = "RICE-CHICKEN", Name = "Rice and Chicken", Description = "Plain rice with chicken", DietTypeId = "REGULAR" },
                new Recipe { Id = "LOW-SODIUM-RICE", Name = "Plain Rice Bowl", Description = "Low sodium option", DietTypeId = "LOW-SODIUM" },
                new Recipe { Id = "DIABETIC-OPTION", Name = "Diabetic Friendly Meal", Description = "Suitable for diabetic diet", DietTypeId = "DIABETIC" },
            };

            foreach (var recipe in seedRecipes)
            {
                try
                {
                    db.Recipes.Add(recipe);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed recipe added: {Id} ({Name})", recipe.Id, recipe.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for recipe {Id}; continuing.", recipe.Id);
                }
            }
        }

        private async Task SeedRecipeIngredientsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var links = new[]
            {
                (RecipeId: "OATMEAL-BREAKFAST", IngredientId: "OATMEAL", Quantity: 1m, Unit: "cup"),
                (RecipeId: "OATMEAL-BREAKFAST", IngredientId: "MILK", Quantity: 0.5m, Unit: "cup"),
                (RecipeId: "RICE-CHICKEN", IngredientId: "RICE", Quantity: 1m, Unit: "cup"),
                (RecipeId: "RICE-CHICKEN", IngredientId: "CHICKEN", Quantity: 100m, Unit: "g"),
                (RecipeId: "LOW-SODIUM-RICE", IngredientId: "RICE", Quantity: 1m, Unit: "cup"),
                (RecipeId: "DIABETIC-OPTION", IngredientId: "RICE", Quantity: 0.5m, Unit: "cup"),
                (RecipeId: "DIABETIC-OPTION", IngredientId: "CHICKEN", Quantity: 80m, Unit: "g"),
            };

            foreach (var (RecipeId, IngredientId, Quantity, Unit) in links)
            {
                try
                {
                    db.RecipeIngredients.Add(new RecipeIngredient { RecipeId = RecipeId, IngredientId = IngredientId, Quantity = Quantity, Unit = Unit });
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed recipe_ingredients failed for {RecipeId}, {IngredientId}.", RecipeId, IngredientId);
                }
            }
        }

        private async Task SeedMealsAsync(MealsDBContext db, CancellationToken cancellationToken)
        {
            var seedMeals = new[]
            {
                new Meal { Id = "MEAL-OATMEAL", Name = "Oatmeal Breakfast", RecipeId = "OATMEAL-BREAKFAST", DietTypeId = "REGULAR" },
                new Meal { Id = "MEAL-RICE-CHICKEN", Name = "Rice and Chicken", RecipeId = "RICE-CHICKEN", DietTypeId = "REGULAR" },
                new Meal { Id = "MEAL-LOW-SODIUM", Name = "Low Sodium Rice Bowl", RecipeId = "LOW-SODIUM-RICE", DietTypeId = "LOW-SODIUM" },
                new Meal { Id = "MEAL-DIABETIC", Name = "Diabetic Friendly", RecipeId = "DIABETIC-OPTION", DietTypeId = "DIABETIC" },
            };

            foreach (var meal in seedMeals)
            {
                try
                {
                    db.Meals.Add(meal);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed meal added: {Id} ({Name})", meal.Id, meal.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for meal {Id}; continuing.", meal.Id);
                }
            }
        }
    }
}
