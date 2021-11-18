using System.Threading;
using System.Threading.Tasks;
using Application.BotRequests;
using Application.Common;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.BotCommands;

public class StartCommand : BaseCommand
{
    private readonly AskCountryRequest _askCountryRequest;

    public StartCommand(ITelegramBotClient botClient, IAppUserService appUserService,
        AskCountryRequest askCountryRequest) : base(botClient, appUserService) =>
        _askCountryRequest = askCountryRequest;

    public override CommandType CommandType => CommandType.Start;

    public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
    {
        var welcomeMessage = BotMessage.GetWelcomeMessage(CurrentAppUser);

        await BotClient.SendTextMessageAsync(ChatId, welcomeMessage, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

        if (!CurrentAppUser.HasCountry)
        {
            await _askCountryRequest.ExecuteAsync(cancellationToken);
        }
    }
}