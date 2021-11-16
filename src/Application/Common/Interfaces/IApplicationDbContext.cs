using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Country> Countries { get; set; }

    DbSet<Currency> Currencies { get; set; }

    DbSet<Language> Languages { get; set; }

    DbSet<Product> Products { get; set; }

    DbSet<ProductCondition> ProductConditions { get; set; }

    DbSet<ProductType> ProductTypes { get; set; }

    DbSet<AppUser> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}