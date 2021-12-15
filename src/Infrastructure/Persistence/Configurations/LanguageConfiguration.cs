using Domain.Entities;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Language");

        builder.HasKey(l => l.Code);
        builder.Property(l => l.Code).HasTypeToLowerStringConversion();

        builder.Property(l => l.NameLocalizationKey).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Code).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Flag).IsUnicode().IsRequired();

        builder.HasData(
            new Language(Language.LanguageCode.En, "English", "🇬🇧"),
            new Language(Language.LanguageCode.Uk, "Ukrainian", "🇺🇦"),
            new Language(Language.LanguageCode.Ru, "Russian", "🇷🇺")
        );
    }
}