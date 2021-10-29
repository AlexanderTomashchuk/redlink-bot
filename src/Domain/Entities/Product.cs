using System.Collections.Generic;
using System.Text;
using Domain.Common;

namespace Domain.Entities
{
    public class Product
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ProductCondition Condition { get; set; }

        public ProductType Type { get; set; }

        public decimal Price { get; set; }

        public Currency Currency { get; set; }

        public IEnumerable<string> HashTags { get; set; }

        public IEnumerable<string> FileIds { get; set; }

        public User Seller { get; set; }

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

    //todo: REMOVE
    public class ProductBuilder
    {
        private Product _product;

        public ProductBuilder()
        {
            _product = new Product();
        }


        public Product Build()
        {
            return _product;
        }
    }
}