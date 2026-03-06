using System;
using Hospital.Kitchen.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Kitchen.Core.InternalModels
{
    internal class Tray
    {
        public Guid Id { get; set; }
        public Guid PatientMealRequestId { get; set; }
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string RecipeName { get; set; } = "";
        public Enums.TrayState State { get; set; }

        public List<TrayIngredient> TrayIngredients { get; set; } = [];

        public static void Configure(EntityTypeBuilder<Tray> entity)
        {
            entity.ToTable("trays");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.PatientMealRequestId).HasColumnName("patient_meal_request_id");
            entity.HasIndex(e => e.PatientMealRequestId);
            entity.Property(e => e.PatientId).HasColumnName("patient_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.PatientName).HasColumnName("patient_name").HasMaxLength(256);
            entity.Property(e => e.RecipeName).HasColumnName("recipe_name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.State).HasColumnName("state").HasConversion<int>().IsRequired();
            entity.HasMany(e => e.TrayIngredients)
                .WithOne()
                .HasForeignKey(e => e.TrayId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
