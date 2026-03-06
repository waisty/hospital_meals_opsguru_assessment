using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Patient.UIViewModels;

namespace Hospital.Patient.Core.InternalModels
{
    internal class Allergy
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public AllergyViewModel ToAllergyViewModel() => new()
        {
            Id = Id,
            Name = Name
        };

        public static void Configure(EntityTypeBuilder<Allergy> entity)
        {
            entity.ToTable("allergies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        }
    }
}
