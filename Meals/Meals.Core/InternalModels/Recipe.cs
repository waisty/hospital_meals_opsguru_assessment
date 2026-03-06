using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    internal class Recipe
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? DietTypeId { get; set; }
        public bool Disabled { get; set; }

        public static void Configure(EntityTypeBuilder<Recipe> entity)
        {
            entity.ToTable("recipes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DietTypeId).HasColumnName("diet_type_id").HasMaxLength(256);
            entity.Property(e => e.Disabled).HasColumnName("disabled");
            entity.HasOne<DietType>()
                .WithMany()
                .HasForeignKey(e => e.DietTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
