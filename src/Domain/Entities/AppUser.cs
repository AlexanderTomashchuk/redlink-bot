using System.Collections.Generic;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class AppUser : AuditableEntity
    {
        public long Id { get; }

        public long TelegramId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Username { get; }

        public long ChatId { get; }

        public UserStatus Status { get; }

        public long CountryId { get; }

        public Country Country { get; }

        public long LanguageId { get; set; }

        public Language Language { get; }

        public ICollection<Product> Products { get; }

        private AppUser()
        {
        }

        public AppUser(long id, string firstName, string lastName, long telegramId, string username, long chatId,
            UserStatus status, long countryId, ICollection<Product> products = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            TelegramId = telegramId;
            Username = username;
            ChatId = chatId;
            Status = status;
            CountryId = countryId;
            Products = products;
        }
    }
}