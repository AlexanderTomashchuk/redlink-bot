using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.BotRequests;
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
        private readonly AskCountryRequest _askCountryRequest;

        public StartCommand(
            ITelegramBotClient botClient,
            IAppUserService appUserService,
            AskCountryRequest askCountryRequest) : base(botClient, appUserService)
        {
            _askCountryRequest = askCountryRequest;
        }

        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            message.Deconstruct(out var chatId);

            await BotClient.SendTextMessageAsync(chatId, GetWelcomeMessage(CurrentAppUser),
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
            foreach (var botCommand in new SupportedCommands())
            {
                sb.AppendLine($"{botCommand.Command} - {botCommand.Description}".Escape());
            }

            return sb.ToString();
        }
    }
}