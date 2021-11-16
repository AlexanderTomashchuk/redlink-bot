using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder.ToTable("ProductType");
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Name).HasMaxLength(100).IsRequired();

        builder.HasData(
            new ProductType(1, "Clothes"),
            new ProductType(2, "Outer wear"),
            new ProductType(3, "Lingerie"),
            new ProductType(4, "Foot wear"),
            new ProductType(5, "Bags"),
            new ProductType(6, "Accessories"),
            new ProductType(7, "jewelry"),
            new ProductType(8, "Clothes for home")
        );
    }
}