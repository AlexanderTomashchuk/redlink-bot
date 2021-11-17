using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Processors;

public class CallbackQueryReceivedProcessor
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly ICountryService _countryService;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;

    public CallbackQueryReceivedProcessor(ITelegramBotClient botClient,
        IAppUserService appUserService, ICountryService countryService, ILanguageService languageService,
        IMapper mapper)
    {
        _botClient = botClient;
        _appUserService = appUserService;
        _countryService = countryService;
        _languageService = languageService;
        _mapper = mapper;
    }

    public async Task ProcessAsync(CallbackQuery updateCallbackQuery, CancellationToken cancellationToken = default)
    {
        var chatId = _appUserService.Current.ChatId;
        var callbackQueryData = _mapper.Map<CallbackQueryDataModel>(updateCallbackQuery.Data);

        switch (callbackQueryData.CommandName)
        {
            case "INIT_COUNTRY":
            {
                //todo: OT TO COMMAND???
                await _appUserService.UpdateAsync(
                    async (appUser, context) => appUser.Country =
                        await context.Countries.FirstAsync(c => c.Id == callbackQueryData.Id, cancellationToken),
                    cancellationToken);

                var selectedCountryMessage = BotMessage.GetSelectedCountryMessage(callbackQueryData.Text);

                var sentTextMessageTask = _botClient.SendTextMessageAsync(chatId,
                    selectedCountryMessage,
                    ParseMode.MarkdownV2, cancellationToken: cancellationToken);

                var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                    selectedCountryMessage,
                    cancellationToken: cancellationToken);

                await Task.WhenAll(sentTextMessageTask, answerCbQueryTask);

                break;
            }
            case "EDIT_COUNTRY_REQUEST":
            {
                var editCountryMessage = BotMessage.GetEditCountryMessage();
                var countries = await _countryService.GetAllAsync(cancellationToken);
                var editCountryKeyboard = BotInlineKeyboard.GetEditCountryKeyboard(countries);

                await _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId, editCountryMessage,
                    ParseMode.MarkdownV2, replyMarkup: editCountryKeyboard, cancellationToken: cancellationToken);

                break;
            }
            case "EDIT_LANGUAGE_REQUEST":
            {
                var editLanguageMessage = BotMessage.GetEditLanguageMessage();
                var languages = await _languageService.GetAllAsync(cancellationToken);
                var editLanguageKeyboard = BotInlineKeyboard.GetEditLanguageKeyboard(languages);

                await _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
                    editLanguageMessage,
                    ParseMode.MarkdownV2, replyMarkup: editLanguageKeyboard, cancellationToken: cancellationToken);

                break;
            }
            case "EDIT_COUNTRY":
            {
                await _appUserService.UpdateAsync(
                    async (appUser, context) => appUser.Country =
                        await context.Countries.FirstAsync(c => c.Id == callbackQueryData.Id, cancellationToken),
                    cancellationToken);

                var profileInfoMessage = BotMessage.GetProfileInfoMessage(_appUserService.Current);
                var changeProfileKeyboard = BotInlineKeyboard.GetChangeProfileKeyboard();

                var editTextMessageTask = _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
                    profileInfoMessage,
                    ParseMode.MarkdownV2, replyMarkup: changeProfileKeyboard, cancellationToken: cancellationToken);

                var selectedCountryMessage = BotMessage.GetSelectedCountryMessage(callbackQueryData.Text);
                var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                    selectedCountryMessage,
                    cancellationToken: cancellationToken);

                await Task.WhenAll(editTextMessageTask, answerCbQueryTask);

                break;
            }
            case "EDIT_LANGUAGE":
            {
                await _appUserService.UpdateAsync(
                    async (appUser, context) => appUser.Language =
                        await context.Languages.FirstAsync(c => c.Code == (Language.LanguageCode)callbackQueryData.Id,
                            cancellationToken),
                    cancellationToken);

                var profileInfoMessage = BotMessage.GetProfileInfoMessage(_appUserService.Current);
                var changeProfileKeyboard = BotInlineKeyboard.GetChangeProfileKeyboard();

                var editTextMessageTask = _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
                    profileInfoMessage,
                    ParseMode.MarkdownV2, replyMarkup: changeProfileKeyboard, cancellationToken: cancellationToken);

                var selectedLanguageMessage = BotMessage.GetSelectedLanguageMessage(callbackQueryData.Text);
                var answerCbQueryTask = _botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                    selectedLanguageMessage, cancellationToken: cancellationToken);

                await Task.WhenAll(editTextMessageTask, answerCbQueryTask);

                break;
            }
            case "BACK_TO_PROFILE":
            {
                var profileInfoMessage = BotMessage.GetProfileInfoMessage(_appUserService.Current);
                var changeProfileKeyboard = BotInlineKeyboard.GetChangeProfileKeyboard();

                await _botClient.EditMessageTextAsync(chatId, updateCallbackQuery.Message.MessageId,
                    profileInfoMessage,
                    ParseMode.MarkdownV2, replyMarkup: changeProfileKeyboard, cancellationToken: cancellationToken);

                break;
            }
        }
    }
}