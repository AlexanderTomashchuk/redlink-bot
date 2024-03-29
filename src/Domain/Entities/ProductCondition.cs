using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class ProductCondition : AuditableEntity
{
    public long Id { get; }

    public string NameLocalizationKey { get; }

    public ICollection<Product> Products { get; }

    // ReSharper disable once UnusedMember.Local
    private ProductCondition()
    {
    }

    public ProductCondition(long id, string nameLocalizationKey, ICollection<Product> products = null)
    {
        Id = id;
        NameLocalizationKey = nameLocalizationKey;
        Products = products;
    }
}