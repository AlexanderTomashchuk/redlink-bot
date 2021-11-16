using System.Linq;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class HashTagConfiguration : IEntityTypeConfiguration<HashTag>
{
    public void Configure(EntityTypeBuilder<HashTag> builder)
    {
        builder.ToTable("HashTag");
        builder.HasKey(ht => ht.Id);
        builder.Property(ht => ht.Value).HasMaxLength(50).IsRequired();

        var hashTags = new[]
            {
                "куртки",
                "штаны",
                "б/у"
            }
            .Select((tag, index) => new HashTag(index + 1, $"#{tag}"));

        builder.HasData(hashTags);
    }
}