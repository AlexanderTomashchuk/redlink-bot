using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Application.Common.Extensions
{
    public static class CallbackQueryExtensions
    {
        public static CallbackQueryModel ToCallbackQueryModel(this CallbackQuery callbackQuery)
        {
            var callbackQueryModel = new CallbackQueryModel
            {
                From = callbackQuery.From,
                ChatId = callbackQuery.Message.Chat.Id,
                Data = JsonConvert.DeserializeObject<CallbackQueryModel.CallbackQueryData>(callbackQuery.Data)
            };

            return callbackQueryModel;
        }
    }
}