using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Currency : AuditableEntity
    {
        public long Id { get; }

        public string Code { get; }

        public string Abbreviation { get; }

        public string Sign { get; }

        public ICollection<Country> Countries { get; }

        public ICollection<Product> Products { get; }

        private Currency()
        {
        }

        public Currency(long id, string code, string abbreviation, string sign, ICollection<Country> countries = null,
            ICollection<Product> products = null)
        {
            Id = id;
            Code = code;
            Abbreviation = abbreviation;
            Sign = sign;
            Countries = countries;
            Products = products;
        }
    }
}