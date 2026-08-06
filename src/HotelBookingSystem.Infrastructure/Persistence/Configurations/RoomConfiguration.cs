using HotelBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
        builder.Property(r => r.RoomType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.PricePerNight).HasColumnType("decimal(10,2)");

        // Room (1) — Booking (*) : يمكن حجز الغرفة نفسها في فترات متعددة غير متعارضة
        builder.HasMany(r => r.Bookings)
               .WithOne(b => b.Room)
               .HasForeignKey(b => b.RoomId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
