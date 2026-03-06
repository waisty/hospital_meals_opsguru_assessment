using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.Core.InternalModels
{
    internal class PatientRequest
    {
        public Guid Id { get; set; }
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public DateTime RequestedForDate { get; set; }
        public MealRequestAppprovalStatus ApprovalStatus { get; set; }
        public string? StatusReason { get; set; }

        public static void Configure(EntityTypeBuilder<PatientRequest> entity)
        {
            entity.ToTable("patient_requests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.PatientId).HasColumnName("patient_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.PatientName).HasColumnName("patient_name").HasMaxLength(256);
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.RequestedForDate).HasColumnName("requested_for_date");
            entity.Property(e => e.ApprovalStatus).HasColumnName("approval_status").HasConversion<int>();
            entity.Property(e => e.StatusReason).HasColumnName("status_reason");
            entity.HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
