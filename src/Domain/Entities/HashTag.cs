using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class HashTag : AuditableEntity
{
    public long Id { get; }

    //todo: implement localization for hashtags
    public string Value { get; }

    public ICollection<Product> Products { get; }

    // ReSharper disable once UnusedMember.Local
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