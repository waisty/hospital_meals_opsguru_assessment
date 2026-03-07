using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class IngredientAllergyExclusions
    {
        public string IngredientId { get; set; } = "";
        public string AllergyId { get; set; } = "";

        public static void Configure(EntityTypeBuilder<IngredientAllergyExclusions> entity)
        {
            entity.ToTable("ingredient_allergy_exclusions");
            entity.HasKey(e => new { e.IngredientId, e.AllergyId });
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id").HasMaxLength(256);
            entity.Property(e => e.AllergyId).HasColumnName("allergy_id").HasMaxLength(256);
            entity.HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Allergy>()
                .WithMany()
                .HasForeignKey(e => e.AllergyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
