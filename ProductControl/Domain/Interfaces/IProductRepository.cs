using ProductControl.Domain.Models;

namespace ProductControl.Domain.Interfaces;

public interface IProductRepository
{
    Task<int> AddProductAsync(Product product);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    
    Task<IEnumerable<Product>> GetProductsByUserIdAsync(int userId);
    Task SaveChangesAsync();
}