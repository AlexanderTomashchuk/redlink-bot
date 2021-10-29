using System.Threading.Tasks;
using Bot.Pooling.Extensions;
using Bot.Pooling.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Pooling.Handlers.CommandHandlers
{
    public class SellCommandHandler : BaseCommandHandler
    {
        public SellCommandHandler(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task HandleAsync(Message message)
        {
            message.Deconstruct(out var chatId);

            var menu = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                        { Text = "Set Name", CallbackData = "SET_PRODUCT_NAME" },
                    new InlineKeyboardButton
                        { Text = "Set Description", CallbackData = "SET_PRODUCT_DESCRIPTION" }
                },
                new[]
                {
                    new InlineKeyboardButton
                        { Text = "Set Price", CallbackData = "SET_PRODUCT_PRICE" },
                    new InlineKeyboardButton
                        { Text = "Set Currency", CallbackData = "SET_PRODUCT_CURRENCY" }
                }
            });

            //BotClient.SendTextMessageAsync(cha)

            await BotClient.SendTextMessageAsync(chatId, "Let's create a product to sell", replyMarkup: menu);
        }
    }
}