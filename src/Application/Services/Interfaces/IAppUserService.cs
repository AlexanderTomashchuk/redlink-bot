using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Telegram.Bot.Types;

namespace Application.Services.Interfaces
{
    public interface IAppUserService
    {
        Task<AppUser> GetAsync(long appUserId, CancellationToken cancellationToken);
        Task<AppUser> GetAsync(Expression<Func<AppUser, bool>> expression, CancellationToken cancellationToken);
        Task<AppUser> CreateOrUpdateAsync(long chatId, User user, CancellationToken cancellationToken);
        Task<AppUser> SetCountryAsync(long countryId, User user, CancellationToken cancellationToken);
    }
}