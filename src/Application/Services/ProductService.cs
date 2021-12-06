using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Product> UpdateAsync(AppUser seller, Action<Product> updateAction,
        CancellationToken cancellationToken = default)
    {
        var lastUserProduct = await GetLastUserProduct(seller);
        updateAction.Invoke(lastUserProduct);
        await _context.SaveChangesAsync(cancellationToken);
        return lastUserProduct;
    }

    private Task<Product> GetLastUserProduct(AppUser seller)
    {
        var lastProduct = _context.Products.OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync(p => p.SellerId == seller.Id);
        return lastProduct;
    }
}