using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Application.Workflows;

public abstract class Workflow
{
    // ReSharper disable once UnusedMember.Global
    public abstract WorkflowType Type { get; }

    public abstract Task RunAsync(Update update, CancellationToken cancellationToken = default);
}