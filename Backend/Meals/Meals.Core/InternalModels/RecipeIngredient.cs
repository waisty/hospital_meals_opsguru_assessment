using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Meals.Core.InternalModels
{
    internal class RecipeIngredient
    {
        public string RecipeId { get; set; } = "";
        public string IngredientId { get; set; } = "";
        
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }

        public static void Configure(EntityTypeBuilder<RecipeIngredient> entity)
        {
            entity.ToTable("recipe_ingredients");
            entity.HasKey(e => new { e.RecipeId, e.IngredientId });
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id").HasMaxLength(256);
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id").HasMaxLength(256);
            entity.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(18, 4);
            entity.Property(e => e.Unit).HasColumnName("unit").HasMaxLength(64);
            entity.HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class RecipeIngredientWithName : RecipeIngredient
    {
        public string IngredientName { get; set; } = "";
    }
}
