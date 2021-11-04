using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions,
            ICurrentUserService currentUserService,
            IDateTime dateTime) : base(dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCondition> ProductConditions { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<User> Users { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var currentUserId = await _currentUserService.GetUserIdAsync();

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserId;
                        entry.Entity.CreatedOn = _dateTime.UtcNow;
                        break;

                    case EntityState.Modified:

                        entry.Entity.ModifiedBy = currentUserId;
                        entry.Entity.ModifiedOn = _dateTime.UtcNow;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            //await DispatchEvents();

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        // private async Task DispatchEvents()
        // {
        //     while (true)
        //     {
        //         var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
        //             .Select(x => x.Entity.DomainEvents)
        //             .SelectMany(x => x)
        //             .Where(domainEvent => !domainEvent.IsPublished)
        //             .FirstOrDefault();
        //         if (domainEventEntity == null) break;
        //
        //         domainEventEntity.IsPublished = true;
        //         await _domainEventService.Publish(domainEventEntity);
        //     }
        // }
    }
}