using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("User");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).IsUnicode();
            builder.Property(u => u.LastName).IsUnicode();
            builder.Property(u => u.TelegramId).IsRequired();

            builder.HasIndex(u => u.TelegramId);
            builder.HasIndex(u => u.ChatId);

            builder.HasOne(u => u.Country)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CountryId);

            builder.HasOne(u => u.Language)
                .WithMany(l => l.Users)
                .HasForeignKey(u => u.LanguageId);
        }
    }
}