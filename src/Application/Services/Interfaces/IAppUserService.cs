using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Telegram.Bot.Types;

namespace Application.Services.Interfaces;

public interface IAppUserService
{
    public AppUser Current { get; }

    Task InitAsync(Update update, CancellationToken cancellationToken = default);

    Task UpdateAsync(Func<AppUser, IApplicationDbContext, Task> updateOtherProperties,
        CancellationToken cancellationToken = default);
}