using Microsoft.EntityFrameworkCore;
using ProductControl.Domain.Models;

namespace ProductControl.Infrastructure.Persistence;

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options){}
}