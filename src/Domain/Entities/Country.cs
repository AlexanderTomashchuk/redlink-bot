using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Country : AuditableEntity
    {
        public long Id { get; }

        public string Name { get; }

        public string Code { get; }

        public string Flag { get; }

        public long DefaultCurrencyId { get; }

        public Currency DefaultCurrency { get; }

        public ICollection<AppUser> Users { get; }

        private Country()
        {
        }

        public Country(long id, string name, string code, string flag,
            long defaultCurrencyId, ICollection<AppUser> users = null)
        {
            Id = id;
            Name = name;
            Code = code;
            Flag = flag;
            DefaultCurrencyId = defaultCurrencyId;
            Users = users;
        }
    }
}