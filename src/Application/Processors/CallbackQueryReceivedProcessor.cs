using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Workflows;
using Application.Workflows.Profile;
using AutoMapper;
using Telegram.Bot.Types;

namespace Application.Processors;

public class CallbackQueryReceivedProcessor
{
    private readonly IProfileWorkflow _profileWorkflow;
    private readonly IMapper _mapper;

    public CallbackQueryReceivedProcessor(IProfileWorkflow profileWorkflow, IMapper mapper)
        => (_profileWorkflow, _mapper) = (profileWorkflow, mapper);

    public async Task ProcessAsync(CallbackQuery updateCallbackQuery, CancellationToken cancellationToken = default)
    {
        var callbackQueryId = updateCallbackQuery.Id;
        var fromMessageId = updateCallbackQuery.Message.MessageId;
        var callbackQueryDto = _mapper.Map<CallbackQueryDto>(updateCallbackQuery.Data);

        switch (callbackQueryDto.WorkflowType)
        {
            case WorkflowType.Profile:
            {
                var (state, trigger, entityId) = callbackQueryDto.ProfileWorkflowDto;

                var profileWorkflowStateMachine = _profileWorkflow
                    .ForMessageId(fromMessageId)
                    .ForCallbackQueryId(callbackQueryId)
                    .ConfigureStateMachine(state);

                await profileWorkflowStateMachine.TriggerAsync(trigger, entityId, cancellationToken);

                return;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(callbackQueryDto.WorkflowType));
        }

        //todo: OT remove
        // switch (callbackQueryData.CommandName)
        // {
        //     case "INIT_COUNTRY":
        //     {
        //         //TODO: IMPLEMENT AS WORKFLOW
        //         await _appUserService.UpdateAsync(
        //             async (appUser, context) => appUser.Country =
        //                 await context.Countries.FirstAsync(c => c.Id == callbackQueryData.Id, cancellationToken),
        //             cancellationToken);
        //
        //         var selectedCountryMessage = BotMessage.GetSelectedCountryMessage(callbackQueryData.Text);
        //
        //         var sentTextMessageTask = _botClient.SendTextMessageAsync(chatId,
        //             selectedCountryMessage,
        //             ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        //
        //         var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
        //             selectedCountryMessage,
        //             cancellationToken: cancellationToken);
        //
        //         await Task.WhenAll(sentTextMessageTask, answerCbQueryTask);
        //
        //         break;
        //     }
        //     case "EDIT_COUNTRY_REQUEST":
        //     {
        //         var editCountryMessage = BotMessage.GetEditCountryMessage();
        //         var countries = await _countryService.GetAllAsync(cancellationToken);
        //         var editCountryKeyboard = BotInlineKeyboard.GetEditCountryKeyboard(countries);
        //     
        //         await _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId, editCountryMessage,
        //             ParseMode.MarkdownV2, replyMarkup: editCountryKeyboard, cancellationToken: cancellationToken);
        //     
        //         break;
        //     }
        //     case "EDIT_LANGUAGE_REQUEST":
        //     {
        //         var editLanguageMessage = BotMessage.GetEditLanguageMessage();
        //         var languages = await _languageService.GetAllAsync(cancellationToken);
        //         var editLanguageKeyboard = BotInlineKeyboard.GetEditLanguageKeyboard(languages);
        //     
        //         await _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
        //             editLanguageMessage,
        //             ParseMode.MarkdownV2, replyMarkup: editLanguageKeyboard, cancellationToken: cancellationToken);
        //     
        //         break;
        //     }
        //     case "EDIT_COUNTRY":
        //     {
        //         await _appUserService.UpdateAsync(
        //             async (appUser, context) => appUser.Country =
        //                 await context.Countries.FirstAsync(c => c.Id == callbackQueryData.Id, cancellationToken),
        //             cancellationToken);
        //     
        //         var profileInfoMessage = BotMessage.GetProfileInfoMessage(_appUserService.Current);
        //         var changeProfileKeyboard = BotInlineKeyboard.GetChangeProfileKeyboard();
        //     
        //         var editTextMessageTask = _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
        //             profileInfoMessage,
        //             ParseMode.MarkdownV2, replyMarkup: changeProfileKeyboard, cancellationToken: cancellationToken);
        //     
        //         var selectedCountryMessage = BotMessage.GetSelectedCountryMessage(callbackQueryData.Text);
        //         var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
        //             selectedCountryMessage,
        //             cancellationToken: cancellationToken);
        //     
        //         await Task.WhenAll(editTextMessageTask, answerCbQueryTask);
        //     
        //         break;
        //     }
        //     case "EDIT_LANGUAGE":
        //     {
        //         await _appUserService.UpdateAsync(
        //             async (appUser, context) => appUser.Language =
        //                 await context.Languages.FirstAsync(c => c.Code == (Language.LanguageCode)callbackQueryData.Id,
        //                     cancellationToken),
        //             cancellationToken);
        //     
        //         var profileInfoMessage = BotMessage.GetProfileInfoMessage(_appUserService.Current);
        //         var changeProfileKeyboard = BotInlineKeyboard.GetChangeProfileKeyboard();
        //     
        //         var editTextMessageTask = _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
        //             profileInfoMessage,
        //             ParseMode.MarkdownV2, replyMarkup: changeProfileKeyboard, cancellationToken: cancellationToken);
        //     
        //         var selectedLanguageMessage = BotMessage.GetSelectedLanguageMessage(callbackQueryData.Text);
        //         var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
        //             selectedLanguageMessage, cancellationToken: cancellationToken);
        //     
        //         await Task.WhenAll(editTextMessageTask, answerCbQueryTask);
        //     
        //         break;
        //     }
        //     case "BACK_TO_PROFILE":
        //     {
        //         var profileInfoMessage = BotMessage.GetProfileInfoMessage(_appUserService.Current);
        //         var changeProfileKeyboard = BotInlineKeyboard.GetChangeProfileKeyboard();
        //     
        //         await _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
        //             profileInfoMessage,
        //             ParseMode.MarkdownV2, replyMarkup: changeProfileKeyboard, cancellationToken: cancellationToken);
        //     
        //         break;
        //     }
        // }
    }
}