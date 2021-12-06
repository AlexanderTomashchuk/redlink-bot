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
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.Profile;

public class EditProfileWorkflow : Workflow
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly ICountryService _countryService;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;
    private readonly ILogger<EditProfileWorkflow> _logger;

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

    private int? _messageId;
    private string _callbackQueryId;
    private State _state;
    private StateMachine<State, Trigger> _machine;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _showProfileInfoTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectLanguageTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _updateCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _updateLanguageTrigger;

    public EditProfileWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService, ILanguageService languageService, IMapper mapper, ILogger<EditProfileWorkflow> logger) =>
        (_botClient, _appUserService, _countryService, _languageService, _mapper, _logger) =
            (botClient, appUserService, countryService, languageService, mapper, logger);

    public override WorkflowType Type => WorkflowType.EditProfile;
    
    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        var state = State.Initial;
        var trigger = Trigger.ShowProfileInfo;
        var entityId = (long?)null;
        if (update.CallbackQuery is not null)
        {
            _callbackQueryId = update.CallbackQuery.Id;
            _messageId = update.CallbackQuery.Message.MessageId;
            var callbackQueryDto = _mapper.Map<CallbackQueryDto>(update.CallbackQuery.Data);
            var (changedState, changedTrigger, changedEntityId) = callbackQueryDto.EditProfileWorkflowDto;
            state = changedState;
            trigger = changedTrigger;
            entityId = changedEntityId;
        }
        
        await ConfigureStateMachine(state)
            .TriggerAsync(trigger, entityId, cancellationToken);
    }
    
    private EditProfileWorkflow ConfigureStateMachine(State state)
    {
        _state = state;

        _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

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

        _machine.Configure(State.Initial)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        _machine.Configure(State.ProfileInfoShowing)
            .OnEntryFromAsync(_showProfileInfoTrigger, ShowProfileInfoAsync)
            .Permit(Trigger.SelectCountry, State.CountrySelection)
            .Permit(Trigger.SelectLanguage, State.LanguageSelection);

        _machine.Configure(State.CountrySelection)
            
            .OnEntryFromAsync(_selectCountryTrigger, ShowCountriesAsync)
            .Permit(Trigger.UpdateCountry, State.CountryUpdated)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        _machine.Configure(State.LanguageSelection)
            .OnEntryFromAsync(_selectLanguageTrigger, ShowLanguagesAsync)
            .Permit(Trigger.UpdateLanguage, State.LanguageUpdated)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        _machine.Configure(State.CountryUpdated)
            .OnEntryFromAsync(_updateCountryTrigger, UpdateAppUserCountryAsync)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        _machine.Configure(State.LanguageUpdated)
            .OnEntryFromAsync(_updateLanguageTrigger, UpdateAppUserLanguageAsync)
            .Permit(Trigger.ShowProfileInfo, State.ProfileInfoShowing);

        _machine.OnTransitioned(t =>
            _logger.LogInformation(
                "OnTransitioned: {Source} -> {Destination} via {Trigger}({Parameters})", t.Source, t.Destination,
                t.Trigger, string.Join(", ", t.Parameters)));

        return this;
    }
    
    private async Task TriggerAsync(Trigger trigger, long? entityId = default, CancellationToken cancellationToken = default)
    {
        var handler = trigger switch
        {
            Trigger.ShowProfileInfo => _machine.FireAsync(_showProfileInfoTrigger, cancellationToken),
            Trigger.SelectCountry => _machine.FireAsync(_selectCountryTrigger, cancellationToken),
            Trigger.SelectLanguage => _machine.FireAsync(_selectLanguageTrigger, cancellationToken),
            Trigger.UpdateCountry => _machine.FireAsync(_updateCountryTrigger, entityId, cancellationToken),
            Trigger.UpdateLanguage => _machine.FireAsync(_updateLanguageTrigger, entityId, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(trigger), trigger,
                "There is an unsupported trigger type provided")
        };

        await handler;
    }
    
    private async Task ShowProfileInfoAsync(CancellationToken cancellationToken)
    {
        var message = BotMessage.GetProfileInfoMessage(_appUserService.Current);
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButton("Change country",
                new EditProfileWorkflowDto { State = State.ProfileInfoShowing, Trigger = Trigger.SelectCountry }
                    .ToCallbackQueryDto()
            )
            .AddButton("Change language",
                new EditProfileWorkflowDto { State = State.ProfileInfoShowing, Trigger = Trigger.SelectLanguage }
                    .ToCallbackQueryDto()
            )
            .ChunkBy(2)
            .Build();

        if (_messageId is not null)
        {
            await _botClient.EditMessageTextAsync(ChatId, _messageId.Value, message, ParseMode.MarkdownV2, replyMarkup,
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
                var cbData = new EditProfileWorkflowDto
                {
                    State = State.CountrySelection, Trigger = Trigger.UpdateCountry, EntityId = c.Id
                }.ToCallbackQueryDto();

                return (text, cbData);
            }))
            .WithBackButton(new EditProfileWorkflowDto { State = State.CountrySelection, Trigger = Trigger.ShowProfileInfo }
                .ToCallbackQueryDto())
            .ChunkBy(2)
            .Build();

        Debug.Assert(_messageId != null, nameof(_messageId) + " != null");
        await _botClient.EditMessageTextAsync(ChatId, _messageId.Value, message, ParseMode.MarkdownV2, replyMarkup,
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
                var cbData = new EditProfileWorkflowDto
                {
                    State = State.LanguageSelection, Trigger = Trigger.UpdateLanguage, EntityId = (long?)l.Code
                }.ToCallbackQueryDto();

                return (text, cbData);
            }))
            .WithBackButton(new EditProfileWorkflowDto
                    { State = State.LanguageSelection, Trigger = Trigger.ShowProfileInfo }
                .ToCallbackQueryDto())
            .ChunkBy(2)
            .Build();

        Debug.Assert(_messageId != null, nameof(_messageId) + " != null");
        await _botClient.EditMessageTextAsync(ChatId, _messageId.Value, message, ParseMode.MarkdownV2, replyMarkup,
            cancellationToken);
    }

    private async Task UpdateAppUserCountryAsync(long? entityId, CancellationToken cancellationToken)
    {
        var newCountry = await _countryService.FirstAsync(c => c.Id == entityId, cancellationToken);

        await _appUserService.UpdateAsync(au => au.Country = newCountry, cancellationToken);

        var message = BotMessage.GetSelectedCountryMessage(newCountry.Name);
        _ = _botClient.AnswerCallbackQueryAsync(_callbackQueryId, message, cancellationToken);

        await TriggerAsync(Trigger.ShowProfileInfo, cancellationToken: cancellationToken);
    }

    private async Task UpdateAppUserLanguageAsync(long? entityId, CancellationToken cancellationToken)
    {
        var newLanguage =
            await _languageService.FirstOrDefaultAsync(l => l.Code == (Language.LanguageCode)entityId,
                cancellationToken);

        await _appUserService.UpdateAsync(au => au.Language = newLanguage, cancellationToken);

        var message = BotMessage.GetSelectedLanguageMessage(newLanguage.Name);
        _ = _botClient.AnswerCallbackQueryAsync(_callbackQueryId, message, cancellationToken: cancellationToken);

        await TriggerAsync(Trigger.ShowProfileInfo, cancellationToken: cancellationToken);
    }

    private long? ChatId => _appUserService.Current.ChatId;
}