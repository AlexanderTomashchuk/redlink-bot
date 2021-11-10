using Telegram.Bot.Types;

namespace Application.Common
{
    public class CallbackQueryModel
    {
        public User From { get; set; }

        public long ChatId { get; set; }

        public CallbackQueryData Data { get; set; } = new();

        public class CallbackQueryData
        {
            public string CommandName { get; set; }

            public long Id { get; set; }

            public string Text { get; set; }
        }
    }

    public static class CallbackQueryModelExtensions
    {
        public static void Deconstruct(this CallbackQueryModel cqm, out long chatId, out string commandName,
            out long id, out string text, out User from)
        {
            chatId = cqm.ChatId;
            commandName = cqm.Data.CommandName;
            id = cqm.Data.Id;
            text = cqm.Data.Text;
            from = cqm.From;
        }
    }
}