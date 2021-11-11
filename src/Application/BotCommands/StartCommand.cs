using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.BotCommands
{
    public class StartCommand : BaseCommand
    {
        private readonly IAppUserService _appUserService;
        private readonly ICountryService _countryService;

        public StartCommand(
            ITelegramBotClient botClient,
            IAppUserService appUserService,
            ICountryService countryService) : base(botClient)
        {
            _appUserService = appUserService;
            _countryService = countryService;
        }

        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            message.Deconstruct(out var chatId);

            await BotClient.SendTextMessageAsync(chatId, GetWelcomeMessage(_appUserService.Current), ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            if (_appUserService.Current.CountryId == null)
            {
                var countries = await _countryService.GetAllAsync(cancellationToken);
                var replyMarkup = countries.ToInlineKeyboardMarkup();

                await BotClient.SendTextMessageAsync(chatId, GetSetCountryMessage(), ParseMode.MarkdownV2,
                    replyMarkup: replyMarkup, cancellationToken: cancellationToken);
            }
        }

        private string GetWelcomeMessage(AppUser user)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"👋 Hello {user.GetTelegramMarkdownLink()}\\. I can help you sell or buy a variety of clothes\\.");
            sb.AppendLine("You can control me by sending these commands:");
            foreach (var botCommand in new SupportedCommands())
            {
                sb.AppendLine($"{botCommand.Command} - {botCommand.Description}".Escape());
            }

            return sb.ToString();
        }

        private string GetSetCountryMessage()
        {
            var sb = new StringBuilder();

            //Прежде чем начать, пожалуйста выберите страну проживания из списка ниже. Мы нуждаемся в данной информации, чтобы предоставить вам максимально релевантные данные о товарах, которые мы имеем.
            sb.AppendLine("Before you start working with bot, please select your country from the list below\\.");
            sb.AppendLine(
                "_We need this information to provide you with the most relevant data about the products that we have\\._");

            return sb.ToString();
        }
    }
}