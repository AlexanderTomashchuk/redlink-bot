using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CountryService : ICountryService
{
    private readonly IApplicationDbContext _context;

    public CountryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var allCountries = _context.Countries.ToListAsync(cancellationToken);

        return allCountries;
    }

    public async Task<Country> FirstAsync(Expression<Func<Country, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var country = await _context.Countries.FirstAsync(predicate, cancellationToken);

        return country;
    }
}