using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Extensions;

namespace Domain.Entities
{
    public class Product : AuditableEntity
    {
        public long Id { get; }

        public string Name { get; }

        public string Description { get; }

        public long ConditionId { get; }

        public ProductCondition Condition { get; }

        public long TypeId { get; }

        public ProductType Type { get; }

        public decimal Price { get; }

        public long CurrencyId { get; }
        public Currency Currency { get; }

        public ICollection<HashTag> HashTags { get; }

        public ICollection<File> Files { get; }

        public long SellerId { get; }
        public User Seller { get; }

        private Product()
        {
        }

        public Product(long id, string name, string description, long conditionId,
            long typeId, decimal price, long currencyId, long sellerId,
            ICollection<HashTag> hashTags = null, ICollection<File> files = null)
        {
            Id = id;
            Name = name;
            Description = description;
            ConditionId = conditionId;
            TypeId = typeId;
            Price = price;
            CurrencyId = currencyId;
            SellerId = sellerId;
            HashTags = hashTags;
            Files = files;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"*{Name.Escape()}*");
            sb.AppendLine();
            sb.AppendLine($"💰 _{string.Join(' ', Price, Currency.Abbreviation).Escape()}_");
            sb.AppendLine();
            var hashTags = string.Join(" ", HashTags).Escape();
            sb.AppendLine(hashTags);
            sb.AppendLine();
            sb.AppendLine($"{Description.Escape()}");
            sb.AppendLine();
            sb.AppendLine($"Раздел: {Type.Name.Escape()}");
            sb.AppendLine($"Состояние: {Condition.Name.Escape()}");
            sb.AppendLine($"Продавец: [{Seller.FirstName} {Seller.LastName}](tg://user?id={Seller.TelegramId})");

            return sb.ToString();
        }
    }
}