using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategory");
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Id).ValueGeneratedOnAdd();
        builder.Property(pt => pt.NameLocalizationKey).HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        var allCategories = new List<ProductCategory>
        {
            //footwear
            new(1, "Footwear"),
            new(1001, "AnkleBoots", parentId: 1),
            new(1002, "Boots", parentId: 1),
            //bags
            new(2, "Bags"),
            new(2001, "Bags", parentId: 2),
            new(2002, "Backpacks", parentId: 2),
            new(2003, "ClutchBags", parentId: 2)
        };

        builder.HasData(allCategories); 
    }
}