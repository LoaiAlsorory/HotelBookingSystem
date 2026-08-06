using HotelBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        // User (1) — Booking (*) : كل مستخدم يمكنه إنشاء عدة حجوزات
        builder.HasMany(u => u.Bookings)
               .WithOne(b => b.User)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
