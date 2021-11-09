using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.BotCommands
{
    public class TestCommand : BaseCommand
    {
        public TestCommand(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override string Name => CommandNames.TestCommand;

        public override async Task ExecuteAsync(Message message)
        {
            message.Deconstruct(out var chatId);

            var msg = await BotClient.SendTextMessageAsync(chatId, "Введите название для Вашего розыгрыша.",
                replyMarkup: new ForceReplyMarkup());

            // await BotClient.SendTextMessageAsync(chatId, "test", replyMarkup:
            //     new ReplyKeyboardMarkup {})

            foreach (var chatid in new[] { 222302217, 189560650, 2062445490 })
            {
                await BotClient.SendTextMessageAsync(chatid, $"Hi user from chatid = {chatid}");

                await Task.Delay(1000);
            }

            return;
            await BotClient.SendTextMessageAsync(chatId, "Please provide the product name:",
                replyMarkup: new ForceReplyMarkup());
        }
    }
}