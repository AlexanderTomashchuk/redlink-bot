using System.Threading.Tasks;
using Bot.WebHook.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.WebHook.Commands
{
    public class UsageCommandHandler : BaseCommandHandler
    {
        public UsageCommandHandler(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task HandleAsync(Message message)
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