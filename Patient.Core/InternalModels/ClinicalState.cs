using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Patient.Core.InternalModels
{
    internal class ClinicalState
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public static void Configure(EntityTypeBuilder<ClinicalState> entity)
        {
            entity.ToTable("clinical_state");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        }
    }
}
