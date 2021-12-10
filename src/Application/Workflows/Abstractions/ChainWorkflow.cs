using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Application.Workflows.Abstractions;

public interface IChainWorkflow
{
    Task EntryWorkflowAsync();
    
    Task ExitWorkflowAsync();
    
    Task AbortWorkflowAsync(Update update, CancellationToken cancellationToken);
}