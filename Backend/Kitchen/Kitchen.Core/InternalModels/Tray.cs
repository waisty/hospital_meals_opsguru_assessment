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
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string RecipeName { get; set; } = "";
        public Enums.TrayState State { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }

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
            entity.HasIndex(e => e.PatientMealRequestId).IsUnique();
            entity.Property(e => e.PatientId).HasColumnName("patient_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.MiddleName).HasColumnName("middle_name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(256).IsRequired();
            entity.HasIndex(e => e.FirstName);
            entity.HasIndex(e => e.MiddleName);
            entity.HasIndex(e => e.LastName);
            entity.Property(e => e.RecipeName).HasColumnName("recipe_name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.State).HasColumnName("state").HasConversion<int>().IsRequired();
            entity.Property(e => e.ReceivedDateTime).HasColumnName("received_date_time").IsRequired();
            entity.Property(e => e.LastUpdateDateTime).HasColumnName("last_update_date_time");
            entity.HasMany(e => e.TrayIngredients)
                .WithOne()
                .HasForeignKey(e => e.TrayId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
