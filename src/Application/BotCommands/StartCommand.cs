using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.BotRequests;
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
        private readonly AskCountryRequest _askCountryRequest;

        public StartCommand(
            ITelegramBotClient botClient,
            IAppUserService appUserService,
            AskCountryRequest askCountryRequest) : base(botClient, appUserService)
        {
            _askCountryRequest = askCountryRequest;
        }

        public override CommandType CommandType => CommandType.Start;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            await BotClient.SendTextMessageAsync(ChatId, GetWelcomeMessage(CurrentAppUser),
                ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            if (CurrentAppUser.CountryId == null)
            {
                await _askCountryRequest.ExecuteAsync(cancellationToken);
            }
        }

        private string GetWelcomeMessage(AppUser user)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"👋 Hello {user.GetTelegramMarkdownLink()}\\. I can help you sell or buy a variety of clothes\\.");
            sb.AppendLine("You can control me by sending these commands:");
            foreach (var commandType in CommandTypeEnumeration.GetAll())
            {
                sb.AppendLine($"{commandType.Name} - {commandType.Description}".Escape());
            }

            return sb.ToString();
        }
    }
}