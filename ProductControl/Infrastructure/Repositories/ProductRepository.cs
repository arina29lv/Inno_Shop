using Microsoft.EntityFrameworkCore;
using ProductControl.Domain.Interfaces;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;

namespace ProductControl.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    
    public async Task<IEnumerable<Product>> GetProductsByUserIdAsync(int userId)
    {
        var products = await _context.Products
            .Where(p => p.UserId == userId)
            .ToListAsync();
        
        return products;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}