using System.Linq;
using Telegram.Bot.Types;

namespace Application.Common.Extensions
{
    public static class MessageExtensions
    {
        public static bool IsCommand(this Message message)
        {
            var allowedCommands = new SupportedCommands();
            return allowedCommands.Any(ac => ac.Command == message.ExtractCommandFromText());
        }

        public static string ExtractCommandFromText(this Message message)
        {
            return message.Text?.Trim().Split(' ').First();
        }

        public static void Deconstruct(this Message m, out long chatId)
        {
            chatId = m.Chat.Id;
        }

        public static void Deconstruct(this Message m, out long chatId, out int messageId)
        {
            chatId = m.Chat.Id;
            messageId = m.MessageId;
        }

        public static void Deconstruct(this Message m, out long chatId, out User from)
        {
            chatId = m.Chat.Id;
            from = m.From;
        }

        public static void Deconstruct(this Message m, out long chatId, out int messageId, out User from)
        {
            chatId = m.Chat.Id;
            messageId = m.MessageId;
            from = m.From;
        }
    }
}