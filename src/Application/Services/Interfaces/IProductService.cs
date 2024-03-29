using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface IProductService
{
    Task<Product> CreateAsync(Product newProduct, CancellationToken cancellationToken = default);

    Task UpdateLastNotPublishedAsync(long sellerId, Action<Product> updateAction,
        CancellationToken cancellationToken = default);

    Task<Product> GetProductByIdAsync(long productId, CancellationToken cancellationToken = default);

    Task<Product> GetLastProductAsync(long sellerId, CancellationToken cancellationToken = default);

    Task<Product> GetInProgressProductAsync(long sellerId, CancellationToken cancellationToken = default);

    Task AttachPhotoToLastNotPublishedProductAsync(long sellerId, string photoId,
        CancellationToken cancellationToken = default);

    Task<List<ProductCategory>> GetProductCategoriesAsync(CancellationToken cancellationToken = default);

    Task<List<ProductCategory>> GetProductCategoriesAsync(Expression<Func<ProductCategory, bool>> expression,
        CancellationToken cancellationToken = default);

    Task<ProductCategory> GetSingleProductCategoryAsync(Expression<Func<ProductCategory, bool>> expression,
        CancellationToken cancellationToken = default);

    Task<List<ProductCondition>> GetProductConditionsAsync(CancellationToken cancellationToken = default);

    Task<ProductCondition> GetSingleProductConditionAsync(Expression<Func<ProductCondition, bool>> expression,
        CancellationToken cancellationToken = default);

    Task<List<Currency>> GetProductCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<Currency> GetSingleProductCurrencyAsync(Expression<Func<Currency, bool>> expression,
        CancellationToken cancellationToken = default);
}