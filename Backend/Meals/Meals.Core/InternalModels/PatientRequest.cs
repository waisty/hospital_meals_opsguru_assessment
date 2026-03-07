using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.Core.InternalModels
{
    internal class PatientRequest
    {
        public Guid Id { get; set; }
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public DateTime RequestedDateTime { get; set; }
        public MealRequestAppprovalStatus ApprovalStatus { get; set; }
        public string? StatusReason { get; set; }
        public string? UnsafeIngredientId { get; set; }
        public DateTime? FinalizedDateTime { get; set; }
        public NpgsqlTsVector SearchVector { get; set; } = null!;

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
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(256);
            entity.Property(e => e.MiddleName).HasColumnName("middle_name").HasMaxLength(256);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(256);
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id").HasMaxLength(256).IsRequired();
            entity.Property(e => e.RequestedDateTime).HasColumnName("requested_date_time");
            entity.Property(e => e.ApprovalStatus).HasColumnName("approval_status").HasConversion<int>();
            entity.Property(e => e.StatusReason).HasColumnName("status_reason");
            entity.Property(e => e.UnsafeIngredientId).HasColumnName("unsafe_ingredient_id").HasMaxLength(256);
            entity.Property(e => e.FinalizedDateTime).HasColumnName("finalized_date_time");
            entity.HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasGeneratedTsVectorColumn(
                    e => e.SearchVector,
                    "simple",
                    e => new { e.FirstName, e.LastName })
                .HasIndex(e => e.SearchVector)
                .HasMethod("GIN");
            entity.Property(e => e.SearchVector).HasColumnName("search_vector");
        }
    }
}
