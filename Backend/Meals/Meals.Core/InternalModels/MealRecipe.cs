using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class MealRecipe
    {
        public string MealId { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public bool Disabled { get; set; }

        public static void Configure(EntityTypeBuilder<MealRecipe> entity)
        {
            entity.ToTable("meal_recipes");
            entity.HasKey(e => new { e.MealId, e.RecipeId });
            entity.Property(e => e.MealId).HasColumnName("meal_id").HasMaxLength(256);
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id").HasMaxLength(256);
            entity.Property(e => e.Disabled).HasColumnName("disabled");

            entity.HasOne<Meal>()
                .WithMany()
                .HasForeignKey(e => e.MealId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
