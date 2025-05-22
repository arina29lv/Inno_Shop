using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Deletion;

public class DeleteProductHandlerTests : IntegrationTestsBase
{
    private DeleteProductHandler CreateHandler()
    {
        var context = new ProductDbContext(ProductDbOptions);
        var repo = new ProductRepository(context);
        return new DeleteProductHandler(repo);
    }

    private async Task<Product> SeedAndGetProductAsync(int userId)
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            IsAvailable = true,
            UserId = userId
        };
        await SeedProductAsync(product);
        return product;
    }

    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenCalledByAdmin()
    {
        /*Arrange*/
        var product = await SeedAndGetProductAsync(userId: 1);
        var handler = CreateHandler();
        var command = new DeleteProductCommand { Id = product.Id, UserId = 999, UserRole = "Admin" };

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);

        await using var context = new ProductDbContext(ProductDbOptions);
        Assert.Null(await context.Products.FindAsync(product.Id));
    }

    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenCalledByOwner()
    {
        /*arrange*/
        var product = await SeedAndGetProductAsync(userId: 42);
        var handler = CreateHandler();
        var command = new DeleteProductCommand { Id = product.Id, UserId = 42, UserRole = "User" };

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);

        await using var context = new ProductDbContext(ProductDbOptions);
        Assert.Null(await context.Products.FindAsync(product.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenCalledByAnotherUser()
    {
        /*arrange*/
        var product = await SeedAndGetProductAsync(userId: 100);
        var handler = CreateHandler();
        var command = new DeleteProductCommand { Id = product.Id, UserId = 200, UserRole = "User" };

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);

        await using var context = new ProductDbContext(ProductDbOptions);
        Assert.NotNull(await context.Products.FindAsync(product.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        /*arrange*/
        var handler = CreateHandler();
        var command = new DeleteProductCommand { Id = 9999, UserId = 1, UserRole = "Admin" };

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
    }
}