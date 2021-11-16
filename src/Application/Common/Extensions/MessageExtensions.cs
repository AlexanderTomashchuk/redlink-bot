using System;
using System.Linq;
using Application.BotCommands;
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

        commandType = CommandTypeEnumeration.GetAll().FirstOrDefault(ct =>
            commandString.Equals(ct.Name, StringComparison.InvariantCultureIgnoreCase));

        return commandType is not null;
    }
}