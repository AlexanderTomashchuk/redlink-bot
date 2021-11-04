using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Language : AuditableEntity
    {
        public long Id { get; }

        public string Name { get; }

        public string Code { get; }

        public string Flag { get; }

        public ICollection<User> Users { get; }

        private Language()
        {
        }

        public Language(long id, string name, string code, string flag,
            ICollection<User> users = null)
        {
            Id = id;
            Name = name;
            Code = code;
            Flag = flag;
            Users = users;
        }
    }
}