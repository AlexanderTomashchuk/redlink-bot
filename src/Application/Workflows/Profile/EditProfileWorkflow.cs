using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.Profile;

public class EditProfileWorkflow : StateMachineWorkflow<EditProfileWorkflow.State, EditProfileWorkflow.Trigger>
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly ICountryService _countryService;
    private readonly ILanguageService _languageService;

    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _showProfileInfoTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectLanguageTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _updateCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _updateLanguageTrigger;

    public EditProfileWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService, ILanguageService languageService, IMapper mapper,
        ILogger<EditProfileWorkflow> logger) : base(mapper, logger) =>
        (_botClient, _appUserService, _countryService, _languageService) =
        (botClient, appUserService, countryService, languageService);

    public override WorkflowType Type => WorkflowType.EditProfile;

    protected override State InitialState => GetCbData<EditProfileCqDto>()?.State ?? State.Initial;

    protected override Trigger GetTriggerToInvoke() =>
        GetCbData<EditProfileCqDto>()?.Trigger ?? Trigger.ShowProfileInfo;

    protected override void ConfigureStateMachine()
    {
        Machine = new StateMachine<State, Trigger>(InitialState);

        _showProfileInfoTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken>(Trigger.ShowProfileInfo);

        _selectCountryTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken>(Trigger.SelectCountry);

        _selectLanguageTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken>(Trigger.SelectLanguage);

        _updateCountryTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken>(Trigger.UpdateCountry);

        _updateLanguageTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken>(Trigger.UpdateLanguage);

        Machine.Configure(State.Initial)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        Machine.Configure(State.ProfileInfoShowing)
            .OnEntryFromAsync(_showProfileInfoTrigger, ShowProfileInfoAsync)
            .Permit(Trigger.SelectCountry, State.CountrySelection)
            .Permit(Trigger.SelectLanguage, State.LanguageSelection);

        Machine.Configure(State.CountrySelection)
            .OnEntryFromAsync(_selectCountryTrigger, ShowCountriesAsync)
            .Permit(Trigger.UpdateCountry, State.CountryUpdated)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        Machine.Configure(State.LanguageSelection)
            .OnEntryFromAsync(_selectLanguageTrigger, ShowLanguagesAsync)
            .Permit(Trigger.UpdateLanguage, State.LanguageUpdated)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        Machine.Configure(State.CountryUpdated)
            .OnEntryFromAsync(_updateCountryTrigger, UpdateAppUserCountryAsync)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        Machine.Configure(State.LanguageUpdated)
            .OnEntryFromAsync(_updateLanguageTrigger, UpdateAppUserLanguageAsync)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);
    }

    protected override async Task TriggerAsync(Trigger? triggerToInvoke = null,
        CancellationToken cancellationToken = default)
    {
        var trigger = triggerToInvoke ?? GetTriggerToInvoke();
        var entityId = GetEntityId<EditProfileCqDto>();
        var handler = trigger switch
        {
            Trigger.ShowProfileInfo => Machine.FireAsync(_showProfileInfoTrigger, cancellationToken),
            Trigger.SelectCountry => Machine.FireAsync(_selectCountryTrigger, cancellationToken),
            Trigger.SelectLanguage => Machine.FireAsync(_selectLanguageTrigger, cancellationToken),
            Trigger.UpdateCountry => Machine.FireAsync(_updateCountryTrigger, entityId, cancellationToken),
            Trigger.UpdateLanguage => Machine.FireAsync(_updateLanguageTrigger, entityId, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(trigger), trigger,
                "There is an unsupported trigger type provided")
        };

        await handler;
    }

    private async Task ShowProfileInfoAsync(CancellationToken cancellationToken)
    {
        var message = BotMessage.GetProfileInfoMessage(_appUserService.Current);
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButton("Change country", new EditProfileCqDto(State.ProfileInfoShowing, Trigger.SelectCountry))
            .AddButton("Change language", new EditProfileCqDto(State.ProfileInfoShowing, Trigger.SelectLanguage))
            .ChunkBy(2)
            .Build();

        if (MessageIdBelongsToCb is not null)
        {
            await _botClient.EditMessageTextAsync(ChatId, MessageIdBelongsToCb.Value, message, ParseMode.MarkdownV2,
                replyMarkup,
                cancellationToken);
        }
        else
        {
            await _botClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2, replyMarkup,
                cancellationToken);
        }
    }

    private async Task ShowCountriesAsync(CancellationToken cancellationToken)
    {
        var countries = await _countryService.GetAllAsync(cancellationToken);

        var message = BotMessage.GetEditCountryMessage();
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButtons(countries.Select(c =>
            {
                var text = $"{c.Flag} {c.Name}";
                var cbData = new EditProfileCqDto(State.CountrySelection, Trigger.UpdateCountry, c.Id);

                return (text, cbData);
            }))
            .WithBackButton(new EditProfileCqDto(State.CountrySelection, Trigger.ShowProfileInfo))
            .ChunkBy(2)
            .Build();

        Debug.Assert(MessageIdBelongsToCb != null, nameof(MessageIdBelongsToCb) + " != null");
        await _botClient.EditMessageTextAsync(ChatId, MessageIdBelongsToCb.Value, message, ParseMode.MarkdownV2,
            replyMarkup,
            cancellationToken);
    }

    private async Task ShowLanguagesAsync(CancellationToken cancellationToken)
    {
        var languages = await _languageService.GetAllAsync(cancellationToken);

        var message = BotMessage.GetEditLanguageMessage();
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButtons(languages.Select(l =>
            {
                var text = l.Name;
                var cbData = new EditProfileCqDto(State.LanguageSelection, Trigger.UpdateLanguage, (long)l.Code);

                return (text, cbData);
            }))
            .WithBackButton(new EditProfileCqDto(State.LanguageSelection, Trigger.ShowProfileInfo))
            .ChunkBy(2)
            .Build();

        Debug.Assert(MessageIdBelongsToCb != null, nameof(MessageIdBelongsToCb) + " != null");
        await _botClient.EditMessageTextAsync(ChatId, MessageIdBelongsToCb.Value, message, ParseMode.MarkdownV2,
            replyMarkup,
            cancellationToken);
    }

    private async Task UpdateAppUserCountryAsync(long? entityId, CancellationToken cancellationToken)
    {
        var newCountry = await _countryService.FirstAsync(c => c.Id == entityId, cancellationToken);

        await _appUserService.UpdateAsync(au => au.Country = newCountry, cancellationToken);

        var message = BotMessage.GetSelectedCountryMessage(newCountry.Name);
        _ = _botClient.AnswerCallbackQueryAsync(CallbackQueryId, message, cancellationToken);

        await TriggerAsync(Trigger.ShowProfileInfo, cancellationToken);
    }

    private async Task UpdateAppUserLanguageAsync(long? entityId, CancellationToken cancellationToken)
    {
        var newLanguage =
            await _languageService.FirstOrDefaultAsync(l => l.Code == (Language.LanguageCode)entityId,
                cancellationToken);

        await _appUserService.UpdateAsync(au => au.Language = newLanguage, cancellationToken);

        var message = BotMessage.GetSelectedLanguageMessage(newLanguage.Name);
        _ = _botClient.AnswerCallbackQueryAsync(CallbackQueryId, message, cancellationToken: cancellationToken);

        await TriggerAsync(Trigger.ShowProfileInfo, cancellationToken);
    }

    private long? ChatId => _appUserService.Current.ChatId;

    public enum Trigger
    {
        ShowProfileInfo,
        SelectCountry,
        SelectLanguage,
        UpdateCountry,
        UpdateLanguage
    }

    public enum State
    {
        Initial,
        ProfileInfoShowing,
        CountrySelection,
        LanguageSelection,
        CountryUpdated,
        LanguageUpdated
    }
}