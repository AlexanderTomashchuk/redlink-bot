using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.ValueObjects;
using Telegram.Bot.Types;

namespace Application.Processors;

public class MyChatMemberReceivedProcessor
{
    private readonly IAppUserService _appUserService;
    private readonly IMapper _mapper;

    public MyChatMemberReceivedProcessor(IAppUserService appUserService, IMapper mapper)
    {
        _appUserService = appUserService;
        _mapper = mapper;
    }

    public async Task ProcessAsync(ChatMemberUpdated chatMemberUpdated,
        CancellationToken cancellationToken = default)
    {
        //todo: use workflow?
        var appUserStatus = _mapper.Map<AppUserStatus>(chatMemberUpdated.NewChatMember.Status);

        await _appUserService.UpdateAsync(au => au.Status = appUserStatus,
            cancellationToken);
    }
}