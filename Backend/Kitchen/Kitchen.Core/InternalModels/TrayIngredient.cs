using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Kitchen.Core.InternalModels
{
    internal class TrayIngredient
    {
        public Guid TrayId { get; set; }
        public string IngredientName { get; set; } = "";
        public decimal Qty { get; set; }
        public string? Unit { get; set; }

        public static void Configure(EntityTypeBuilder<TrayIngredient> entity)
        {
            entity.ToTable("tray_ingredients");
            entity.HasKey(e => new { e.TrayId, e.IngredientName });
            entity.Property(e => e.TrayId).HasColumnName("tray_id");
            entity.Property(e => e.IngredientName).HasColumnName("ingredient_name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Qty).HasColumnName("qty").HasPrecision(18, 4);
            entity.Property(e => e.Unit).HasColumnName("unit").HasMaxLength(64);
        }
    }
}
