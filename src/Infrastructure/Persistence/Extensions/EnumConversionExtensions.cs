using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<TProperty> HasTypeToLowerStringConversion<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasConversion(
                v => v.ToString().ToLower(),
                v => (TProperty)Enum.Parse(typeof(TProperty), v, true));
        }
    }
}