using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface ICountryService
{
    Task<List<Country>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Country> FirstAsync(Expression<Func<Country, bool>> predicate, CancellationToken cancellationToken = default);
}