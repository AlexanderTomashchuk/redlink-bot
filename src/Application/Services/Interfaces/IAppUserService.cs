using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Telegram.Bot.Types;

namespace Application.Services.Interfaces
{
    public interface IAppUserService
    {
        public AppUser Current { get; }

        Task InitAsync(Update update, CancellationToken cancellationToken = default);
        Task UpdateAsync(Action<AppUser> updateOtherProperties, CancellationToken cancellationToken = default);
    }
}