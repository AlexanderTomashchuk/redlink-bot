using System.Collections;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Bot.Pooling.Helpers
{
    //todo: is this shit?
    public class SupportedCommands : IEnumerable<BotCommand>
    {
        public IEnumerator<BotCommand> GetEnumerator()
        {
            yield return new BotCommand { Command = "/sell", Description = "Create new product" };
            yield return new BotCommand { Command = "/find", Description = "Find a product" };
            yield return new BotCommand { Command = "/setting", Description = "Change user settings" };
            yield return new BotCommand { Command = "/test", Description = "Test command" };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}