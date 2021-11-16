using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands;

public class FindCommand : BaseCommand
{
    public FindCommand(ITelegramBotClient botClient, IAppUserService appUserService)
        : base(botClient, appUserService)
    {
    }

    public override CommandType CommandType => CommandType.Find;

    public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
    {
        await BotClient.SendTextMessageAsync(ChatId, "NOT IMPLEMENTED", cancellationToken: cancellationToken);
    }
}