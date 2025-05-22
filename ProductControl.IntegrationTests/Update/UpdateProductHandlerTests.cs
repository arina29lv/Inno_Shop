using AutoMapper;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Update;

public class UpdateProductHandlerTests : IntegrationTestsBase
{
    private readonly IMapper _mapper;

    public UpdateProductHandlerTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UpdateProductCommand, Product>();
        }));
    }

    private UpdateProductHandler CreateHandler() =>
        new(new ProductRepository(new ProductDbContext(ProductDbOptions)), _mapper);

    private static UpdateProductCommand CreateCommand(int productId, int userId, string role) =>
        new()
        {
            Id = productId,
            Name = "Updated Name",
            Description = "Updated Desc",
            Price = 99,
            IsAvailable = false,
            UpdatorId = userId,
            UpdatorRole = role
        };

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenCalledByAdmin()
    {
        /*arrange*/
        var product = new Product
        {
            Name = "Original",
            Description = "Original Desc",
            Price = 10,
            IsAvailable = true,
            UserId = 1
        };
        await SeedProductAsync(product);

        var handler = CreateHandler();
        var command = CreateCommand(product.Id, 999, "Admin");

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        await using var context = new ProductDbContext(ProductDbOptions);
        var updated = await context.Products.FindAsync(product.Id);

        Assert.NotNull(updated);
        Assert.Equal(command.Name, updated!.Name);
        Assert.Equal(command.Description, updated.Description);
        Assert.Equal(command.Price, updated.Price);
        Assert.Equal(command.IsAvailable, updated.IsAvailable);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenCalledByOwner()
    {
        /*arrange*/
        var userId = 55;
        var product = new Product
        {
            Name = "Old",
            Description = "Desc",
            Price = 20,
            IsAvailable = true,
            UserId = userId
        };
        await SeedProductAsync(product);

        var handler = CreateHandler();
        var command = CreateCommand(product.Id, userId, "User");

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        await using var context = new ProductDbContext(ProductDbOptions);
        var updated = await context.Products.FindAsync(product.Id);

        Assert.Equal(command.Name, updated!.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenNotOwnerAndNotAdmin()
    {
        /*arrange*/
        var product = new Product
        {
            Name = "Secure",
            Description = "Should not update",
            Price = 200,
            IsAvailable = true,
            UserId = 10
        };
        await SeedProductAsync(product);

        var handler = CreateHandler();
        var command = CreateCommand(product.Id, 999, "User");

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        await using var context = new ProductDbContext(ProductDbOptions);
        var unchanged = await context.Products.FindAsync(product.Id);

        Assert.Equal("Secure", unchanged!.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        /*arrange*/
        var handler = CreateHandler();
        var command = CreateCommand(9999, 1, "Admin");

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
    }
}