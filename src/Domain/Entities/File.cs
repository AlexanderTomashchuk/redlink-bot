using Domain.Common;

namespace Domain.Entities;

public class File : AuditableEntity
{
    public long Id { get; }

    public string TelegramId { get; set; }

    public long ProductId { get; set; }

    public Product Product { get; set; }

    public File()
    {
    }

    public File(long id, string telegramId, long productId)
    {
        Id = id;
        TelegramId = telegramId;
        ProductId = productId;
    }
}