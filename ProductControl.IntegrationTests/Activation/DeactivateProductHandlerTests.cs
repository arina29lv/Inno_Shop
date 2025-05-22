using Microsoft.EntityFrameworkCore;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Activation;

public class DeactivateProductHandlerTests : IntegrationTestsBase
{
    private DeactivateProductsHandler CreateHandler()
    {
        var context = new ProductDbContext(ProductDbOptions);
        var repo = new ProductRepository(context);
        return new DeactivateProductsHandler(repo);
    }

    private async Task SeedProductsAsync(params Product[] products)
    {
        foreach (var product in products)
            await SeedProductAsync(product);
    }

    [Fact]
    public async Task Handle_ShouldDeactivateAllUserProducts()
    {
        /*arrange*/
        var userId = 101;
        await SeedProductsAsync(
            new Product { Name = "Product A", Description = "To deactivate", Price = 99, IsDeleted = false, UserId = userId },
            new Product { Name = "Product B", Description = "To deactivate", Price = 199, IsDeleted = false, UserId = userId }
        );

        var handler = CreateHandler();

        /*act*/
        await handler.Handle(new DeactivateProductsCommand { UserId = userId }, CancellationToken.None);

        /*assert*/
        await using var context = new ProductDbContext(ProductDbOptions);
        var products = await context.Products.Where(p => p.UserId == userId).ToListAsync();
        Assert.All(products, p => Assert.True(p.IsDeleted));
    }

    [Fact]
    public async Task Handle_ShouldNotFail_WhenUserHasNoProducts()
    {
        /*arrange*/
        var handler = CreateHandler();

        /*act*/
        var exception = await Record.ExceptionAsync(() =>
            handler.Handle(new DeactivateProductsCommand { UserId = 999 }, CancellationToken.None));

        /*assert*/
        Assert.Null(exception);
    }

    [Fact]
    public async Task Handle_ShouldLeaveProductsDeleted_WhenAlreadyDeleted()
    {
        /*arrange*/
        var userId = 202;
        await SeedProductAsync(new Product
        {
            Name = "Deleted",
            Description = "Already deleted",
            Price = 150,
            IsDeleted = true,
            UserId = userId
        });

        var handler = CreateHandler();

        /*act*/
        await handler.Handle(new DeactivateProductsCommand { UserId = userId }, CancellationToken.None);

        /*assert*/
        await using var context = new ProductDbContext(ProductDbOptions);
        var product = await context.Products.FirstAsync(p => p.UserId == userId);
        Assert.True(product.IsDeleted);
    }

    [Fact]
    public async Task Handle_ShouldDeactivateMixedStateProducts()
    {
        /*arrange*/
        var userId = 303;
        await SeedProductsAsync(
            new Product { Name = "Active", Description = "Will be deactivated", Price = 120, IsDeleted = false, UserId = userId },
            new Product { Name = "Deleted", Description = "Already deleted", Price = 80, IsDeleted = true, UserId = userId }
        );

        var handler = CreateHandler();

        /*act*/
        await handler.Handle(new DeactivateProductsCommand { UserId = userId }, CancellationToken.None);

        /*assert*/
        await using var context = new ProductDbContext(ProductDbOptions);
        var products = await context.Products.Where(p => p.UserId == userId).ToListAsync();
        Assert.All(products, p => Assert.True(p.IsDeleted));
    }
}