using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface ICountryService
{
    Task<List<Country>> GetAllAsync(CancellationToken cancellationToken);
}