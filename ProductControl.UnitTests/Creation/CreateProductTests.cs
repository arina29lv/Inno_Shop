using Moq;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Creation;

public class CreateProductTests : BaseProductTestWithMapper
{
    private readonly CreateProductHandler _handler;

    public CreateProductTests()
    {
        _handler = new CreateProductHandler(ProductRepositoryMock.Object, MapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenCommandIsValid()
    {
        /*arrange*/
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Desc",
            Price = 9.99m,
            IsAvailable = true,
            UserId = 5
        };

        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            IsAvailable = command.IsAvailable,
            UserId = command.UserId.Value
        };

        MapperMock.Setup(m => m.Map<Product>(command)).Returns(product);
        ProductRepositoryMock.Setup(r => r.AddProductAsync(product)).ReturnsAsync(101);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Equal(101, result);
        MapperMock.Verify(m => m.Map<Product>(command), Times.Once);
        ProductRepositoryMock.Verify(r => r.AddProductAsync(product), Times.Once);
    }
}