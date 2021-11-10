using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Processors
{
    public class CallbackQueryReceivedProcessor
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAppUserService _appUserService;

        public CallbackQueryReceivedProcessor(ITelegramBotClient botClient, IAppUserService appUserService)
        {
            _botClient = botClient;
            _appUserService = appUserService;
        }

        public async Task ProcessAsync(CallbackQuery updateCallbackQuery, CancellationToken cancellationToken = default)
        {
            var callbackQueryModel = updateCallbackQuery.ToCallbackQueryModel();
            var (chatId, commandName, id, text, from) = callbackQueryModel;

            switch (commandName)
            {
                case "SET_COUNTRY":
                    //todo: OT TO COMMAND???
                    var appUser = await _appUserService.SetCountryAsync(id, from, cancellationToken);

                    await _botClient.SendTextMessageAsync(chatId, $"Selected country: _{text}_",
                        ParseMode.MarkdownV2, cancellationToken: cancellationToken);

                    break;
                case "SET_PRODUCT_NAME":
                    var messageId = updateCallbackQuery.Message.MessageId;

                    await _botClient.EditMessageTextAsync(chatId, messageId, "CHANGED TEXT",
                        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                            { Text = "test button", CallbackData = "SET_PRODUCT_DESCRIPTION" }),
                        cancellationToken: cancellationToken);

                    break;
            }
        }
    }
}