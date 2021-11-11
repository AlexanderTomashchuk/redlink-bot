using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IApplicationDbContext _context;

        public AppUserService(IApplicationDbContext context)
        {
            _context = context;
        }

        private AppUser _currentAppUser;

        public AppUser Current => _currentAppUser;

        public async Task InitAsync(AppUser appUser, CancellationToken cancellationToken = default)
        {
            _currentAppUser = await UpsertAsync(appUser, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(Action<AppUser> updateOtherProperties,
            CancellationToken cancellationToken = default)
        {
            _currentAppUser = await UpsertAsync(_currentAppUser, updateOtherProperties, cancellationToken);
        }

        private async Task<AppUser> UpsertAsync(AppUser appUser, Action<AppUser> updateOtherProperties = null,
            CancellationToken cancellationToken = default)
        {
            var appUserFromDb = await _context.Users.FirstOrDefaultAsync(au => au.Id == appUser.Id, cancellationToken);

            if (appUserFromDb == null)
            {
                return await InsertAsync(appUser, cancellationToken);
            }

            appUserFromDb.FirstName = appUser.FirstName;
            appUserFromDb.LastName = appUser.LastName;
            appUserFromDb.Username = appUser.Username;
            appUserFromDb.ChatId = appUser.ChatId;

            updateOtherProperties?.Invoke(appUserFromDb);

            await _context.SaveChangesAsync(cancellationToken);

            return appUserFromDb;
        }

        private async Task<AppUser> InsertAsync(AppUser appUser, CancellationToken cancellationToken = default)
        {
            var result = await _context.Users.AddAsync(appUser, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return result.Entity;
        }

        //TODO: TOMORROW 1. set default language based on code received from telegram client (use enum as primary key - https://medium.com/agilix/entity-framework-core-enums-ee0f8f4063f2) 
        //TODO: TOMORROW 2. ask country on any update if countryId is null
        //TODO: TOMORROW 3. implement /profile bot command 
    }
}