using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface IProductConditionService
{
    Task<List<ProductCondition>> GetAllAsync(CancellationToken cancellationToken);
}