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

    public async Task<Product> UpdateLastNotPublishedAsync(long sellerId, Action<Product> updateAction,
        CancellationToken cancellationToken = default)
    {
        var lastUserProduct = await GetLastNotPublishedProductAsync(sellerId);

        //todo: fix this shit
        if (lastUserProduct is null) return lastUserProduct;
        
        updateAction.Invoke(lastUserProduct);
        await _context.SaveChangesAsync(cancellationToken);
        return lastUserProduct;
    }

    public async Task<Product> GetLastProductAsync(long sellerId, CancellationToken cancellationToken = default)
        => await ProductsQuery.OrderByDescending(p => p.CreatedOn).FirstOrDefaultAsync(p => p.SellerId == sellerId,cancellationToken);


    public async Task<Product> GetLastNotPublishedProductAsync(long sellerId,CancellationToken cancellationToken = default)
        => await ProductsQuery.FirstOrDefaultAsync(p =>
            p.SellerId == sellerId && p.CurrentState != ProductState.Finished, cancellationToken);

    public async Task AttachPhotoToLastNotPublishedProductAsync(long sellerId, string photoId,
        CancellationToken cancellationToken = default)
    {
        var lastUserProduct = await GetLastNotPublishedProductAsync(sellerId);

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