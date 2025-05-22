using AutoMapper;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Creation;

public class CreateProductHandlerTests : IntegrationTestsBase
{
    private CreateProductHandler CreateHandler()
    {
        var mapper = new Mapper(new MapperConfiguration(cfg =>
            cfg.CreateMap<CreateProductCommand, Product>()));

        var context = new ProductDbContext(ProductDbOptions);
        var repo = new ProductRepository(context);

        return new CreateProductHandler(repo, mapper);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenCommandIsValid()
    {
        /*arrange*/
        var command = new CreateProductCommand
        {
            Name = "New Product",
            Description = "Description of new product",
            Price = 49.99m,
            IsAvailable = true,
            UserId = 123
        };

        var handler = CreateHandler();

        /*act*/
        var productId = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(productId > 0);

        await using var context = new ProductDbContext(ProductDbOptions);
        var created = await context.Products.FindAsync(productId);

        Assert.NotNull(created);
        Assert.Equal(command.Name, created!.Name);
        Assert.Equal(command.Description, created.Description);
        Assert.Equal(command.Price, created.Price);
        Assert.Equal(command.IsAvailable, created.IsAvailable);
        Assert.Equal(command.UserId, created.UserId);
        Assert.False(created.IsDeleted);
    }
}