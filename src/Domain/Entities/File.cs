using Domain.Common;

namespace Domain.Entities
{
    public class File : AuditableEntity
    {
        public long Id { get; }

        public string TelegramId { get; }

        public long ProductId { get; }

        public Product Product { get; }

        private File()
        {
        }

        public File(long id, string telegramId, long productId)
        {
            Id = id;
            TelegramId = telegramId;
            ProductId = productId;
        }
    }
}