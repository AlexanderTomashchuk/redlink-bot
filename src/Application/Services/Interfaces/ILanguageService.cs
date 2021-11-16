using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface ILanguageService
{
    Task<Language> FirstOrDefaultAsync(Expression<Func<Language, bool>> predicate, CancellationToken cancellationToken);
}