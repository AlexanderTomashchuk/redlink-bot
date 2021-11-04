using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Language");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Name).HasMaxLength(50).IsRequired();
            builder.Property(l => l.Code).HasMaxLength(20).IsRequired();
            builder.Property(l => l.Flag).IsUnicode().IsRequired();

            builder.HasData(
                new Language(1, "English", "en", "🇬🇧"),
                new Language(2, "Ukrainian", "uk", "🇺🇦"),
                new Language(3, "Russian", "ru", "🇷🇺")
            );
        }
    }
}