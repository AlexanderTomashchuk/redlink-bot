using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.ValueObjects;
using Telegram.Bot.Types;

namespace Application.Workflows.ChatMemberUpdated;

public class ChatMemberUpdatedWorkflow : Workflow
{
    private readonly IAppUserService _appUserService;
    private readonly IMapper _mapper;

    public ChatMemberUpdatedWorkflow(IAppUserService appUserService, IMapper mapper)
    {
        _appUserService = appUserService;
        _mapper = mapper;
    }

    public override WorkflowType Type => WorkflowType.ChatMemberUpdated;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        var appUserStatus = _mapper.Map<AppUserStatus>(update.MyChatMember.NewChatMember.Status);

        await _appUserService.UpdateAsync(au => au.Status = appUserStatus,
            cancellationToken);
    }
}