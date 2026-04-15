using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategory(string category, CancellationToken cancellationToken = default);
}
