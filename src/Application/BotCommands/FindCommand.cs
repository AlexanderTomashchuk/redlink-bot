using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.BotCommands
{
    public class FindCommand : BaseCommand
    {
        public FindCommand(ITelegramBotClient botClient, IAppUserService appUserService)
            : base(botClient, appUserService)
        {
        }

        public override CommandType CommandType => CommandType.Find;
        
        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
            var msg = await BotClient.SendTextMessageAsync(ChatId, "Введите название для Вашего розыгрыша.",
                replyMarkup: new ForceReplyMarkup());

            // await BotClient.SendTextMessageAsync(chatId, "test", replyMarkup:
            //     new ReplyKeyboardMarkup {})

            foreach (var chatid in new[] { 222302217, 189560650, 2062445490 })
            {
                await BotClient.SendTextMessageAsync(chatid, $"Hi user from chatid = {chatid}");

                await Task.Delay(1000);
            }
        }
    }
}