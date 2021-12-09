using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.Start;

public class DemandCountryWorkflow : StateMachineWorkflow<DemandCountryWorkflow.State, DemandCountryWorkflow.Trigger>,
    StateStorageMode.ITransitional<DemandCountryWorkflow.State>
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICountryService _countryService;

    StateMachine<State, Trigger>.TriggerWithParameters<CancellationToken> _selectCountryTrigger;
    StateMachine<State, Trigger>.TriggerWithParameters<long?, CancellationToken> _setCountryTrigger;

    public DemandCountryWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService, IMapper mapper, ILogger<DemandCountryWorkflow> logger) : base(botClient,
        appUserService, mapper, logger)
        => (_botClient, _countryService) = (botClient, countryService);

    public override WorkflowType Type => WorkflowType.DemandCountry;

    public State CurrentState => GetCbData<DemandCountryCbDto>()?.State ?? State.Initial;

    protected override Trigger GetTriggerToInvoke()
    {
        var cbTrigger = GetCbData<DemandCountryCbDto>()?.Trigger;

        if (cbTrigger is not null)
        {
            return cbTrigger.Value;
        }

        return CurrentAppUser.HasCountry ? Trigger.DoNothing : Trigger.SelectCountry;
    }

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

    protected override async Task TriggerAsync(Trigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default)
    {
        var trigger = GetTriggerToInvoke();
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

        var initCountryMessage = BotMessage.GetInitCountryMessage();
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButtons(countries.Select(c =>
            {
                var text = $"{c.Flag} {c.Name}";
                var cbData = new DemandCountryCbDto(State.CountrySelection, Trigger.SetCountry, c.Id);

                return (text, cbData);
            }))
            .ChunkBy(2)
            .Build();

        await _botClient.SendTextMessageAsync(chatId, initCountryMessage, ParseMode.MarkdownV2,
            replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }

    private async Task SetAppUserCountryAsync(long? entityId, CancellationToken cancellationToken)
    {
        var country = await _countryService.FirstAsync(c => c.Id == entityId, cancellationToken);

        await AppUserService.UpdateAsync(au => au.Country = country, cancellationToken);
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