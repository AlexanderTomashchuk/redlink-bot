using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Application.Workflows.FindProduct;

public class FindProductWorkflow : Workflow
{
    public override WorkflowType Type => WorkflowType.FindProduct;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
    }
}