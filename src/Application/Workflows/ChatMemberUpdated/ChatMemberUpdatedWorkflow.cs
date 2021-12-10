using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Domain.ValueObjects;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows.ChatMemberUpdated;

public class ChatMemberUpdatedWorkflow : SimpleWorkflow
{
    public ChatMemberUpdatedWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, workflowFactory)
    {
    }

    protected override WorkflowType WorkflowType => WorkflowType.ChatMemberUpdated;

    protected override async Task ProcessAsync(Update update, CancellationToken cancellationToken = default)
    {
        var appUserStatus = Mapper.Map<AppUserStatus>(update.MyChatMember.NewChatMember.Status);

        await AppUserService.UpdateAsync(au => au.Status = appUserStatus,
            cancellationToken);
    }
}