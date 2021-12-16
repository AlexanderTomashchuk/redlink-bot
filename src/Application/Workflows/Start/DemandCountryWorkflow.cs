using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using l10n = Application.Resources.Localization;

namespace Application.Workflows.Start;

public class DemandCountryWorkflow : StateMachineWorkflow<DemandCountryWorkflow.State, DemandCountryWorkflow.Trigger>,
    StateStorageMode.ITransitional<DemandCountryWorkflow.State>
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICountryService _countryService;

    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _setCountryTrigger;

    public DemandCountryWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService, IMapper mapper, ILogger<DemandCountryWorkflow> logger,
        WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, logger, workflowFactory)
        => (_botClient, _countryService) = (botClient, countryService);

    protected override WorkflowType WorkflowType => WorkflowType.DemandCountry;

    public State CurrentState => GetCbData<DemandCountryCbDto>()?.State ?? State.Initial;

    protected override void ConfigureStateMachine()
    {
        Machine = new StateMachine<State, Trigger>(CurrentState);

        _selectCountryTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken>(Trigger.SelectCountry);

        _setCountryTrigger =
            new StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken>(Trigger.SetCountry);

        Machine.Configure(State.Initial)
            .Permit(Trigger.SelectCountry, State.CountrySelection)
            .Permit(Trigger.DoNothing, State.CountrySelected);

        Machine.Configure(State.CountrySelection)
            .OnEntryFromAsync(_selectCountryTrigger, ShowCountriesAsync)
            .Permit(Trigger.SetCountry, State.CountrySelected);

        Machine.Configure(State.CountrySelected)
            .OnEntryFromAsync(_setCountryTrigger, SetAppUserCountryAsync);
    }

    protected override async Task TriggerNextAsync(Trigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default)
    {
        var trigger = GetNextTriggerToInvoke();
        var entityId = GetEntityId<DemandCountryCbDto>();
        var handler = trigger switch
        {
            Trigger.SelectCountry => Machine.FireAsync(_selectCountryTrigger, cancellationToken),
            Trigger.SetCountry => Machine.FireAsync(_setCountryTrigger, entityId, cancellationToken),
            Trigger.DoNothing => Task.CompletedTask,
            _ => throw new ArgumentOutOfRangeException(nameof(trigger), trigger,
                "There is an unsupported trigger type provided")
        };

        await handler;
    }

    private async Task ShowCountriesAsync(CancellationToken cancellationToken)
    {
        var chatId = CurrentAppUser.ChatId;

        var countries = await _countryService.GetAllAsync(cancellationToken);

        var text = new MessageTextBuilder(ParseMode.MarkdownV2)
            .AddTextLine($"{l10n.ChooseCountryFromList}.")
            .BreakLine()
            .AddTextLine($"{l10n.CountryHelpsProvideRelevantProducts}.", TextStyle.Italic)
            .Build();
        
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButtons(countries.Select(c =>
            {
                var text = $"{c.Flag} {l10n.ResourceManager.GetString(c.NameLocalizationKey)}";
                var cbData = new DemandCountryCbDto(State.CountrySelection, Trigger.SetCountry, c.Id);

                return (text, cbData);
            }))
            .ChunkBy(2)
            .Build();

        await _botClient.SendFormattedTxtMessageAsync(chatId, text, replyMarkup, cancellationToken);
    }

    private async Task SetAppUserCountryAsync(long? entityId, CancellationToken cancellationToken)
    {
        var country = await _countryService.FirstAsync(c => c.Id == entityId, cancellationToken);

        await AppUserService.UpdateAsync(au => au.Country = country, cancellationToken);
        
        var text = $"{l10n.SelectedCountry}: {l10n.ResourceManager.GetString(country.NameLocalizationKey)}";
        _ = BotClient.AnswerCbQueryAsync(CallbackQueryId, text, cancellationToken);
    }

    private Trigger GetNextTriggerToInvoke()
    {
        var cbTrigger = GetCbData<DemandCountryCbDto>()?.Trigger;

        if (cbTrigger is not null)
        {
            return cbTrigger.Value;
        }

        return CurrentAppUser.HasCountry ? Trigger.DoNothing : Trigger.SelectCountry;
    }

    public enum Trigger
    {
        SelectCountry,
        SetCountry,
        DoNothing
    }

    public enum State
    {
        Initial,
        CountrySelection,
        CountrySelected
    }
}