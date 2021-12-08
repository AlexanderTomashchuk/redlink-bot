using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Application.Workflows.UnknownUpdate;

public class UnknownUpdateWorkflow : Workflow
{
    private readonly IAppUserService _appUserService;
    private readonly ILogger<UnknownUpdateWorkflow> _logger;

    public UnknownUpdateWorkflow(IAppUserService appUserService, ILogger<UnknownUpdateWorkflow> logger)
    {
        _appUserService = appUserService;
        _logger = logger;
    }

    public override WorkflowType Type => WorkflowType.Unknown;

    public override Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("User {Username} sent the unknown update type {UpdateType}. Message: {Message}",
            _appUserService.Current.GetUsername(), update.Type, update.Message?.Text);

        return Task.CompletedTask;
    }
}