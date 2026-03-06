using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Meals.Core.InternalModels
{
    //NOTE: this will only be populated via the patient service and should not have a UI view model
    internal class DietType
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public static void Configure(EntityTypeBuilder<DietType> entity)
        {
            entity.ToTable("diet_types");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        }
    }
}
