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

        public AppUserStatus? Status { get; set; }

        public long? CountryId { get; set; }

        public Country Country { get; set; }

        public long? LanguageId { get; set; }

        public Language Language { get; }

        public ICollection<Product> Products { get; }

        private AppUser()
        {
        }

        public AppUser(long id, string firstName, string lastName, string username, long? chatId = null,
            AppUserStatus? status = null, long? countryId = null, long? languageId = null,
            ICollection<Product> products = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            ChatId = chatId;
            Status = status;
            CountryId = countryId;
            Products = products;
        }
    }
}