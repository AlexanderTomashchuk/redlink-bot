using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currency");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Abbreviation).IsUnicode().IsRequired();
        builder.Property(c => c.Sign).IsUnicode().IsRequired();

        builder.HasData(
            new Currency(1, "USD", "$", "$"),
            new Currency(2, "UAH", "грн", "₴"),
            new Currency(3, "PLN", "zł", "zł"),
            new Currency(4, "RUB", "руб", "₽")
        );
    }
}