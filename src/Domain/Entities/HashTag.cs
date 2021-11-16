using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class HashTag : AuditableEntity
{
    public long Id { get; }

    public string Value { get; }

    public ICollection<Product> Products { get; }

    private HashTag()
    {
    }

    public HashTag(long id, string value, ICollection<Product> products = null)
    {
        Id = id;
        Value = value;
        Products = products;
    }
}