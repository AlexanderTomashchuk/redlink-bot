using Domain.ValueObjects;

namespace Domain.Entities
{
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long TelegramId { get; set; }

        public UserStatus Status { get; set; }

        public Country Country { get; set; }
    }
}