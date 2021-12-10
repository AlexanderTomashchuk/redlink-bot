using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows.UnknownUpdate;

public class UnknownUpdateWorkflow : SimpleWorkflow
{
    private readonly ILogger<UnknownUpdateWorkflow> _logger;

    public UnknownUpdateWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        ILogger<UnknownUpdateWorkflow> logger, WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, workflowFactory)
    {
        _logger = logger;
    }

    protected override WorkflowType WorkflowType => WorkflowType.Unknown;

    protected override Task ProcessAsync(Update update, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("User {Username} sent the unknown update type {UpdateType}. Message: {Message}",
            CurrentAppUser.GetUsername(), update.Type, update.Message?.Text);

        return Task.CompletedTask;
    }
}