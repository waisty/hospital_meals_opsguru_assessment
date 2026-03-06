using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Patient.Core.InternalModels
{
    internal class Patient
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string Notes { get; set; } = "";

        public static void Configure(EntityTypeBuilder<Patient> entity)
        {
            entity.ToTable("patient");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.DietTypeId).HasColumnName("diet_type_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.HasOne<DietType>()
                .WithMany()
                .HasForeignKey(e => e.DietTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
