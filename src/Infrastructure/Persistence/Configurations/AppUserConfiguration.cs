using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUser");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FirstName).IsUnicode();
        builder.Property(u => u.LastName).IsUnicode();
        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(u => u.InProgressChainWorkflowName).HasMaxLength(50);
        builder.Ignore(u => u.HasCountry);
        builder.Ignore(u => u.LanguageCodeName);

        builder.HasIndex(u => u.ChatId);

        builder.HasOne(u => u.Country)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CountryId);

        builder.HasOne(u => u.Language)
            .WithMany(l => l.Users)
            .HasForeignKey(u => u.LanguageCode);
    }
}