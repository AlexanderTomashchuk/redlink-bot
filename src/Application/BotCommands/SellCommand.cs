using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.BotCommands
{
    public class SellCommand : BaseCommand
    {
        public SellCommand(ITelegramBotClient botClient, IAppUserService appUserService)
            : base(botClient, appUserService)
        {
        }

        public override CommandType CommandType => CommandType.Sell;

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
        {
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

            await BotClient.SendTextMessageAsync(ChatId, "Let's create a product to sell", replyMarkup: menu);
        }
    }
}