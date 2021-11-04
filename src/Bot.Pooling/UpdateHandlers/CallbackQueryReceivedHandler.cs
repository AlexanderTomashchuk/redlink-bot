using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Pooling.UpdateHandlers
{
    public class CallbackQueryReceivedHandler
    {
        private readonly ITelegramBotClient _botClient;

        public CallbackQueryReceivedHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleAsync(CallbackQuery updateCallbackQuery)
        {
            switch (updateCallbackQuery.Data)
            {
                case "SET_PRODUCT_NAME":
                    var chatId = updateCallbackQuery.Message.Chat.Id;
                    var messageId = updateCallbackQuery.Message.MessageId;


                    await _botClient.EditMessageTextAsync(chatId, messageId, "CHANGED TEXT",
                        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                            { Text = "test button", CallbackData = "SET_PRODUCT_DESCRIPTION" }));

                    //await bot.AnswerCallbackQueryAsync(updateCallbackQuery.Id, "DEDEDE");
                    //updateCallbackQuery.Message.Text = "DEDEDE";

                    break;
                case "SET_PRODUCT_DESCRIPTION":

                    break;
            }
        }
    }
}