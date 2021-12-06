using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Workflows;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Bot.WebHook.Services;

public class HandleUpdateService
{
    private readonly WorkflowFactory _workflowFactory;
    private readonly ILogger<HandleUpdateService> _logger;

    public HandleUpdateService(WorkflowFactory workflowFactory, ILogger<HandleUpdateService> logger)
    {
        _workflowFactory = workflowFactory;
        _logger = logger;
    }

    public async Task EchoAsync(Update update, CancellationToken cancellationToken)
    {
        var workflow = _workflowFactory.DetermineWorkflowAsync(update);
        try
        {
            await workflow.RunAsync(update, cancellationToken);
        }
        catch (Exception exception)
        {
            var errorMessage = JsonConvert.SerializeObject(exception, Formatting.Indented);

            _logger.LogError(errorMessage);
        }
    }
}