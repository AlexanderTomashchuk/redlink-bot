using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.BotRequests
{
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
            var chatId = _appUserService.Current.ChatId;

            var countries = await _countryService.GetAllAsync(cancellationToken);
            var replyMarkup = countries.ToInlineKeyboardMarkup();

            await _botClient.SendTextMessageAsync(chatId, GetSetCountryMessage(), ParseMode.MarkdownV2,
                replyMarkup: replyMarkup, cancellationToken: cancellationToken);
        }

        private string GetSetCountryMessage()
        {
            var sb = new StringBuilder();

            //Прежде чем начать, пожалуйста выберите страну проживания из списка ниже. Мы нуждаемся в данной информации, чтобы предоставить вам максимально релевантные данные о товарах, которые мы имеем.
            sb.AppendLine("Please select your country from the list below\\.");
            sb.AppendLine(
                "_We need this information to provide you with the most relevant data about the products that we have\\._");

            return sb.ToString();
        }
    }
}