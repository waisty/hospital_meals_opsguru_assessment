using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Patient.UIViewModels;

namespace Hospital.Patient.Core.InternalModels
{
    internal class ClinicalState
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public ClinicalStateViewModel ToClinicalStateViewModel() => new()
        {
            Id = Id,
            Name = Name
        };

        public static void Configure(EntityTypeBuilder<ClinicalState> entity)
        {
            entity.ToTable("clinical_states");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        }
    }
}
