using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Patient.Core.InternalModels
{
    internal class Allergy
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public static void Configure(EntityTypeBuilder<Allergy> entity)
        {
            entity.ToTable("allergy");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        }
    }
}
