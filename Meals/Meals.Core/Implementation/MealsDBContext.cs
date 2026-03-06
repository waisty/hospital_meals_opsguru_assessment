using Hospital.Meals.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hospital.Meals.Core.Implementation
{
    internal class MealsDBContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MealsDBContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<ClinicalState> ClinicalStates { get; set; }
        public DbSet<DietType> DietTypes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientAllergyExclusions> IngredientAllergyExclusions { get; set; }
        public DbSet<IngredientClinicalStateExclusions> IngredientClinicalStateExclusions { get; set; }
        public DbSet<IngredientDietTypeExclusions> IngredientDietTypeExclusions { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<PatientMealRequest> PatientMealRequests { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this.configuration["ConnectionString"]);
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
            modelBuilder.Entity<InternalModels.Meal>(InternalModels.Meal.Configure);
            modelBuilder.Entity<InternalModels.PatientMealRequest>(InternalModels.PatientMealRequest.Configure);
            modelBuilder.Entity<InternalModels.Recipe>(InternalModels.Recipe.Configure);
            modelBuilder.Entity<InternalModels.RecipeIngredient>(InternalModels.RecipeIngredient.Configure);
        }
    }
}
