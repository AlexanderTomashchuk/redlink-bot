using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;

namespace Application.BotRequests;

public class AskCountryRequest
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly ICountryService _countryService;

    public AskCountryRequest(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService)
    {
        _botClient = botClient;
        _appUserService = appUserService;
        _countryService = countryService;
    }


    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        //TODO: IMPLEMENT AS A NEW WORKFLOW
        //TODO: THEN REMOVE THIS 
        // var chatId = _appUserService.Current.ChatId;
        //
        // var countries = await _countryService.GetAllAsync(cancellationToken);
        //
        // var initCountryMessage = BotMessage.GetInitCountryMessage();
        // var replyMarkup = BotInlineKeyboard.GetInitCountryKeyboard(countries);
        //
        // await _botClient.SendTextMessageAsync(chatId, initCountryMessage, ParseMode.MarkdownV2,
        //     replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
}