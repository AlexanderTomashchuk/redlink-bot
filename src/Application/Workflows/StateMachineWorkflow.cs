using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows;

public abstract class StateMachineWorkflow<TState, TTrigger> : Workflow
    where TState : struct
    where TTrigger : struct
{
    protected readonly ITelegramBotClient BotClient;
    protected readonly IAppUserService AppUserService;
    protected readonly IMapper Mapper;
    protected readonly ILogger Logger;

    protected StateMachineWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        ILogger logger)
        : base(botClient, appUserService)
    {
        BotClient = botClient;
        AppUserService = appUserService;
        Mapper = mapper;
        Logger = logger;
    }

    protected Update Update { get; private set; }

    protected StateMachine<TState, TTrigger> Machine { get; set; }

    protected abstract TTrigger GetTriggerToInvoke();

    protected abstract Task TriggerAsync(TTrigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default);

    protected abstract void ConfigureStateMachine();

    private void BaseConfigureStateMachine()
    {
        ConfigureStateMachine();

        Machine.OnTransitioned(t =>
            Logger.LogInformation(
                "OnTransitioned: {Source} -> {Destination} via {Trigger}({Parameters})", t.Source, t.Destination,
                t.Trigger, string.Join(", ", t.Parameters)));
    }

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        Update = update;

        BaseConfigureStateMachine();

        await TriggerAsync(cancellationToken: cancellationToken);
    }

    protected string MessageText => Update?.Message?.Text;

    protected string MessagePhotoId => Update?.Message?.Photo?.MaxBy(p => p.FileSize)?.FileId;

    protected string CallbackQueryId => Update?.CallbackQuery?.Id;

    protected int? MessageIdBelongsToCb => Update?.CallbackQuery?.Message?.MessageId;

    protected T GetCbData<T>() where T : CallbackQueryDto => Mapper.Map<T>(Update?.CallbackQuery?.Data);

    protected long? GetEntityId<T>() where T : CallbackQueryDto => GetCbData<T>()?.EntityId;
}