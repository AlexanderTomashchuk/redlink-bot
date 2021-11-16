using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands
{
    public class SellCommand : BaseCommand
    {
        public SellCommand(ITelegramBotClient botClient, IAppUserService appUserService)
            : base(botClient, appUserService)
        {
        }

        public override CommandType CommandType => CommandType.Sell;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            await BotClient.SendTextMessageAsync(ChatId, "NOT IMPLEMENTED", cancellationToken: cancellationToken);
        }
    }
}