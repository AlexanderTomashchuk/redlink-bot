using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Country");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Flag).IsUnicode().IsRequired();
        builder.Property(c => c.DefaultCurrencyId).IsRequired();

        builder.HasOne(c => c.DefaultCurrency)
            .WithMany(c => c.Countries)
            .HasForeignKey(c => c.DefaultCurrencyId);

        builder.HasData(
            new Country(1, "USA", "US", "🇺🇸", 1),
            new Country(2, "Ukraine", "UA", "🇺🇦", 2),
            new Country(3, "Poland", "PL", "🇵🇱", 3),
            new Country(4, "Russia", "RU", "🇷🇺", 4)
        );
    }
}