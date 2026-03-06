using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Patient.UIViewModels;

namespace Hospital.Patient.Core.InternalModels
{
    internal class Patient
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string Notes { get; set; } = "";

        public PatientViewModel ToPatientViewModel() => new()
        {
            Id = Id.ToString(),
            Name = Name,
            MobileNumber = MobileNumber,
            DietTypeId = DietTypeId,
            Notes = Notes
        };

        public PatientDetailViewModel ToPatientDetailViewModel(IReadOnlyList<string> allergyIds, IReadOnlyList<string> clinicalStateIds) => new()
        {
            Id = Id.ToString(),
            Name = Name,
            MobileNumber = MobileNumber,
            DietTypeId = DietTypeId,
            Notes = Notes,
            AllergyIds = allergyIds,
            ClinicalStateIds = clinicalStateIds
        };

        public static void Configure(EntityTypeBuilder<Patient> entity)
        {
            entity.ToTable("patients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.MobileNumber).HasColumnName("mobile_number").HasMaxLength(32).IsRequired();
            entity.HasIndex(e => e.MobileNumber).IsUnique();
            entity.Property(e => e.DietTypeId).HasColumnName("diet_type_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.HasOne<DietType>()
                .WithMany()
                .HasForeignKey(e => e.DietTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
