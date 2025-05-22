using Microsoft.EntityFrameworkCore;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;

namespace ProductControl.IntegrationTests.Base;

public abstract class IntegrationTestsBase : IAsyncLifetime
{
    protected readonly DbContextOptions<ProductDbContext> ProductDbOptions;

    public IntegrationTestsBase()
    {
        ProductDbOptions = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase("ProductTestDb")
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var db = new ProductDbContext(ProductDbOptions);
        db.Products.RemoveRange(db.Products);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task SeedProductAsync(Product product)
    {
        await using var db = new ProductDbContext(ProductDbOptions);
        db.Products.Add(product);
        await db.SaveChangesAsync();
    }
}