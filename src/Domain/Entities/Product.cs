using System.Collections.Generic;
using Domain.Common;
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public long Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long? ConditionId { get; set; }

    public ProductCondition Condition { get; }

    public long? CategoryId { get; set; }
    public ProductCategory Category { get; }

    public decimal Price { get; set; }

    public long? CurrencyId { get; set; }
    public Currency Currency { get; }
 
    public ICollection<HashTag> HashTags { get; }

    public ICollection<File> Files { get; }

    public long SellerId { get; set; }
    public AppUser Seller { get; set; }

    public ProductState CurrentState { get; set; }
}