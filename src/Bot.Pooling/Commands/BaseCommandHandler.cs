using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Pooling.Commands
{
    public abstract class BaseCommandHandler
    {
        protected readonly ITelegramBotClient BotClient;

        protected BaseCommandHandler(ITelegramBotClient botClient)
        {
            BotClient = botClient;
        }

        public abstract Task HandleAsync(Message message);
    }
}