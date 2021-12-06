using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProductConditionService : IProductConditionService
{
    private readonly IApplicationDbContext _context;

    public ProductConditionService(IApplicationDbContext context) => _context = context;

    public Task<List<ProductCondition>> GetAllAsync(CancellationToken cancellationToken)
    {
        var allProductConditions = _context.ProductConditions
            .OrderBy(pc => pc.Id)
            .ToListAsync(cancellationToken);

        return allProductConditions;
    }
}