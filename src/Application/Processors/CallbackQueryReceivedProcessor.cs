using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Processors
{
    public class CallbackQueryReceivedProcessor
    {
        private readonly ITelegramBotClient _botClient;

        public CallbackQueryReceivedProcessor(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task ProcessAsync(CallbackQuery updateCallbackQuery)
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