using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.BotCommands
{
    public class UsageCommand : BaseCommand
    {
        public UsageCommand(ITelegramBotClient botClient, IAppUserService appUserService)
            : base(botClient, appUserService)
        {
        }

        public override string Name => CommandNames.UsageCommand;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            message.Deconstruct(out var chatId);

            var botInfo = await BotClient.GetMeAsync();

            var usage = $"How to use {botInfo.FirstName} bot:\n" +
                        "/inline   - send inline keyboard\n" +
                        "/keyboard - send custom keyboard\n" +
                        "/sell     - Create new product\n" +
                        "/find     - Find a product\n" +
                        "/settings - Change user settings";

            await BotClient.SendTextMessageAsync(chatId, usage, replyMarkup: new ReplyKeyboardRemove());
        }
    }
}