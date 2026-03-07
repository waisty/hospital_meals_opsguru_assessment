using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.InternalModels
{
    internal class PatientAllergy
    {
        public Guid PatientId { get; set; }
        public string AllergyId { get; set; } = "";

        public PatientAllergyWithName ToPatientAllergyWithName(string allergyName) => new()
        {
            PatientId = PatientId,
            AllergyId = AllergyId,
            AllergyName = allergyName
        };

        public static void Configure(EntityTypeBuilder<PatientAllergy> entity)
        {
            entity.ToTable("patient_allergies");
            entity.HasKey(e => new { e.PatientId, e.AllergyId });
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
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

    internal class PatientAllergyWithName : PatientAllergy
    {
        public string AllergyName { get; set; } = "";

        public PatientAllergyViewModel ToPatientAllergyViewModel() => new()
        {
            AllergyId = AllergyId,
            AllergyName = AllergyName
        };
    }
}
