using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class ProductType : AuditableEntity
{
    public long Id { get; }

    public string Name { get; }

    public ICollection<Product> Products { get; }

    private ProductType()
    {
    }

    public ProductType(long id, string name, ICollection<Product> products = null)
    {
        Id = id;
        Name = name;
        Products = products;
    }
}