using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface IProductService
{
    Task<Product> CreateAsync(Product newProduct, CancellationToken cancellationToken = default);

    Task<Product> UpdateAsync(AppUser seller, Action<Product> updateAction,
        CancellationToken cancellationToken = default);
}