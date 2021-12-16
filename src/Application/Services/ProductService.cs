using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context) => _context = context;

    public async Task<Product> CreateAsync(Product newProduct, CancellationToken cancellationToken = default)
    {
        var result = await _context.Products.AddAsync(newProduct, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<Product> GetLastProductAsync(long sellerId, CancellationToken cancellationToken = default)
    {
        return await ProductsQuery
            .OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync(p => p.SellerId == sellerId, cancellationToken);
    }

    public async Task<Product> GetInProgressProductAsync(long sellerId,
        CancellationToken cancellationToken = default)
    {
        return await ProductsQuery
            .OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync(p =>
                    p.SellerId == sellerId &&
                    !new[] { ProductState.ReadyForPublishing, ProductState.Published, ProductState.Aborted }.Contains(
                        p.CurrentState),
                cancellationToken);
    }

    public async Task UpdateLastNotPublishedAsync(long sellerId, Action<Product> updateAction,
        CancellationToken cancellationToken = default)
    {
        var lastUserProduct = await GetInProgressProductAsync(sellerId);

        if (lastUserProduct is null) return;

        updateAction.Invoke(lastUserProduct);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AttachPhotoToLastNotPublishedProductAsync(long sellerId, string photoId,
        CancellationToken cancellationToken = default)
    {
        var lastUserProduct = await GetInProgressProductAsync(sellerId, cancellationToken);

        if (lastUserProduct is not null)
        {
            lastUserProduct.Files.Add(new File { TelegramId = photoId, ProductId = lastUserProduct.Id });
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<ProductCondition>> GetProductConditionsAsync(CancellationToken cancellationToken = default)
    {
        var allProductConditions = await _context.ProductConditions
            .OrderBy(pc => pc.Id)
            .ToListAsync(cancellationToken);

        return allProductConditions;
    }

    private IIncludableQueryable<Product, ICollection<File>> ProductsQuery =>
        _context.Products
            .Include(p => p.Condition)
            .Include(p => p.Files);
}