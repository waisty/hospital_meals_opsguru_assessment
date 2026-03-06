using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Patient.UIViewModels;

namespace Hospital.Patient.Core.InternalModels
{
    internal class DietType
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public DietTypeViewModel ToDietTypeViewModel() => new()
        {
            Id = Id,
            Name = Name
        };

        public static void Configure(EntityTypeBuilder<DietType> entity)
        {
            entity.ToTable("diet_types");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        }
    }
}
