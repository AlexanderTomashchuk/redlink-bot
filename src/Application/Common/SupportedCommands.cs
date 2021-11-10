using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace Application.Common
{
    //todo: is this shit?
    public class SupportedCommands : IEnumerable<BotCommand>
    {
        public IEnumerator<BotCommand> GetEnumerator()
        {
            yield return new BotCommand { Command = "/start", Description = "Getting started with the bot" };
            yield return new BotCommand { Command = "/sell", Description = "Create a new product" };
            yield return new BotCommand { Command = "/find", Description = "Find a product" };
            yield return new BotCommand { Command = "/setting", Description = "Change user settings" };
            yield return new BotCommand { Command = "/test", Description = "Test command" };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("; ", this.Select(c => c.Command));
        }
    }
}