using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly IDateTime _dateTime;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions,
        IDateTime dateTime) : base(dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
        _dateTime = dateTime;
    }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<Language> Languages { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductCondition> ProductConditions { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }

    public DbSet<AppUser> Users { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = _dateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedOn = _dateTime.UtcNow;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}