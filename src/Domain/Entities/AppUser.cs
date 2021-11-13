using System.Collections.Generic;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class AppUser : AuditableEntity
    {
        /// <summary>
        /// Equals to Telegram.User.Id
        /// </summary>
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public long? ChatId { get; set; }

        public AppUserStatus? Status { get; set; } = AppUserStatus.Member;

        public long? CountryId { get; set; }
        
        public Country Country { get; set; }
        
        public bool HasCountry => CountryId != null;
        
        public Language.LanguageCode? LanguageCode { get; set; }

        public Language Language { get; }

        public ICollection<Product> Products { get; }

        private AppUser()
        {
        }
    }
}