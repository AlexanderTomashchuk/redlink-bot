using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Extensions;

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public long Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long? ConditionId { get; set; }

    public ProductCondition Condition { get; }

    public long? TypeId { get; }

    public ProductType Type { get; set; }

    public decimal Price { get; set; }

    public long? CurrencyId { get; set; }
    public Currency Currency { get; set; }
 
    public ICollection<HashTag> HashTags { get; }

    public ICollection<File> Files { get; }

    public long SellerId { get; set; }
    public AppUser Seller { get; set; }

    public ProductState CurrentState { get; set; }
}