using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Patient.Core.InternalModels
{
    internal class PatientAllergy
    {
        public string PatientId { get; set; } = "";
        public string AllergyId { get; set; } = "";

        public static void Configure(EntityTypeBuilder<PatientAllergy> entity)
        {
            entity.ToTable("patient_allergy");
            entity.HasKey(e => new { e.PatientId, e.AllergyId });
            entity.Property(e => e.PatientId).HasColumnName("patient_id").HasMaxLength(256);
            entity.Property(e => e.AllergyId).HasColumnName("allergy_id").HasMaxLength(256);
            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Allergy>()
                .WithMany()
                .HasForeignKey(e => e.AllergyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
