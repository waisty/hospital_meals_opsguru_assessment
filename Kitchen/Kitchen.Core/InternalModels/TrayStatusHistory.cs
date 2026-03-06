using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Kitchen.Core.InternalModels
{
    internal class TrayStatusHistory
    {
        public Guid Id { get; set; }
        public Guid TrayId { get; set; }
        public string Status { get; set; } = "";
        public DateTime Timestamp { get; set; }

        public static void Configure(EntityTypeBuilder<TrayStatusHistory> entity)
        {
            entity.ToTable("tray_status_history");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.TrayId).HasColumnName("tray_id");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(64).IsRequired();
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.HasIndex(e => e.TrayId);
            entity.HasOne<Tray>()
                .WithMany()
                .HasForeignKey(e => e.TrayId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
