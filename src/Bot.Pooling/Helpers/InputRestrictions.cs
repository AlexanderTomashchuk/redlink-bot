using System.Linq;
using Telegram.Bot.Types.Enums;

namespace Bot.Pooling.Helpers
{
    public static class InputRestrictions
    {
        public static UpdateType[] AllowedUpdates => new[]
        {
            UpdateType.Message, UpdateType.EditedMessage, UpdateType.CallbackQuery, UpdateType.MyChatMember,
            UpdateType.Unknown
        };

        private static MessageType[] AllowedMessageTypes => new[]
        {
            MessageType.Text, MessageType.Photo, MessageType.Contact
        };

        public static bool IsAllowedMessageType(this MessageType messageType)
        {
            return AllowedMessageTypes.Any(amt => amt == messageType);
        }
    }
}