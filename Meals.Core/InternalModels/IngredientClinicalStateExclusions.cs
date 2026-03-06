using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class IngredientClinicalStateExclusions
    {
        public string IngredientId { get; set; } = "";
        public string ClinicalStateId { get; set; } = "";

        public static void Configure(EntityTypeBuilder<IngredientClinicalStateExclusions> entity)
        {
            entity.ToTable("ingredient_clinical_state_exclusions");
            entity.HasKey(e => new { e.IngredientId, e.ClinicalStateId });
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id").HasMaxLength(256);
            entity.Property(e => e.ClinicalStateId).HasColumnName("clinical_state_id").HasMaxLength(256);
            entity.HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ClinicalState>()
                .WithMany()
                .HasForeignKey(e => e.ClinicalStateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
