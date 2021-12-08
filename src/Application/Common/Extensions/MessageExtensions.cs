using System.Linq;
using Telegram.Bot.Types;

namespace Application.Common.Extensions;

public static class MessageExtensions
{
    public static bool TryParseCommandType(this Message message, out CommandType commandType)
    {
        var commandString = message?.Text.Trim().Split(' ').FirstOrDefault();

        if (commandString is null)
        {
            commandType = null;
            return false;
        }

        return CommandType.TryFromName(commandString, out commandType);
    }
}