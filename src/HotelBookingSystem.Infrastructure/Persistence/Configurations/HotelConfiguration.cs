using HotelBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Infrastructure.Persistence.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).IsRequired().HasMaxLength(150);
        builder.Property(h => h.City).IsRequired().HasMaxLength(100);
        builder.Property(h => h.Address).IsRequired().HasMaxLength(250);

        // Hotel (1) — Room (*) : كل فندق يحتوي عدد غير محدود من الغرف
        builder.HasMany(h => h.Rooms)
               .WithOne(r => r.Hotel)
               .HasForeignKey(r => r.HotelId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
