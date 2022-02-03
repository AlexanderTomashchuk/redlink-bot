using System.Collections.Generic;
using System.Linq;
using Domain.Common;

namespace Domain.Entities;

public class ProductCategory : AuditableEntity
{
    public long Id { get; }

    public string NameLocalizationKey { get; }

    public long? ParentId { get; set; }

    public ProductCategory Parent { get; set; }

    public ICollection<ProductCategory> SubCategories { get; }

    public ICollection<Product> Products { get; }

    public bool HasSubCategories => SubCategories != null && SubCategories.Any();

    public bool IsRootCategory => ParentId != null;

    // ReSharper disable once UnusedMember.Local
    private ProductCategory()
    {
    }

    public ProductCategory(long id, string nameLocalizationKey, long? parentId = default,
        ICollection<ProductCategory> subCategories = default,
        ICollection<Product> products = null)
    {
        Id = id;
        NameLocalizationKey = nameLocalizationKey;
        ParentId = parentId;
        SubCategories = subCategories;
        Products = products;
    }
}