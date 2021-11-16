using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsUnicode();
        builder.Property(p => p.Description).IsUnicode();

        builder.HasOne(p => p.Condition)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.ConditionId);

        builder.HasOne(p => p.Type)
            .WithMany(t => t.Products)
            .HasForeignKey(p => p.TypeId);

        builder.HasOne(p => p.Currency)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CurrencyId);

        builder.HasMany<HashTag>(p => p.HashTags)
            .WithMany(ht => ht.Products)
            .UsingEntity(j => j.ToTable("ProductHashTags"));

        builder.HasMany(p => p.Files)
            .WithOne(f => f.Product)
            .HasForeignKey(f => f.ProductId);

        builder.HasOne(p => p.Seller)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.SellerId);
    }
}