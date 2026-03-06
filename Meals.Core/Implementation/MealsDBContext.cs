using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using InternalModels = Hospital.Meals.Core.InternalModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class MealsDBContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public MealsDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<InternalModels.Allergy> Allergies { get; set; }
        public DbSet<InternalModels.ClinicalState> ClinicalStates { get; set; }
        public DbSet<InternalModels.DietType> DietTypes { get; set; }
        public DbSet<InternalModels.Ingredient> Ingredients { get; set; }
        public DbSet<InternalModels.IngredientAllergyExclusions> IngredientAllergyExclusions { get; set; }
        public DbSet<InternalModels.IngredientClinicalStateExclusions> IngredientClinicalStateExclusions { get; set; }
        public DbSet<InternalModels.IngredientDietTypeExclusions> IngredientDietTypeExclusions { get; set; }
        public DbSet<InternalModels.Recipe> Recipes { get; set; }
        public DbSet<InternalModels.RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<InternalModels.Meal> Meals { get; set; }
        public DbSet<InternalModels.PatientMealRequest> PatientMealRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration["ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<InternalModels.Allergy>(InternalModels.Allergy.Configure);
            modelBuilder.Entity<InternalModels.ClinicalState>(InternalModels.ClinicalState.Configure);
            modelBuilder.Entity<InternalModels.DietType>(InternalModels.DietType.Configure);
            modelBuilder.Entity<InternalModels.Ingredient>(InternalModels.Ingredient.Configure);
            modelBuilder.Entity<InternalModels.IngredientAllergyExclusions>(InternalModels.IngredientAllergyExclusions.Configure);
            modelBuilder.Entity<InternalModels.IngredientClinicalStateExclusions>(InternalModels.IngredientClinicalStateExclusions.Configure);
            modelBuilder.Entity<InternalModels.IngredientDietTypeExclusions>(InternalModels.IngredientDietTypeExclusions.Configure);
            modelBuilder.Entity<InternalModels.Recipe>(InternalModels.Recipe.Configure);
            modelBuilder.Entity<InternalModels.RecipeIngredient>(InternalModels.RecipeIngredient.Configure);
            modelBuilder.Entity<InternalModels.Meal>(InternalModels.Meal.Configure);
            modelBuilder.Entity<InternalModels.PatientMealRequest>(InternalModels.PatientMealRequest.Configure);
        }
    }
}
