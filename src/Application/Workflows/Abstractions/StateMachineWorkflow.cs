using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows.Abstractions;

public abstract class StateMachineWorkflow<TState, TTrigger> : Workflow
    where TState : struct
    where TTrigger : struct
{
    private readonly ILogger _logger;

    protected StateMachineWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        ILogger logger, WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, workflowFactory)
        => _logger = logger;

    protected abstract Task TriggerNextAsync(TTrigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default);

    protected abstract void ConfigureStateMachine();

    protected StateMachine<TState, TTrigger> Machine { get; set; }

    private void ConfigureStateMachineBase()
    {
        ConfigureStateMachine();

        Machine.OnTransitioned(t =>
            _logger.LogInformation(
                "OnTransitioned: {Source} -> {Destination} via {Trigger}({Parameters})", t.Source, t.Destination,
                t.Trigger, string.Join(", ", t.Parameters)));
    }

    protected override async Task ProcessAsync(Update update, CancellationToken cancellationToken = default)
    {
        ConfigureStateMachineBase();

        await TriggerNextAsync(cancellationToken: cancellationToken);
    }
}