using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Auth.Core.InternalModels
{
    internal class User
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Admin { get; set; }
        public bool PatientAdmin { get; set; }
        public bool MealsAdmin { get; set; }
        public bool MealsUser { get; set; }
        public bool KitchenUser { get; set; }

        public static void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Username);
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Admin).HasColumnName("admin");
            entity.Property(e => e.PatientAdmin).HasColumnName("patient_admin");
            entity.Property(e => e.MealsAdmin).HasColumnName("meals_admin");
            entity.Property(e => e.MealsUser).HasColumnName("meals_user");
            entity.Property(e => e.KitchenUser).HasColumnName("kitchen_user");
        }
    }
}
