using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IAppUserService
    {
        public AppUser Current { get; }

        Task InitAsync(AppUser appUser, CancellationToken cancellationToken = default);
        Task UpdateAsync(Action<AppUser> updateOtherProperties, CancellationToken cancellationToken = default);
    }
}