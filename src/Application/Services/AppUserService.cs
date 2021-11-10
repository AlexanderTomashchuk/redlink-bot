using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Application.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILanguageService _languageService;

        public AppUserService(IApplicationDbContext context, ILanguageService languageService)
        {
            _context = context;
            _languageService = languageService;
        }

        public Task<AppUser> GetAsync(long appUserId, CancellationToken cancellationToken = default)
        {
            var appUser = GetAsync(u => u.Id == appUserId, cancellationToken);
            return appUser;
        }

        public Task<AppUser> GetAsync(Expression<Func<AppUser, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            var appUser = _context.Users
                .Include(u => u.Country)
                .Include(u => u.Language)
                .FirstOrDefaultAsync(expression, cancellationToken);
            return appUser;
        }

        public async Task<AppUser> CreateOrUpdateAsync(long chatId, User user,
            CancellationToken cancellationToken = default)
        {
            var userLanguage = await _languageService.FirstOrDefaultAsync(l =>
                l.Code == user.LanguageCode.ToLower(), cancellationToken);

            var appUser = await GetAsync(user.Id, cancellationToken);

            if (appUser != null)
            {
                //todo: OT use automapper
                appUser.FirstName = user.FirstName;
                appUser.LastName = user.LastName;
                appUser.Username = user.Username;
                appUser.LanguageId = userLanguage.Id;
                appUser.ChatId = chatId;
            }
            else
            {
                //todo: OT use automapper
                appUser = new AppUser(user.Id, user.FirstName, user.LastName, user.Username, chatId: chatId,
                    languageId: userLanguage.Id);
                await _context.Users.AddAsync(appUser, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return appUser;
        }

        public async Task<AppUser> SetCountryAsync(long countryId, User user, CancellationToken cancellationToken)
        {
            var appUser = await GetAsync(user.Id, cancellationToken);

            appUser.CountryId = countryId;

            await _context.SaveChangesAsync(cancellationToken);

            return appUser;
        }
    }
}