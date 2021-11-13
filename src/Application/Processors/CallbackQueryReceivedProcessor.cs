using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Services.Interfaces;
using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Processors
{
    public class CallbackQueryReceivedProcessor
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;

        public CallbackQueryReceivedProcessor(ITelegramBotClient botClient,
            IAppUserService appUserService, IMapper mapper)
        {
            _botClient = botClient;
            _appUserService = appUserService;
            _mapper = mapper;
        }

        public async Task ProcessAsync(CallbackQuery updateCallbackQuery, CancellationToken cancellationToken = default)
        {
            var chatId = _appUserService.Current.ChatId;
            var callbackQueryData = _mapper.Map<CallbackQueryDataModel>(updateCallbackQuery.Data);

            switch (callbackQueryData.CommandName)
            {
                case "SET_COUNTRY":
                    //todo: OT TO COMMAND???
                    await _appUserService.UpdateAsync(appUser => appUser.CountryId = callbackQueryData.Id,
                        cancellationToken);

                    var sentTextMessageTask = _botClient.SendTextMessageAsync(chatId,
                        $"Selected country: _{callbackQueryData.Text}_",
                        ParseMode.MarkdownV2, cancellationToken: cancellationToken);

                    var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                        $"Selected country: {callbackQueryData.Text}",
                        cancellationToken: cancellationToken);

                    await Task.WhenAll(sentTextMessageTask, answerCbQueryTask);

                    break;
            }
        }
    }
}