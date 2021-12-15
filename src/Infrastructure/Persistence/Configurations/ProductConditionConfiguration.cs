using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConditionConfiguration : IEntityTypeConfiguration<ProductCondition>
{
    public void Configure(EntityTypeBuilder<ProductCondition> builder)
    {
        builder.ToTable("ProductCondition");
        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.NameLocalizationKey).HasMaxLength(100).IsRequired();

        builder.HasData(
            new ProductCondition(1, "New"),
            new ProductCondition(2, "Perfect"),
            new ProductCondition(3, "VeryGood"),
            new ProductCondition(4, "Good"),
            new ProductCondition(5, "Satisfactory")
        );
    }
}