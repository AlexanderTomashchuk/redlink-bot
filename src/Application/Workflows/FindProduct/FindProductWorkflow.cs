using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows.FindProduct;

public class FindProductWorkflow : Workflow
{
    public FindProductWorkflow(ITelegramBotClient botClient, IAppUserService appUserService)
        : base(botClient, appUserService)
    {
    }

    public override WorkflowType Type => WorkflowType.FindProduct;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
    }
}