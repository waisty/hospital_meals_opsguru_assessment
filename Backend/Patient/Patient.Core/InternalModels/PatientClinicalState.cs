using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.InternalModels
{
    internal class PatientClinicalState
    {
        public Guid PatientId { get; set; }
        public string ClinicalStateId { get; set; } = "";

        public PatientClinicalStateWithName ToPatientClinicalStateWithName(string clinicalStateName) => new()
        {
            PatientId = PatientId,
            ClinicalStateId = ClinicalStateId,
            ClinicalStateName = clinicalStateName
        };

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

    internal class PatientClinicalStateWithName : PatientClinicalState
    {
        public string ClinicalStateName { get; set; } = "";

        public PatientClinicalStateViewModel ToPatientClinicalStateViewModel() => new()
        {
            ClinicalStateId = ClinicalStateId,
            ClinicalStateName = ClinicalStateName
        };
    }
}
