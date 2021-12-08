using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot.Types;

namespace Application.Workflows;

public abstract class StateMachineWorkflow<TState, TTrigger> : Workflow
    where TState : struct
    where TTrigger : struct
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    

    protected StateMachineWorkflow(IMapper mapper, ILogger logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    protected Update Update { get; private set; }

    protected StateMachine<TState, TTrigger> Machine { get; set; }

    protected virtual TState InitialState { get; private set; }

    protected virtual TState StateAccessor() => InitialState;

    protected virtual void StateMutator(TState state) => InitialState = state;

    protected abstract TTrigger GetTriggerToInvoke();

    protected abstract Task TriggerAsync(TTrigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default);

    protected abstract void ConfigureStateMachine();

    private void BaseConfigureStateMachine()
    {
        ConfigureStateMachine();
        
        Machine.OnTransitioned(t =>
            _logger.LogInformation(
                "OnTransitioned: {Source} -> {Destination} via {Trigger}({Parameters})", t.Source, t.Destination,
                t.Trigger, string.Join(", ", t.Parameters)));
    }
    
    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        Update = update;

        BaseConfigureStateMachine();

        await TriggerAsync(cancellationToken: cancellationToken);
    }

    protected string CallbackQueryId => Update?.CallbackQuery?.Id;

    protected int? MessageIdBelongsToCb => Update?.CallbackQuery?.Message?.MessageId;

    protected T GetCbData<T>() where T : CallbackQueryDto => _mapper.Map<T>(Update?.CallbackQuery?.Data);

    protected long? GetEntityId<T>() where T : CallbackQueryDto => GetCbData<T>()?.EntityId;
}