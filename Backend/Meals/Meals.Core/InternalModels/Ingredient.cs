using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace Hospital.Meals.Core.InternalModels
{
    internal class Ingredient
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public NpgsqlTsVector SearchVector { get; set; } = null!;

        public static void Configure(EntityTypeBuilder<Ingredient> entity)
        {
            entity.ToTable("ingredients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasColumnName("description");

            entity.HasGeneratedTsVectorColumn(
                    e => e.SearchVector,
                    "simple",
                    e => new { e.Name, e.Description })
                .HasIndex(e => e.SearchVector)
                .HasMethod("GIN");
            entity.Property(e => e.SearchVector).HasColumnName("search_vector");
        }
    }
}
