using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class LanguageService : ILanguageService
{
    private readonly IApplicationDbContext _context;

    public LanguageService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Language> FirstOrDefaultAsync(Expression<Func<Language, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var language = await _context.Languages.FirstOrDefaultAsync(predicate, cancellationToken) ??
                       await _context.Languages.FirstAsync(l =>
                           l.Code == Language.DefaultLanguageCode, cancellationToken);

        return language;
    }

    public Task<List<Language>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var allLanguages = _context.Languages.OrderBy(l => l.Code).ToListAsync(cancellationToken);

        return allLanguages;
    }
}