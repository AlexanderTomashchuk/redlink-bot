using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Emoji = Application.Common.Emoji;
using l10n = Application.Resources.Localization;

namespace Application.Workflows.Profile;

public class EditProfileWorkflow : StateMachineWorkflow<EditProfileWorkflow.State, EditProfileWorkflow.Trigger>,
    StateStorageMode.ITransitional<EditProfileWorkflow.State>, ICommandWorkflow
{
    private readonly ICountryService _countryService;
    private readonly ILanguageService _languageService;

    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _showProfileInfoTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectLanguageTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _updateCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _updateLanguageTrigger;

    public EditProfileWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService, ILanguageService languageService, IMapper mapper,
        ILogger<EditProfileWorkflow> logger, WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, logger, workflowFactory) =>
        (_countryService, _languageService) = (countryService, languageService);

    protected override WorkflowType WorkflowType => WorkflowType.EditProfile;

    public State CurrentState => GetCbData<EditProfileCqDto>()?.State ?? State.Initial;

    protected override void ConfigureStateMachine()
    {
        Machine = new StateMachine<State, Trigger>(CurrentState);

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

    protected override async Task TriggerNextAsync(Trigger? triggerToInvoke = null,
        CancellationToken cancellationToken = default)
    {
        var trigger = triggerToInvoke ?? GetNextTriggerToInvoke();
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
        var text = new MessageTextBuilder(ParseMode.MarkdownV2)
            .AddTextLine($"{l10n.YourCurrProfileSettings}:")
            .BreakLine()
            .AddTextLine(
                $"{$"{Emoji.COUNTRY} {l10n.Country}:",-13} {l10n.ResourceManager.GetString(CurrentAppUser.Country.NameLocalizationKey)}",
                TextStyle.Code)
            .AddTextLine(
                $"{$"{Emoji.LANGUAGE} {l10n.Language}:",-13} {l10n.ResourceManager.GetString(CurrentAppUser.Language.NameLocalizationKey)}",
                TextStyle.Code)
            .BreakLine()
            .AddTextLine($"{l10n.UseBtnsToChangeProfile}.", TextStyle.Italic)
            .Build();
 
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButton(l10n.ChangeCountry, new EditProfileCqDto(State.ProfileInfoShowing, Trigger.SelectCountry))
            .AddButton(l10n.ChangeLanguage, new EditProfileCqDto(State.ProfileInfoShowing, Trigger.SelectLanguage))
            .ChunkBy(2)
            .Build();

        if (MessageIdBelongsToCb is not null)
        {
            await BotClient.EditFormattedMessageTxtAsync(ChatId, MessageIdBelongsToCb.Value, text, 
                replyMarkup, cancellationToken);
        }
        else
        {
            await BotClient.SendFormattedTxtMessageAsync(ChatId, text, replyMarkup, cancellationToken);
        }
    }

    private async Task ShowCountriesAsync(CancellationToken cancellationToken)
    {
        var countries = await _countryService.GetAllAsync(cancellationToken);

        var replyMarkup = new InlineKeyboardBuilder()
            .AddButtons(countries.Select(c =>
            {
                var text = $"{c.Flag} {l10n.ResourceManager.GetString(c.NameLocalizationKey)}";
                var cbData = new EditProfileCqDto(State.CountrySelection, Trigger.UpdateCountry, c.Id);

                return (text, cbData);
            }))
            .WithBackButton(new EditProfileCqDto(State.CountrySelection, Trigger.ShowProfileInfo))
            .ChunkBy(2)
            .Build();

        Debug.Assert(MessageIdBelongsToCb != null, nameof(MessageIdBelongsToCb) + " != null");
        await BotClient.EditMessageTxtAsync(ChatId, MessageIdBelongsToCb.Value, l10n.ChooseCountryFromList,
            replyMarkup, cancellationToken);
    }

    private async Task ShowLanguagesAsync(CancellationToken cancellationToken)
    {
        var languages = await _languageService.GetAllAsync(cancellationToken);

        var replyMarkup = new InlineKeyboardBuilder()
            .AddButtons(languages.Select(l =>
            {
                var text = l10n.ResourceManager.GetString(l.NameLocalizationKey);
                var cbData = new EditProfileCqDto(State.LanguageSelection, Trigger.UpdateLanguage, (long)l.Code);

                return (text, cbData);
            }))
            .WithBackButton(new EditProfileCqDto(State.LanguageSelection, Trigger.ShowProfileInfo))
            .ChunkBy(2)
            .Build();

        Debug.Assert(MessageIdBelongsToCb != null, nameof(MessageIdBelongsToCb) + " != null");
        await BotClient.EditMessageTxtAsync(ChatId, MessageIdBelongsToCb.Value, l10n.ChooseLanguageFromList,
            replyMarkup, cancellationToken);
    }

    private async Task UpdateAppUserCountryAsync(long? entityId, CancellationToken cancellationToken)
    {
        var newCountry = await _countryService.FirstAsync(c => c.Id == entityId, cancellationToken);

        await AppUserService.UpdateAsync(au => au.Country = newCountry, cancellationToken);

        var text = $"{l10n.SelectedCountry}: {l10n.ResourceManager.GetString(newCountry.NameLocalizationKey)}";
        _ = BotClient.AnswerCbQueryAsync(CallbackQueryId, text, cancellationToken);

        await TriggerNextAsync(Trigger.ShowProfileInfo, cancellationToken);
    }

    private async Task UpdateAppUserLanguageAsync(long? entityId, CancellationToken cancellationToken)
    {
        var newLanguage =
            await _languageService.FirstOrDefaultAsync(l => l.Code == (Language.LanguageCode)entityId,
                cancellationToken);

        await AppUserService.UpdateAsync(au => au.Language = newLanguage, cancellationToken);

        var text = $"{l10n.SelectedLanguage}: {l10n.ResourceManager.GetString(newLanguage.NameLocalizationKey)}";
        _ = BotClient.AnswerCbQueryAsync(CallbackQueryId, text, cancellationToken);

        await TriggerNextAsync(Trigger.ShowProfileInfo, cancellationToken);
    }

    private Trigger GetNextTriggerToInvoke() =>
        GetCbData<EditProfileCqDto>()?.Trigger ?? Trigger.ShowProfileInfo;

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

    public CommandType CommandType => CommandType.Profile;
}