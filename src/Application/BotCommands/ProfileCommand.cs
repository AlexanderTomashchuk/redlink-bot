using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            var botInfo = await BotClient.GetMeAsync();

            var usage = $"How to use {botInfo.FirstName} bot:\n" +
                        "/inline   - send inline keyboard\n" +
                        "/keyboard - send custom keyboard\n" +
                        "/sell     - Create new product\n" +
                        "/find     - Find a product\n" +
                        "/settings - Change user settings";

            await BotClient.SendTextMessageAsync(ChatId, usage, replyMarkup: new ReplyKeyboardRemove());
        }
    }
}