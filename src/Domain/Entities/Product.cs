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

    public long? CurrencyId { get; }
    public Currency Currency { get; set; }
 
    public ICollection<HashTag> HashTags { get; }

    public ICollection<File> Files { get; }

    public long SellerId { get; set; }
    public AppUser Seller { get; set; }

    public ProductState CurrentState { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        //todo: ot localization
        //todo: move to application
        sb.AppendLine($"*{Name.Escape()}*");
        sb.AppendLine();
        //sb.AppendLine($"💰 _{string.Join(' ', Price, Currency.Abbreviation).Escape()}_");
        sb.AppendLine($"💰 _{string.Join(' ', Price).Escape()}_");
        sb.AppendLine();
        //var hashTags = string.Join(" ", HashTags).Escape();
        //sb.AppendLine(hashTags);
        //sb.AppendLine();
        //sb.AppendLine($"{Description.Escape()}");
        //sb.AppendLine();
        //sb.AppendLine($"Раздел: {Type.Name.Escape()}");
        //todo: localization for condition
        //sb.AppendLine($"Состояние: {Condition.Name.Escape()}");
        sb.AppendLine($"Продавец: [{Seller.FirstName} {Seller.LastName}](tg://user?id={Seller.Id})");

        return sb.ToString();
    }
}