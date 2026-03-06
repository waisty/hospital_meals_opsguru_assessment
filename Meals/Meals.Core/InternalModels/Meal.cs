using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class Meal
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public string? DietTypeId { get; set; }
        public bool Disabled { get; set; }

        public static void Configure(EntityTypeBuilder<Meal> entity)
        {
            entity.ToTable("meals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.DietTypeId).HasColumnName("diet_type_id").HasMaxLength(256);
            entity.Property(e => e.Disabled).HasColumnName("disabled");
            entity.HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<DietType>()
                .WithMany()
                .HasForeignKey(e => e.DietTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
