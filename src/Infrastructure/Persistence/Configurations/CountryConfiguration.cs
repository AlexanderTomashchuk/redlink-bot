using Domain.Entities;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Country");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.NameLocalizationKey).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Flag).IsUnicode().IsRequired();
        builder.Property(l => l.DefaultLanguageCode).HasTypeToLowerStringConversion();

        builder.HasData(
            new Country(1, "USA", "US", "🇺🇸", Language.LanguageCode.En),
            new Country(2, "Ukraine", "UA", "🇺🇦", Language.LanguageCode.Uk),
            new Country(3, "Poland", "PL", "🇵🇱", Language.LanguageCode.En),
            new Country(4, "Russia", "RU", "🇷🇺", Language.LanguageCode.Ru)
        );
    }
}