using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands
{
    public abstract class BaseCommand
    {
        protected readonly ITelegramBotClient BotClient;

        protected BaseCommand(ITelegramBotClient botClient)
        {
            BotClient = botClient;
        }

        public abstract string Name { get; }

        public abstract Task ExecuteAsync(Message message, CancellationToken cancellationToken = default);
    }
}