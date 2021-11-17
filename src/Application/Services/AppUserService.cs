using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Application.Services;

public class AppUserService : IAppUserService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AppUserService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private AppUser _currentAppUser;

    public AppUser Current => _currentAppUser;

    public async Task InitAsync(Update update, CancellationToken cancellationToken = default)
    {
        var appUserFromRequest = _mapper.Map<AppUser>(update);

        _currentAppUser = await UpsertAsync(appUserFromRequest, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Func<AppUser, IApplicationDbContext, Task> updateOtherProperties,
        CancellationToken cancellationToken = default)
    {
        _currentAppUser = await UpsertAsync(_currentAppUser, updateOtherProperties, cancellationToken);
    }

    private async Task<AppUser> UpsertAsync(AppUser appUserFromRequest,
        Func<AppUser, IApplicationDbContext, Task> updateOtherProperties = null,
        CancellationToken cancellationToken = default)
    {
        var appUserFromDb = await GetByIdAsync(appUserFromRequest.Id, cancellationToken);

        if (appUserFromDb == null)
        {
            return await InsertAsync(appUserFromRequest, cancellationToken);
        }

        appUserFromDb.FirstName = appUserFromRequest.FirstName;
        appUserFromDb.LastName = appUserFromRequest.LastName;
        appUserFromDb.Username = appUserFromRequest.Username;
        appUserFromDb.ChatId = appUserFromRequest.ChatId;

        if (updateOtherProperties is not null)
        {
            await updateOtherProperties.Invoke(appUserFromDb, _context);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return appUserFromDb;
    }

    private async Task<AppUser> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var appUserFromDb = await _context.Users
            .Include(u => u.Country)
            .Include(u => u.Language)
            .FirstOrDefaultAsync(au => au.Id == id, cancellationToken);

        return appUserFromDb;
    }

    private async Task<AppUser> InsertAsync(AppUser appUser, CancellationToken cancellationToken = default)
    {
        var result = await _context.Users.AddAsync(appUser, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }
}