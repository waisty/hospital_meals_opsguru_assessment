using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class IngredientDietTypeExclusions
    {
        public string IngredientId { get; set; } = "";
        public string DietTypeId { get; set; } = "";

        public static void Configure(EntityTypeBuilder<IngredientDietTypeExclusions> entity)
        {
            entity.ToTable("ingredient_diet_type_exclusions");
            entity.HasKey(e => new { e.IngredientId, e.DietTypeId });
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id").HasMaxLength(256);
            entity.Property(e => e.DietTypeId).HasColumnName("diet_type_id").HasMaxLength(256);
            entity.HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<DietType>()
                .WithMany()
                .HasForeignKey(e => e.DietTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
