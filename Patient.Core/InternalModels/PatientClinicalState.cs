using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Patient.Core.InternalModels
{
    internal class PatientClinicalState
    {
        public Guid PatientId { get; set; }
        public string ClinicalStateId { get; set; } = "";

        public static void Configure(EntityTypeBuilder<PatientClinicalState> entity)
        {
            entity.ToTable("patient_clinical_states");
            entity.HasKey(e => new { e.PatientId, e.ClinicalStateId });
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ClinicalStateId).HasColumnName("clinical_state_id").HasMaxLength(256);
            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ClinicalState>()
                .WithMany()
                .HasForeignKey(e => e.ClinicalStateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
