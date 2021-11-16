using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.BotCommands
{
    public class ProfileCommand : BaseCommand
    {
        public ProfileCommand(ITelegramBotClient botClient, IAppUserService appUserService)
            : base(botClient, appUserService)
        {
        }

        public override CommandType CommandType => CommandType.Profile;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            var profileInfoMessage = BotMessage.GetProfileInfoMessage(CurrentAppUser);
            var replyMarkup = BotInlineKeyboard.GetChangeProfileKeyboard();

            await BotClient.SendTextMessageAsync(ChatId, profileInfoMessage, ParseMode.MarkdownV2,
                replyMarkup: replyMarkup, cancellationToken: cancellationToken);
        }
    }
}