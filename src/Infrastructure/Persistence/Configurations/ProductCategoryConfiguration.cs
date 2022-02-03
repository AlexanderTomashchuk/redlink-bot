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
            new(1003, "Sneakers", parentId: 1),
            new(1004, "Uggs", parentId: 1),
            new(1005, "RubberisedBoots", parentId: 1),
            new(1006, "FlipFlops", parentId: 1),
            new(1007, "Shoes", parentId: 1),
            new(1008, "Ballerinas", parentId: 1),
            new(1009, "Loafers", parentId: 1),
            new(1010, "Sandals", parentId: 1),
            new(1011, "Moccasins", parentId: 1),
            new(1012, "Espadrilles", parentId: 1),
            new(1013, "Slippers", parentId: 1),
            new(1014, "Mules", parentId: 1),
            //bags
            new(2, "Bags"),
            new(2001, "Bags", parentId: 2),
            new(2002, "Backpacks", parentId: 2),
            new(2003, "ClutchBags", parentId: 2),
            new(2004, "ToiletryBags", parentId: 2),
            new(2005, "DocumentCovers", parentId: 2),
            new(2006, "Cardholders", parentId: 2),
            new(2007, "Wallets", parentId: 2),
            //clothes
            new(3, "Clothes"),
            new(3001, "Knitwear", parentId: 3),
            new(3002, "SweatshirtsHoodies", parentId: 3),
            new(3003, "Trousers", parentId: 3),
            new(3004, "TShirts", parentId: 3),
            new(3005, "Shirts", parentId: 3),
            new(3006, "Shorts", parentId: 3),
            new(3007, "SuitsJumpsuits", parentId: 3),
            new(3008, "Loungewear", parentId: 3),
            new(3009, "Sportswear", parentId: 3),
            new(3010, "Skirts", parentId: 3),
            new(3011, "Jeans", parentId: 3),
            new(3012, "Dresses", parentId: 3),
            //outerwear
            new(4, "Outerwear"),
            new(4001, "Coats", parentId: 4),
            new(4002, "TrenchCoats", parentId: 4),
            new(4003, "Jackets", parentId: 4),
            new(4004, "FurCoats", parentId: 4),
            new(4005, "Gilets", parentId: 4),
            new(4006, "Blazers", parentId: 4),
            new(4007, "PufferJackets", parentId: 4),
            new(4008, "Parkas", parentId: 4),
            new(4009, "DoubleFacedJackets", parentId: 4),
            //accessories
            new(5, "Accessories"),
            new(5001, "Glasses", parentId: 5),
            new(5002, "Headwears", parentId: 5),
            new(5003, "Belts", parentId: 5),
            new(5004, "Scarves", parentId: 5),
            new(5005, "Gloves", parentId: 5),
            new(5006, "Umbrellas", parentId: 5),
            //lingerie
            new(6, "Lingerie"),
            new(6001, "Bras", parentId: 6),
            new(6002, "Briefs", parentId: 6),
            new(6003, "Swimwear", parentId: 6),
            new(6004, "SocksTights", parentId: 6),
            new(6005, "Bodysuits", parentId: 6),
            new(6006, "Corsets", parentId: 6),
            //jewellery
            new(7, "Jewellery"),
            new(7001, "Necklaces", parentId: 7),
            new(7002, "Rings", parentId: 7),
            new(7003, "Bracelets", parentId: 7),
            new(7004, "LegBracelets", parentId: 7),
            new(7005, "Earrings", parentId: 7),
            new(7006, "EarCuffs", parentId: 7),
            new(7007, "HairClips", parentId: 7),
            //nightwear
            new(8, "Nightwear"),
            new(8001, "Pyjamas", parentId: 8),
            new(8002, "SlipDresses", parentId: 8),
            new(8003, "DressingGowns", parentId: 8),
            new(8004, "Slippers", parentId: 8),
            //dog clothes & accessories 
            new(9, "DogsClothesAndAccessories"),
            new(9001, "Clothes", parentId: 9),
            new(9002, "Accessories", parentId: 9),
            //perfumes
            new(10, "Perfumes"),
            //cosmetics & accessories
            new(11, "CosmeticsAndAccessories"),
        };

        builder.HasData(allCategories);
    }
}