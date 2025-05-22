using Microsoft.EntityFrameworkCore;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Activation;

public class ActivateProductHandlerTests : IntegrationTestsBase
{
    private ActivateProductsHandler CreateHandler()
    {
        var context = new ProductDbContext(ProductDbOptions);
        var repo = new ProductRepository(context);
        return new ActivateProductsHandler(repo);
    }

    private async Task SeedProductsAsync(IEnumerable<Product> products)
    {
        foreach (var product in products)
            await SeedProductAsync(product);
    }

    [Fact]
    public async Task Handle_ShouldActivateAllUserProducts()
    {
        /*arrange*/
        var userId = 42;
        await SeedProductsAsync(new[]
        {
            new Product { Name = "Deleted A", Description = "Product A", Price = 100, IsDeleted = true, UserId = userId },
            new Product { Name = "Deleted B", Description = "Product B", Price = 200, IsDeleted = true, UserId = userId }
        });

        var handler = CreateHandler();
        var command = new ActivateProductsCommand { UserId = userId };

        /*act*/
        await handler.Handle(command, CancellationToken.None);

        /*assert*/
        await using var context = new ProductDbContext(ProductDbOptions);
        var userProducts = await context.Products.Where(p => p.UserId == userId).ToListAsync();
        Assert.All(userProducts, p => Assert.False(p.IsDeleted));
    }

    [Fact]
    public async Task Handle_ShouldLeaveProductsUnchanged_WhenAlreadyActive()
    {
        /*arrange*/
        var userId = 100;
        await SeedProductAsync(new Product
        {
            Name = "Already Active",
            Description = "Nothing to change",
            Price = 100,
            IsAvailable = true,
            IsDeleted = false,
            UserId = userId
        });

        var handler = CreateHandler();
        await handler.Handle(new ActivateProductsCommand { UserId = userId }, CancellationToken.None);

        /*act + assert*/
        await using var context = new ProductDbContext(ProductDbOptions);
        var product = await context.Products.FirstAsync(p => p.UserId == userId);
        Assert.False(product.IsDeleted); // Still false
    }

    [Fact]
    public async Task Handle_ShouldActivateOnlyDeletedProducts_WhenMixed()
    {
        /*arrange*/
        var userId = 200;
        await SeedProductsAsync(new[]
        {
            new Product { Name = "Deleted A", Description = "Should activate", Price = 100, IsDeleted = true, UserId = userId },
            new Product { Name = "Deleted B", Description = "Should activate", Price = 150, IsDeleted = true, UserId = userId },
            new Product { Name = "Active Product", Description = "Should remain", Price = 180, IsDeleted = false, UserId = userId }
        });

        var handler = CreateHandler();

        /*act*/
        await handler.Handle(new ActivateProductsCommand { UserId = userId }, CancellationToken.None);

        /*assert*/
        await using var context = new ProductDbContext(ProductDbOptions);
        var products = await context.Products.Where(p => p.UserId == userId).ToListAsync();
        Assert.All(products, p => Assert.False(p.IsDeleted));
    }
}